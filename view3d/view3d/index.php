<?php

/*
Plugin Name: Yanai 3D Viewing 
Plugin URI: https://christinayan01.jp/architecture/plugin
Description: 3D viewing plugin using Three.js.
Author: Takahiro Yanai
Version: 0.9.1
Author URI: https://christinayan01.jp/architecture/
*/

function view3d($atts)
{
	extract(shortcode_atts(array(
		'path' => '',
		'scale' => 1.0,
	), $atts));

	if (strlen($path) == 0) {
		return 'error: unknown file path.';
	}

	// HTTP or HTTPS
	$str_http = 'http://';
	if (isset($_SERVER['HTTPS'])) {
	    $str_http = "https://";
	}
	// base URL
	$url_dir = $str_http . $_SERVER['HTTP_HOST'] . dirname($_SERVER["SCRIPT_NAME"]) . '/';

	// output string.
	$out = '';

	$arg_dir = pathinfo($path, PATHINFO_DIRNAME);
	$arg_name = pathinfo($path, PATHINFO_BASENAME);
	$arg_scale = strval($scale);

// debug -->
/*
$out .= '[DEBUG]dir = ';
$out .= pathinfo($path, PATHINFO_DIRNAME) . '/';
$out .= '<br>';
$out .= '[DEBUG]file = ';
$out .= pathinfo($path, PATHINFO_BASENAME);
$out .= '<br>';
*/
// debug <--

	pathinfo($file, PATHINFO_FILENAME);

	// Plugin directory
	$plugin_dir = 'wp-content/plugins/view3d/';

// debug -->
/*
$out .= '[DEBUG]view URL = ';
$out .= $url_dir . $plugin_dir;
$out .= '<br>';
*/
// debug <--

	$out .= '<br><iframe src="';
	$out .= $url_dir . $plugin_dir . 'view.php?dir=';
	$out .= $arg_dir;
    $out .= '&name=';
	$out .= $arg_name;
    $out .= '&scale=';
	$out .= $arg_scale;
	$out .= '" width="100%" height="500px"></iframe>';

// debug -->
/*
$out .= '[DEBUG]final URL = ';
$out .= $url_dir . $plugin_dir . 'view.php?dir=';
$out .= $arg_dir;
$out .= '&name=';
$out .= $arg_name;
$out .= '&scale=';
$out .= $arg_scale;
$out .= '<br>';
*/
// debug <--

	return $out;
}

add_shortcode('view3d', 'view3d');
