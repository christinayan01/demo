# About
This repository contains demo projects pertaining to Reflect Publisher API.
Node : only Windows is supported right now.

## Contents

### Reflect Nugets
We provide our C# Nugets directly in this repository, in the `nugets` folder.

### PublisherSample
This is a small project that showcases how to use the Publisher API in about 100 lines of code.
This "tiny exporter" creates hardcoded models and sends them to Reflect, using the C# Reflect API, and can be launched either in GUI or in console mode.

### ConfigFileCreator
This is a tiny utilitary program that helps create a config file, which can be very useful for automated publishing.

## Publisher API overview
Take a look at the [Reflect documentation](https://docs.unity3d.com/reflect/manual/devguide/DevGuide.html) for a Publisher API overview.

## Advanced workflows

### Skip Reflect UI

The default plugin behavior is to display the Reflect UI, so that the user can log in and select a Unity project to export their model to. However, as a developer, you might be interested in automated testing, in which case the UI gets useless.

For this reason, the Reflect UI can be skipped and replaced with a predefined config file, by using a single environment variable.
You simply need to create a `REFLECT_UI_CONFIG_PATH` env variable, and set the absolute path to your config path as a value.

The `config.json.template` file, at the root of this repository, indicates what your config file should look like.
You can generate your own config file by running the ConfigFileCreator utilitary. Note that the user token that is set in the config value expires after 30 days.
