@pushd %~dp0%

cd..

@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

"%DEVENV%" /rebuild "Release|AnyCPU" Microsoft.WP8.PlayerFramework.SL.sln

copy Phone.SL\bin\Release\Microsoft.PlayerFramework.dll											Build.NuGet\Microsoft.PlayerFramework\lib\windowsphone8\
copy Phone.SL\bin\Release\Microsoft.PlayerFramework.xml											Build.NuGet\Microsoft.PlayerFramework\lib\windowsphone8\

copy Phone.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.dll						Build.NuGet\Microsoft.PlayerFramework.Adaptive\lib\windowsphone8\
copy Phone.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.xml						Build.NuGet\Microsoft.PlayerFramework.Adaptive\lib\windowsphone8\

copy Phone.SL.Adaptive.Dash\bin\Release\Microsoft.PlayerFramework.Adaptive.Dash.dll				Build.NuGet\Microsoft.PlayerFramework.Adaptive.Dash\lib\windowsphone8\

copy Phone.SL.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll						Build.NuGet\Microsoft.PlayerFramework.TimedText\lib\windowsphone8\
copy Phone.TimedText\bin\Release\Microsoft.TimedText.dll										Build.NuGet\Microsoft.PlayerFramework.TimedText\lib\windowsphone8\

copy Phone.SL.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll					Build.NuGet\Microsoft.PlayerFramework.Advertising\lib\windowsphone8\
copy Phone.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.dll							Build.NuGet\Microsoft.PlayerFramework.Advertising\lib\windowsphone8\

copy Phone.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll						Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\windowsphone8\
copy Phone.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml					    Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\windowsphone8\
copy Phone.SL.Adaptive.Analytics\bin\Release\Microsoft.PlayerFramework.Adaptive.Analytics.dll   Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\windowsphone8\
copy Phone.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.dll								Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\windowsphone8\
copy Phone.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.xml								Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\windowsphone8\
copy Lib\Portable\ZLib\bin\Release\ZLib.dll														Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\windowsphone8\

copy Phone.SL.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll							Build.NuGet\Microsoft.PlayerFramework.WebVTT\lib\windowsphone8\
copy Phone.WebVTT\bin\Release\Microsoft.WebVTT.dll												Build.NuGet\Microsoft.PlayerFramework.WebVTT\lib\windowsphone8\

@popd

@echo.
@echo Done.
@echo.
@pause
