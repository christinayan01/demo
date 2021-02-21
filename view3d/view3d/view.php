<!DOCTYPE html>
<html lang="jp">
<head>
	<title>Viewing 3D plugin by christinayan</title>
	<meta charset="utf-8">
	<meta name="viewport" content="width=device-width, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0">
	<link type="text/css" rel="stylesheet" href="./main.css">
</head>
<body>

<!--ボタン類-->
<section class="section">
<div class="container">
  <div class="buttons is-centered">
		<div class="button is-large" id="camFrontRotButton">
		↑
		</div>
		<div class="button is-large" id="camBackRotButton">
		↓
		</div>
		<div class="button is-large" id="camLeftRotButton">
		←
		</div>
		<div class="button is-large" id="camRightRotButton">
		→
		</div>
	</div>
	<div class="buttons is-centered">
		<div class="button is-large" id="camOrthoButton">
		Ortho
		</div>
  </div>
	<div class="buttons is-centered">
		<div class="button is-large" id="autoRotButton">
		AutoRotation
		</div>
  </div>
</div>
</section>

<!--<canvas id="view_canvas"></canvas>-->

<?php
// GETパラメータを得る
function getGet($key) {
	$str = '';
	if (isset($_GET[$key])) {
		$str = $_GET[$key];
	}
	return $str;
}

// ファイル存在をPHPで確認しておく
function getTextureMap($type) {
	$dir = './' . getGet('dir') . '/';
	$name = getGet('name');
	$name = pathinfo($name, PATHINFO_FILENAME);

	$diffuseMap = '';
	if ( file_exists($dir . $name . '_' . $type . '.png') ) {
		$diffuseMap   = $name . '_' . $type . '.png';
	} else if ( file_exists($dir . $name . '_' . $type . '.jpg') ) {
		$diffuseMap   = $name . '_' . $type . '.jpg';
	}
	return $diffuseMap;
}
?>

<script>
//window.addEventListener('load', function(){
  	var dir = '<?php echo getGet('dir'); ?>';
  	var name = '<?php echo getGet('name'); ?>';
  	var scale = Number('<?php echo getGet('scale'); ?>');
	var canvas_id = '#view_canvas';

	var diffuse_map = '<?php echo getTextureMap('d'); ?>';
	var normal_map = '<?php echo getTextureMap('n'); ?>';
	var roughness_map = '<?php echo getTextureMap('r'); ?>';
	var metalness_map = '<?php echo getTextureMap('m'); ?>';
	var alpha_map = '<?php echo getTextureMap('a'); ?>';
	//view_show();
</script>
<script type="module" src="./view.js"></script>

	<!-- ボタン状態表示 -->
<footer>
	<div class="container-footer">
		<div class="notification is-primary">
			<strong><p id="buttonStateDisplay">まだどのボタンも選択されていません</p></strong>
	  </div>
	</div>
</footer>


</body>
</html>
