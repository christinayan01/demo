<!DOCTYPE html>
<html lang="jp">
<head>
	<title>Viewing 3D plugin by christinayan</title>
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0">
	<link type="text/css" rel="stylesheet" href="./main.css">
</head>
<body>

<?php
/*
print '<h1>debug</h1>';
	$dir = $_GET['dir'];
	$name = $_GET['name'];
	$str_file = pathinfo($name, PATHINFO_FILENAME);
	print 'str_file = ' . $str_file . '<br>';
	print "var diffuseMap = loader.load( '" . $str_file . "_d.png' );" . "<br>";
	print ".setPath( ./". $dir ."/' );";
*/
?>

<script type="module">
	var url = location.origin + location.pathname;
	var root_dir = url.substring(0, url.length - 8);

	import * as THREE from './threejs/three.module.js';
	import Stats from './threejs/jsm/libs/stats.module.js';

	import { OrbitControls } from './threejs/jsm/controls/OrbitControls.js';
	import { FBXLoader } from './threejs/jsm/loaders/FBXLoader.js';
	import { RGBELoader } from './threejs/jsm/loaders/RGBELoader.js';

	import { GUI } from './threejs/jsm/libs/dat.gui.module.js';

	var statsEnabled = false;
	var container, stats;
	var camera, scene, renderer, controls;

	var obj2 = null;

	var mesh_ground = null;

	var params = {
		exposure: 1.0,
		ground: false,
		sunrotate: 180,
		sunheight: 0.5
	};

	var directionalLight = null;

	init();
	ground();
	setupGUI();
	animate();

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

		gui.open();
		gui.domElement.style.webkitUserSelect = 'none';
	}

	// create ground.
	function ground() {
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

		renderer = new THREE.WebGLRenderer( { antialias: true } );
		renderer.setPixelRatio( window.devicePixelRatio );
		renderer.setSize( window.innerWidth, window.innerHeight );
		container.appendChild( renderer.domElement );

		renderer.outputEncoding = THREE.sRGBEncoding;
		renderer.toneMapping = THREE.ReinhardToneMapping;
		renderer.toneMappingExposure = 1;
		renderer.shadowMap.enabled = true;
		//renderer.shadowMapEnabled = true;

		// create scene.

		scene = new THREE.Scene();

		// set up camera

		camera = new THREE.PerspectiveCamera( 60, window.innerWidth / window.innerHeight, 0.01, 10000 );
		camera.position.z = 100;

		// set up orbit manipulator.
		controls = new OrbitControls( camera, renderer.domElement );

		controls.enableDamping = true; // an animation loop is required when either damping or auto-rotation are enabled
		controls.dampingFactor = 0.05;

		controls.screenSpacePanning = false;

		controls.minDistance = 1;
		controls.maxDistance = 1000;

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

		new FBXLoader()
<?php
$dir = $_GET['dir'];
$name = $_GET['name'];
$str_file = pathinfo($name, PATHINFO_FILENAME);
$load_path = './'. $dir .'/';

echo "			.setPath( '" . $load_path . "' )\r\n";
echo "			.load( '" . $name . "', function ( group ) {\r\n";

echo "				var loader = new THREE.TextureLoader()\r\n";
echo "					.setPath( './" . $dir . "/' );\r\n";

				if ( file_exists($load_path . $str_file . '_d.png') ) {
echo "				var diffuseMap   = loader.load( '" . $str_file . "_d.png' );\r\n";
				} else if ( file_exists($load_path . $str_file . '_d.jpg') ) {
echo "				var diffuseMap   = loader.load( '" . $str_file . "_d.jpg' );\r\n";
				} else {
echo "				var diffuseMap   = null;\r\n";
				}

				if ( file_exists($load_path . $str_file . '_n.png') ) {
echo "				var normalMap    = loader.load( '" . $str_file . "_n.png' );\r\n";
				} else if ( file_exists($load_path . $str_file . '_n.jpg') ) {
echo "				var normalMap    = loader.load( '" . $str_file . "_n.jpg' );\r\n";
				} else {
echo "				var normalMap    = null;\r\n";
				}

				if ( file_exists($load_path . $str_file . '_r.png') ) {
echo "				var roughnessMap = loader.load( '" . $str_file . "_r.png' );\r\n";
				} else if ( file_exists($load_path . $str_file . '_r.jpg') ) {
echo "				var roughnessMap = loader.load( '" . $str_file . "_r.jpg' );\r\n";
				} else {
echo "				var roughnessMap = null;\r\n";
				}

				if ( file_exists($load_path . $str_file . '_m.png') ) {
echo "				var metalnessMap = loader.load( '" . $str_file . "_m.png' );\r\n";
				} else 	if ( file_exists($load_path . $str_file . '_m.jpg') ) {
echo "				var metalnessMap = loader.load( '" . $str_file . "_m.jpg' );\r\n";
				} else {
echo "				var metalnessMap = null;\r\n";
				}

				if ( file_exists($load_path . $str_file . '_a.png') ) {
echo "				var alphaMap     = loader.load( '" . $str_file . "_a.png' );\r\n";
				} else if ( file_exists($load_path . $str_file . '_a.jpg') ) {
echo "				var alphaMap     = loader.load( '" . $str_file . "_a.jpg' );\r\n";
				} else {
echo "				var alphaMap     = null;\r\n";
				}
?>

				var material = new THREE.MeshStandardMaterial();

				if (diffuseMap != null) { 
					diffuseMap.encoding   = THREE.sRGBEncoding;
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

<?php
$scale = $_GET['scale'];
echo '				var scale = ' . (string)$scale . ';';
?>

				group.scale.set(scale, scale, scale);
				scene.add( group );
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

		camera.aspect = window.innerWidth / window.innerHeight;
		camera.updateProjectionMatrix();

	}

	function animate() {
		requestAnimationFrame( animate );

		render();

		if ( statsEnabled ) stats.update();
	}

	function render() {
		controls.update();
		renderer.render( scene, camera );

		// gui control
		//mesh_ground.visible = params.ground;
		//renderer.toneMappingExposure = params.exposure;
	}

</script>

</body>
</html>
