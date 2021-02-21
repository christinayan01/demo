//
//import * as THREE from './threejs/three.module.js';

//
var dir = '';
var name = '';
var scale = 1.0;
var canvas_id = '';
//
var diffuse_map = '';
var normal_map = '';
var roughness_map = '';
var metalness_map = '';
var alpha_map = '';

var url = location.origin + location.pathname;
var root_dir = url.substring(0, url.length - 8);

import * as THREE from './threejs/three.module.js';
import Stats from './threejs/jsm/libs/stats.module.js';

import { OrbitControls } from './threejs/jsm/controls/OrbitControls.js';
import { FBXLoader } from './threejs/jsm/loaders/FBXLoader.js';
import { RGBELoader } from './threejs/jsm/loaders/RGBELoader.js';

import { GUI } from './threejs/jsm/libs/dat.gui.module.js';

import { EffectComposer } from './threejs/jsm/postprocessing/EffectComposer.js'; //ssao
import { SSAOPass } from './threejs/jsm/postprocessing/SSAOPass.js';

var statsEnabled = false;
var container, stats;
var camera, scene, renderer, controls;
var camera_ortho, controls_ortho; // 平行投影
var composer;//ssao
var ssaoPass;//ssao
var useSSAO = false;//ssao
var inited = false;

var obj2 = null;

var mesh_ground = null;

var params = {
	exposure: 1.0,
	ground: false,
	sunrotate: 180,
	sunheight: 0.5
};

var directionalLight = null;

var bbox = null;  // バウンディングボックス

var isAutoRot = false;
var group_root;

//----------------------------
window.addEventListener('load', function(){
	dir = window.dir;
	name = this.name;
	scale = this.scale;
	canvas_id = this.canvas_id;

	diffuse_map = this.diffuse_map;
	normal_map = this.normal_map;
	roughness_map = this.roughness_map;
	metalness_map = this.metalness_map;
	alpha_map = this.alpha_map;

	view_main();
});

// メイン処理
function view_main(){
	init();
	generateGround();
	setupGUI();
	animate();
}

// set up GUI
function setupGUI() {
	var gui = new GUI();

	// exposure
	gui.add( params, 'exposure', 0, 2 )
		.onChange( function () {
			renderer.toneMappingExposure = params.exposure; 
			render();
		} );

	// rotate sun
	gui.add( params, 'sunrotate', 0, 359 )
		.onChange( function () {
			directionalLight.position.x = 200 * Math.cos(params.sunrotate * Math.PI / 180);
			directionalLight.position.z = 200 * Math.sin(params.sunrotate * Math.PI / 180);
			render();
		} );

	// sun height
	gui.add( params, 'sunheight', 0, 1 )
		.onChange( function () {
			directionalLight.position.y = params.sunheight * 200 * 5;
			render();
		} );

	// show ground
	gui.add( params, 'ground', false )
		.onChange( function () {
			mesh_ground.visible = params.ground;
			render();
		} );

	// ssao
	if (useSSAO) {
		gui.add( ssaoPass, 'output', {
			'Default': SSAOPass.OUTPUT.Default,
			'SSAO Only': SSAOPass.OUTPUT.SSAO,
			'SSAO Only + Blur': SSAOPass.OUTPUT.Blur,
			'Beauty': SSAOPass.OUTPUT.Beauty,
			'Depth': SSAOPass.OUTPUT.Depth,
			'Normal': SSAOPass.OUTPUT.Normal
		} ).onChange( function ( value ) {
			ssaoPass.output = parseInt( value );
		} );
		gui.add( ssaoPass, 'kernelRadius' ).min( 0 ).max( 32 );
		gui.add( ssaoPass, 'minDistance' ).min( 0.001 ).max( 0.02 );
		gui.add( ssaoPass, 'maxDistance' ).min( 0.01 ).max( 0.3 );
	}
	// ssao

	gui.open();
	gui.domElement.style.webkitUserSelect = 'none';
}

