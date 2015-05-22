@pushd %~dp0%

call Cleanup.bat
call Build.Win8.Xaml.bat
call Build.Win8.JS.bat
@rem call Build.Win81.Xaml.bat
@rem call Build.Win81.JS.bat
call Build.WP8.SL.bat

call Package.Win80.bat
@rem call Package.Win81.bat
call Package.WP80.bat

copy Win80.extension.vsixmanifest Microsoft.PlayerFramework\extension.vsixmanifest

call Package.Final.bat
call Cleanup.bat

@popd

@pause