@pushd %~dp0%

call Cleanup.bat
call Build.Win10.Xaml.bat
call Build.Win10.JS.bat
call Build.Win81.Xaml.bat
call Build.Win81.JS.bat
call Build.WP81.Xaml.bat
call Build.WP81.JS.bat
call Build.WP8.SL.bat
call Build.Win8.Xaml.bat
call Build.Win8.JS.bat

call Package.Win10.bat
call Package.Win81.bat
call Package.WP81.bat
call Package.Win80.bat
call Package.WP80.bat

copy Win10.extension.vsixmanifest Microsoft.PlayerFramework\extension.vsixmanifest

call Package.Final.bat
call Cleanup.bat

@popd

@pause