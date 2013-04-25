HTML5 Player Framework 1.1
==========================

Additional information about this release is available here:
http://playerframework.codeplex.com/releases/view/86402

Server Configuration
====================

A web.config file is included with the examples that enables Gzip compression 
and adds the necessary video mime types for ASP.NET applications and IIS 7.

Additional server configurations and recommendations are available here:
https://github.com/h5bp/server-configs#readme

Custom Builds
=============

To generate a custom build incorporating the latest changes or additions to 
the src folder, execute the batch file located in the build folder.

a. For Debug builds, execute:
build-player-framework.bat Debug

This command will concatenate the src files into a single css and js file.

b. For Release builds, execute:
build-player-framework.bat Release

This command will minify the src files into a single css and js file.

In Visual Studio, you can execute this automatically as a post-build event.

For example:
call "$(ProjectDir)build\build-player-framework.bat" "$(ConfigurationName)"

* Please note curl.exe (http://curl.haxx.se/) is required for release builds.