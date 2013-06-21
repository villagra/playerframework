@pushd %~dp0%

cd..

@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

@rem "%DEVENV%" /build "Release|AnyCPU" Microsoft.Win8.PlayerFramework.Js.sln
"%DEVENV%" /build "Release|x86"    Microsoft.Win8.PlayerFramework.Js.sln
"%DEVENV%" /build "Release|x64"    Microsoft.Win8.PlayerFramework.Js.sln
"%DEVENV%" /build "Release|ARM"    Microsoft.Win8.PlayerFramework.Js.sln

@rem "%DEVENV%" /build "Release|AnyCPU" Microsoft.Win8.PlayerFramework.Xaml.sln
"%DEVENV%" /build "Release|x86"    Microsoft.Win8.PlayerFramework.Xaml.sln
"%DEVENV%" /build "Release|x64"    Microsoft.Win8.PlayerFramework.Xaml.sln
"%DEVENV%" /build "Release|ARM"    Microsoft.Win8.PlayerFramework.Xaml.sln

"%DEVENV%" /build "Release|AnyCPU" Microsoft.WP8.PlayerFramework.SL.sln

@popd

@echo.
@echo Done.
@echo.
@pause