// create ground.
function generateGround() {
	var width = 100;
	var height = 100;
	mesh_ground =  new THREE.Mesh(
					new THREE.PlaneGeometry(width, height, 1, 1),
					new THREE.MeshLambertMaterial({ 
					color: 0x999999
					})); 
	mesh_ground.rotation.x = -Math.PI/2
	mesh_ground.receiveShadow = true;
	mesh_ground.visible = params.ground;
	scene.add(mesh_ground);
}

// 
function init() {
	container = document.createElement( 'div' );
	document.body.appendChild( container );

	// create scene.
	scene = new THREE.Scene();

	// set up camera
	camera = new THREE.PerspectiveCamera( 60, window.innerWidth / window.innerHeight, 0.01, 10000 );
	camera.position.z = 100;

	// set up renderer
	renderer = new THREE.WebGLRenderer( { antialias: true } );
	renderer.setPixelRatio( window.devicePixelRatio );
	renderer.setSize( window.innerWidth, window.innerHeight );
	
	// setup ssao
	composer = new EffectComposer( renderer );

	if (useSSAO) {
		ssaoPass = new SSAOPass( scene, camera, window.innerWidth, window.innerHeight );
		ssaoPass.kernelRadius = 16;
		ssaoPass.minDistance = 0.001; // これいるのか？
		ssaoPass.maxDistance = 1; // これいるのか？
		//ssaoPass.kernelSize = 1;
		//ssaoPass.output = 2;
		composer.addPass( ssaoPass );
		composer.setSize( window.innerWidth, window.innerHeight );//ssao
	}
	// ssao <--

	container.appendChild( renderer.domElement );
	
	// set up orbit manipulator.
	controls = new OrbitControls( camera, renderer.domElement );

	controls.enableDamping = true; // an animation loop is required when either damping or auto-rotation are enabled
	controls.dampingFactor = 0.05;

	controls.screenSpacePanning = false;

	controls.minDistance = 1;
	controls.maxDistance = 1000;

	renderer.outputEncoding = THREE.sRGBEncoding;
	renderer.toneMapping = THREE.ReinhardToneMapping;
	renderer.toneMappingExposure = 1;
	renderer.shadowMap.enabled = true;
	//renderer.shadowMapEnabled = true;

	// background color
	scene.background = new THREE.Color( 0xcce0ff );

	// set light
	var hemiLight = new THREE.HemisphereLight( 0xffffff, 0x444444 );
	hemiLight.position.set( 0, 200, 0 );
	scene.add( hemiLight );

	// set sun
	directionalLight = new THREE.DirectionalLight( 0xffffff, 8 );
	directionalLight.position.set( -200, 200, 0 );
	directionalLight.castShadow = true;
	//directionalLight.shadow.mapSize.width  = 1024;
	//directionalLight.shadow.mapSize.height = 1024;
	scene.add( directionalLight );

	//var str_file = name.split(/\.(?=[^.]+$)/)[0];
	//var load_path = './' + dir + '/';
	//var file_path = load_path + str_file;
	new FBXLoader()
		.setPath('./' + dir + '/')
		.load(name, function ( group ) {
			var loader = new THREE.TextureLoader()
				.setPath('./' + dir + '/');

			var diffuseMap = null;
			var normalMap = null;
			var roughnessMap = null;
			var metalnessMap = null;
			var alphaMap = null;
			if (diffuse_map.length > 0){
				diffuseMap = loader.load(diffuse_map);
			}
			if (normal_map.length > 0){
				normalMap = loader.load(normal_map);
			}
			if (roughness_map.length > 0){
				roughnessMap = loader.load(roughness_map);
			}
			if (metalness_map.length > 0){
				metalnessMap = loader.load(metalness_map);
			}
			if (alpha_map.length > 0){
				alphaMap = loader.load(alpha_map);
			}
			
			var material = new THREE.MeshStandardMaterial();

			if (diffuseMap != null) { 
				diffuseMap.encoding = THREE.sRGBEncoding;
				material.map = diffuseMap; 
			}
			if (normalMap != null) { 
				material.normalMap = normalMap; 
			}
			if (roughnessMap != null) { 
				roughnessMap.encoding = THREE.sRGBEncoding;
				material.roughnessMap = roughnessMap; 
				material.roughnessMap.wrapS = THREE.RepeatWrapping; 
				material.roughnessMap.wrapT = THREE.RepeatWrapping; 
			} else {
				material.roughness = 0.5;
			}
			if (alphaMap != null) { 
				material.alphaMap = alphaMap; 
				material.transparent = true;
			}
			if (metalnessMap != null) { 
				metalnessMap.encoding = THREE.sRGBEncoding;
				material.metalnessMap = metalnessMap; 
			}

			group.traverse( function ( child ) {
				if (child.isMesh) {
					child.material = material;
					child.castShadow = true;
					child.receiveShadow = true;
				}
			} );

			group.scale.set(scale, scale, scale);
			bbox = new THREE.Box3().setFromObject(group);
			scene.add( group );

			group_root = group;
			inited = true;

			// 初期視線の決定
			//camera.position.z = bbox.max.z * 5;
			const offset = 2;
			var size = (bbox.max.x > bbox.max.y) ? bbox.max.x : bbox.max.y;
			//var lookAtVector = new THREE.Vector3(0,0, -1);
			//lookAtVector.applyQuaternion(camera_ortho.quaternion);
			var lookAtVector = new THREE.Vector3(camera.matrix.elements[8], camera.matrix.elements[9], camera.matrix.elements[10]);
			camera.position.x = lookAtVector.x * size * offset;	// 近いと途切れるので遠くに移動
			camera.position.y = bbox.getCenter().y;//lookAtVector.y * size * offset;
			camera.position.z = lookAtVector.z * size * offset;
			controls.target = bbox.getCenter();

		} );

	new RGBELoader()
		.setDataType( THREE.UnsignedByteType )
		.setPath( './textures/equirectangular/' )
		//.load( 'equirectangular.hdr', function ( hdrEquirect ) {
		.load( 'grayback3.hdr', function ( hdrEquirect ) {

			var hdrCubeRenderTarget = pmremGenerator.fromEquirectangular( hdrEquirect );
			hdrEquirect.dispose();
			pmremGenerator.dispose();

			scene.background = hdrCubeRenderTarget.texture;
			scene.environment = hdrCubeRenderTarget.texture;
		} );

	var pmremGenerator = new THREE.PMREMGenerator( renderer );
	pmremGenerator.compileEquirectangularShader();

	if ( statsEnabled ) {
		stats = new Stats();
		container.appendChild( stats.dom );
	}

	window.addEventListener( 'resize', onWindowResize, false );
}

