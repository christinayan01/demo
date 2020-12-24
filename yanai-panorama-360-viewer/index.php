<?php

/*
Plugin Name: Yanai 360 Panorama Viewer
Plugin URI: https://christinayan01.jp/architecture/plugin
Description: Generate 360 Panorama view using equirectangular.
	url : (string) absolute path.
    width : (string) view width. include unit.
    height : (string)  view height. include unit.
Author: christinayan by Takahiro Yanai
Version: 1.0
Author URI: https://christinayan01.jp/architecture/
*/


function panorama360($atts)
{
	extract(shortcode_atts(array(
		'url' => '',
		'width' => '100%',
		'height' => '400px',
	), $atts));

	if (strlen($url) == 0) {
		$url = 'junction.jpg';
	}
	
    $out = '
	<!-- yanai panorama 360 viewer -->
	<style>
	#panorama360plugin {
		width:' . $width . ';
		height:' . $height . ';
	}
	</style>
    <script src="https://aframe.io/releases/1.1.0/aframe.min.js"></script>
	<div id="panorama360plugin">
	    <a-scene embedded>
    <a-sky src="' . $url . '"></a-sky>
	    </a-scene>
	</div>
	<!-- yanai panorama 360 viewer -->
    ';

	return $out;
}

add_shortcode('panorama360', 'panorama360');
