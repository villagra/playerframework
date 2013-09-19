@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\VSWinExpress.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

"%DEVENV%" /rebuild "Release|AnyCPU" Microsoft.WP8.PlayerFramework.SL.sln

@popd

@pushd %~dp0%

mkdir "Microsoft.PlayerFramework.WP8\References"
mkdir "Microsoft.PlayerFramework.WP8\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.Adaptive\References"
mkdir "Microsoft.PlayerFramework.WP8.Adaptive\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.Adaptive\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.Advertising\References"
mkdir "Microsoft.PlayerFramework.WP8.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.Analytics\References"
mkdir "Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.TimedText\References"
mkdir "Microsoft.PlayerFramework.WP8.TimedText\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.TimedText\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.WebVTT\References"
mkdir "Microsoft.PlayerFramework.WP8.WebVTT\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.WebVTT\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.Dash\References"
mkdir "Microsoft.PlayerFramework.WP8.Dash\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.Dash\References\CommonConfiguration\neutral"

copy ..\Phone.SL\bin\Release\Microsoft.PlayerFramework.dll  Microsoft.PlayerFramework.WP8\References\CommonConfiguration\neutral\
copy ..\Phone.SL\bin\Release\Microsoft.PlayerFramework.xml  Microsoft.PlayerFramework.WP8\References\CommonConfiguration\neutral\

copy ..\Phone.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.dll  Microsoft.PlayerFramework.WP8.Adaptive\References\CommonConfiguration\neutral\
copy ..\Phone.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.xml  Microsoft.PlayerFramework.WP8.Adaptive\References\CommonConfiguration\neutral\

copy ..\Phone.SL.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll  Microsoft.PlayerFramework.WP8.TimedText\References\CommonConfiguration\neutral\
copy ..\Phone.TimedText\bin\Release\Microsoft.TimedText.dll						Microsoft.PlayerFramework.WP8.TimedText\References\CommonConfiguration\neutral\

copy ..\Phone.SL.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll	Microsoft.PlayerFramework.WP8.Advertising\References\CommonConfiguration\neutral\
copy ..\Phone.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.dll			Microsoft.PlayerFramework.WP8.Advertising\References\CommonConfiguration\neutral\

copy ..\Phone.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll					    Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy ..\Phone.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml					    Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy ..\Phone.SL.Adaptive.Analytics\bin\Release\Microsoft.PlayerFramework.Adaptive.Analytics.dll	Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy ..\Phone.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.dll								Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy ..\Phone.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.xml								Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy ..\Lib\Portable\ZLib\bin\Release\ZLib.dll														Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\

copy ..\Phone.SL.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll	Microsoft.PlayerFramework.WP8.WebVTT\References\CommonConfiguration\neutral\
copy ..\Phone.WebVTT\bin\Release\Microsoft.WebVTT.dll						Microsoft.PlayerFramework.WP8.WebVTT\References\CommonConfiguration\neutral\

copy ..\Phone.SL.Adaptive.Dash\bin\Release\Microsoft.PlayerFramework.Adaptive.Dash.dll		Microsoft.PlayerFramework.WP8.Dash\References\CommonConfiguration\neutral\
copy ..\Lib\Portable\Microsoft.Media.ISO\bin\Release\Microsoft.Media.ISO.dll				Microsoft.PlayerFramework.WP8.Dash\References\CommonConfiguration\neutral\

@popd

@echo.
@echo Done.
@echo.