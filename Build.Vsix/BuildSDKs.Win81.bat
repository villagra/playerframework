@pushd %~dp0%

call Cleanup.bat
call Build.Win8.Xaml.bat
call Build.Win8.JS.bat
call Build.Win81.Xaml.bat
call Build.Win81.JS.bat
call Build.WP8.bat

call Package.Win80.bat
call Package.Win81.bat
call Package.WP8.bat

copy Win81.extension.vsixmanifest Microsoft.PlayerFramework\extension.vsixmanifest

call Package.Final.bat
call Cleanup.bat

@popd

@pause