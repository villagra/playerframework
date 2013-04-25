@pushd %~dp0%

call Build.bat
call Sign.bat
call Deploy.Win8.Xaml.bat
call Deploy.Win8.JS.bat
call Deploy.WP8.bat
call Package.bat

@popd