function onWindowResize() {
	renderer.setSize( window.innerWidth, window.innerHeight );
	composer.setSize( window.innerWidth, window.innerHeight );//ssao

	var new_aspect = window.innerWidth / window.innerHeight;
	var new_size = (bbox.max.x > bbox.max.y) ? bbox.max.x : bbox.max.y;				

	camera.aspect = new_aspect;
	camera.updateProjectionMatrix();
	// 平行投影カメラがある場合
	if (camera_ortho != null) {
		camera_ortho.left   = -new_size/2 * new_aspect;
		camera_ortho.right  =  new_size/2 * new_aspect;
		camera_ortho.top    =  new_size/2;
		camera_ortho.bottom = -new_size/2;
		camera_ortho.updateProjectionMatrix();
	}
}

	function animate() {
		requestAnimationFrame( animate );

		render();

		if ( statsEnabled ) stats.update();
	}

	function render() {
		if (controls.enabled) {	// 透視投影
			
			// 初期レンダーはどんどん大きくする
			if (inited && renderer.info.render.frame <= 60) {
				var s = renderer.info.render.frame / 60;	
				scene.scale.set(s, s, s);
			}


			controls.update();
			if (useSSAO) {
				composer.render();
			} else {
				renderer.render( scene, camera );
			}
		} else {	// 平行投影
			controls_ortho.update();
			renderer.render( scene, camera_ortho );
		}

		// auto rotation
		if (isAutoRot) {
			var timer = performance.now();
			group_root.rotation.y = timer *0.0002;
		}
		// gui control
		//mesh_ground.visible = params.ground;
		//renderer.toneMappingExposure = params.exposure;

		// 初期表示時に遠くから接近するアニメーション

	}

