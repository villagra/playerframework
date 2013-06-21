@pushd %~dp0%

call Pave.bat
call Deploy.WP7.bat
call Deploy.WP8.bat
call Package.bat

@popd