//--------------------------------------------
// ボタン
const camFrontRotButton = document.getElementById('camFrontRotButton');
const camBackRotButton = document.getElementById('camBackRotButton');
const camLeftRotButton = document.getElementById('camLeftRotButton');
const camRightRotButton = document.getElementById('camRightRotButton');
const camOrthoButton = document.getElementById('camOrthoButton');
const autoRotButton = document.getElementById('autoRotButton');
let camLeftRotButtonOn = false;
let camOrthoButtonOn = false;
//ボタンのイベント状態表示用
const buttonStateDisplay = document.getElementById("buttonStateDisplay");
buttonStateDisplay.style.display = "none";//release:true, debug:false. デバッグのときに解除してください
// イベントハンドラーの設定
setupEventHandlers();

function setupEventHandlers(){
    camLeftRotButton.addEventListener('pointerenter', () => {    //カーソルが「camLeftRot」ボタン上にあるとき
        buttonStateDisplay.innerHTML = "「camLeftRot」ボタン上にカーソルが来ました！";
    }, false);
	//camLeftRotButton.addEventListener('pointerdown', () => {	//「camLeftRot」ボタンが押されたとき
    //    buttonStateDisplay.innerHTML = "「camLeftRot」ボタンが押されました！　カメラを左回転させます！！";
    //    camLeftRotButtonOn = true;
    //}, false);
    //カーソルが「camLeftRot」ボタン上から離れたとき
    camLeftRotButton.addEventListener('pointerleave', () => {
        mouseLeaseFromButton();
        camLeftRotButtonOn = false;
    }, false);
    camLeftRotButton.addEventListener('pointerup', () => {    //「camLeftRot」ボタンが離されたとき
        buttonStateDisplay.innerHTML = "「camLeftRot」ボタンがリリースされました！";
        camLeftRotButtonOn = false;
		camera.position.x = -(bbox.max.x - bbox.min.x);
		camera.position.y = bbox.getCenter().y;
		camera.position.z = bbox.getCenter().z;
		controls.target = bbox.getCenter();
		if (camera_ortho != null) { 
			camera_ortho.position.x = -(bbox.max.x - bbox.min.x);
			camera_ortho.position.y = bbox.getCenter().y;
			camera_ortho.position.z = bbox.getCenter().z;
			controls_ortho.target = bbox.getCenter();
	    }
	}, false);
	camRightRotButton.addEventListener('pointerup', () => {    //「camLeftRot」ボタンが離されたとき
        buttonStateDisplay.innerHTML = "「camLeftRot」ボタンがリリースされました！";
		camera.position.x = (bbox.max.x - bbox.min.x);
		camera.position.y = bbox.getCenter().y;
		camera.position.z = bbox.getCenter().z;
		controls.target = bbox.getCenter();
		if (camera_ortho != null) { 
		camera_ortho.position.x = (bbox.max.x - bbox.min.x);
		camera_ortho.position.y = bbox.getCenter().y;
		camera_ortho.position.z = bbox.getCenter().z;
		controls_ortho.target = bbox.getCenter();
	}
    }, false);
	camFrontRotButton.addEventListener('pointerup', () => {    //「camLeftRot」ボタンが離されたとき
        buttonStateDisplay.innerHTML = "「camLeftRot」ボタンがリリースされました！";
		camera.position.x = bbox.getCenter().z;
		camera.position.y = bbox.getCenter().y;
		camera.position.z = (bbox.max.x - bbox.min.x);
		controls.target = bbox.getCenter();
		if (camera_ortho != null) { 
		camera_ortho.position.x = bbox.getCenter().x;
		camera_ortho.position.z = (bbox.max.y - bbox.min.y);
		controls_ortho.target = bbox.getCenter();
	}
    }, false);
	camBackRotButton.addEventListener('pointerup', () => {    //「camLeftRot」ボタンが離されたとき
        buttonStateDisplay.innerHTML = "「camLeftRot」ボタンがリリースされました！";
		camera.position.x = bbox.getCenter().z;
		camera.position.y = bbox.getCenter().y;
		camera.position.z = -(bbox.max.x - bbox.min.x);
		controls.target = bbox.getCenter();
		if (camera_ortho != null) { 
			camera_ortho.position.x = bbox.getCenter().x;
			camera_ortho.position.y = bbox.getCenter().y;
			camera_ortho.position.z = -(bbox.max.x - bbox.min.x);
			controls_ortho.target = bbox.getCenter();
	    }
	}, false);

	// 自動回転
	autoRotButton.addEventListener('pointerup', () => {    //「camLeftRot」ボタンが離されたとき
        buttonStateDisplay.innerHTML = "「AutoRotation」ボタンがリリースされました！";
		isAutoRot = !isAutoRot;
	}, false);


	// 透視投影／平行投影の切り替え
    camOrthoButton.addEventListener('pointerup', () => {
        buttonStateDisplay.innerHTML = "「camOrtho」ボタンがリリースされました！";
		camOrthoButtonOn = false;
		if (controls.enabled == true) {
			camOrthoButton.innerHTML = "camPerspective";

			const offset = 5;
			if (camera_ortho == null) {
				var size = (bbox.max.x > bbox.max.y) ? bbox.max.x : bbox.max.y;				
				var aspect = window.innerWidth / window.innerHeight;
				var width  = size * offset * aspect;
				var height = size * offset;
				camera_ortho = new THREE.OrthographicCamera(width/-2, width/2, height/2, height/-2, 1, 1000);
				camera_ortho.position.x = size * offset;	// 近いと途切れるので遠くに移動
				camera_ortho.position.z = size * offset;
			}
			// 視線方向を継承
			//var lookAtVector = new THREE.Vector3(0,0, -1);
			//lookAtVector.applyQuaternion(camera.quaternion);
			var lookAtVector = new THREE.Vector3(camera.matrix.elements[8], camera.matrix.elements[9], camera.matrix.elements[10]);
			camera_ortho.position.x = lookAtVector.x * size * offset;	// 近いと途切れるので遠くに移動
			camera_ortho.position.y = lookAtVector.y * size * offset;
			camera_ortho.position.z = lookAtVector.z * size * offset;

			if (controls_ortho == null) {
				controls_ortho = new OrbitControls( camera_ortho, renderer.domElement );
				controls_ortho.enableDamping = true; // an animation loop is required when either damping or auto-rotation are enabled
				controls_ortho.dampingFactor = 0.05;
				controls_ortho.screenSpacePanning = false;
				controls_ortho.minDistance = 1;
				controls_ortho.maxDistance = 1000;
			}

			controls_ortho.enabled = true;
			controls.enabled = false;
		} else {
			camOrthoButton.innerHTML = "camOrtho";

			// 視線方向を継承
			const offset = 5;
			var size = (bbox.max.x > bbox.max.y) ? bbox.max.x : bbox.max.y;
			//var lookAtVector = new THREE.Vector3(0,0, -1);
			//lookAtVector.applyQuaternion(camera_ortho.quaternion);
			var lookAtVector = new THREE.Vector3(camera_ortho.matrix.elements[8], camera_ortho.matrix.elements[9], camera_ortho.matrix.elements[10]);
			camera.position.x = lookAtVector.x * size * offset;	// 近いと途切れるので遠くに移動
			camera.position.y = lookAtVector.y * size * offset;
			camera.position.z = lookAtVector.z * size * offset;

			controls_ortho.enabled = false;
			controls.enabled = true;
		}

    }, false);

}
