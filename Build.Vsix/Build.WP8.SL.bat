@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\VSWinExpress.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

"%DEVENV%" /build "Release|AnyCPU" Microsoft.PlayerFramework.WP8.SL.sln

@popd

@pushd %~dp0%

cd WP80

mkdir "Microsoft.PlayerFramework.Core\References"
mkdir "Microsoft.PlayerFramework.Core\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Core\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Adaptive\References"
mkdir "Microsoft.PlayerFramework.Adaptive\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Adaptive\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Advertising\References"
mkdir "Microsoft.PlayerFramework.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics\References"
mkdir "Microsoft.PlayerFramework.Analytics\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.TimedText\References"
mkdir "Microsoft.PlayerFramework.TimedText\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.TimedText\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.TTML.Settings\References"
mkdir "Microsoft.PlayerFramework.TTML.Settings\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.TTML.Settings\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WebVTT\References"
mkdir "Microsoft.PlayerFramework.WebVTT\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WebVTT\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WebVTT.Settings\References"
mkdir "Microsoft.PlayerFramework.WebVTT.Settings\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WebVTT.Settings\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.CaptionSettings\References"
mkdir "Microsoft.PlayerFramework.CaptionSettings\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.CaptionSettings\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Dash\References"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration\neutral"
mkdir "Microsoft.Media.AudienceInsight\References"
mkdir "Microsoft.Media.AudienceInsight\References\CommonConfiguration"
mkdir "Microsoft.Media.AudienceInsight\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\neutral"

copy ..\..\WP8.SL.Core\bin\Release\Microsoft.PlayerFramework.dll											Microsoft.PlayerFramework.Core\References\CommonConfiguration\neutral\
copy ..\..\WP8.SL.Core\bin\Release\Microsoft.PlayerFramework.xml											Microsoft.PlayerFramework.Core\References\CommonConfiguration\neutral\

copy ..\..\WP8.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.dll								Microsoft.PlayerFramework.Adaptive\References\CommonConfiguration\neutral\
copy ..\..\WP8.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.xml								Microsoft.PlayerFramework.Adaptive\References\CommonConfiguration\neutral\

copy ..\..\WP8.AdaptiveStreaming.Helper\bin\Release\Microsoft.Media.AdaptiveStreaming.Helper.dll			Microsoft.PlayerFramework.Adaptive\References\CommonConfiguration\neutral\

copy ..\..\WP8.SL.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll								Microsoft.PlayerFramework.TimedText\References\CommonConfiguration\neutral\
copy ..\..\WP8.TimedText\bin\Release\Microsoft.Media.TimedText.dll											Microsoft.PlayerFramework.TimedText\References\CommonConfiguration\neutral\

copy ..\..\WP8.SL.TTML.Settings\Bin\Release\Microsoft.PlayerFramework.TTML.Settings.dll						Microsoft.PlayerFramework.TTML.Settings\References\CommonConfiguration\neutral\
copy ..\..\WP8.SL.TTML.Settings\Bin\Release\Microsoft.PlayerFramework.TTML.Settings.xml						Microsoft.PlayerFramework.TTML.Settings\References\CommonConfiguration\neutral\

copy ..\..\WP8.SL.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll							Microsoft.PlayerFramework.Advertising\References\CommonConfiguration\neutral\
copy ..\..\WP8.Advertising\bin\Release\Microsoft.Media.Advertising.dll										Microsoft.PlayerFramework.Advertising\References\CommonConfiguration\neutral\

copy ..\..\WP8.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll								Microsoft.PlayerFramework.Analytics\References\CommonConfiguration\neutral\
copy ..\..\WP8.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml								Microsoft.PlayerFramework.Analytics\References\CommonConfiguration\neutral\
copy ..\..\WP8.AdaptiveStreaming.Analytics\bin\Release\Microsoft.Media.AdaptiveStreaming.Analytics.dll		Microsoft.PlayerFramework.Analytics\References\CommonConfiguration\neutral\
copy ..\..\WP8.Analytics\bin\Release\Microsoft.Media.Analytics.dll											Microsoft.PlayerFramework.Analytics\References\CommonConfiguration\neutral\
copy ..\..\WP8.Analytics\bin\Release\Microsoft.Media.Analytics.xml											Microsoft.PlayerFramework.Analytics\References\CommonConfiguration\neutral\
copy ..\..\WP8.AudienceInsight\bin\Release\Microsoft.Media.AudienceInsight.dll								Microsoft.Media.AudienceInsight\References\CommonConfiguration\neutral\

copy ..\..\WP8.SL.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll									Microsoft.PlayerFramework.WebVTT\References\CommonConfiguration\neutral\
copy ..\..\WP8.WebVTT\bin\Release\Microsoft.Media.WebVTT.dll												Microsoft.PlayerFramework.WebVTT\References\CommonConfiguration\neutral\

copy ..\..\WP8.SL.WebVTT.Settings\Bin\Release\Microsoft.PlayerFramework.WebVTT.Settings.dll					Microsoft.PlayerFramework.WebVTT.Settings\References\CommonConfiguration\neutral\
copy ..\..\WP8.SL.WebVTT.Settings\Bin\Release\Microsoft.PlayerFramework.WebVTT.Settings.xml					Microsoft.PlayerFramework.WebVTT.Settings\References\CommonConfiguration\neutral\

copy ..\..\WP8.AdaptiveStreaming.Dash\bin\Release\Microsoft.Media.AdaptiveStreaming.Dash.dll				Microsoft.PlayerFramework.Dash\References\CommonConfiguration\neutral\
copy ..\..\Lib\Portable\Microsoft.Media.ISO\bin\Release\Microsoft.Media.ISO.dll								Microsoft.PlayerFramework.Dash\References\CommonConfiguration\neutral\

copy ..\..\WP8.Analytics.Advertising\bin\Release\Microsoft.Media.Analytics.Advertising.dll					Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\neutral\
copy ..\..\WP8.Analytics.AudienceInsight\bin\Release\Microsoft.Media.Analytics.AudienceInsight.dll			Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\neutral\

copy ..\..\WP8.SL.CaptionSettings\Bin\Release\Microsoft.PlayerFramework.CaptionSettings.dll					Microsoft.PlayerFramework.CaptionSettings\References\CommonConfiguration\neutral\
copy ..\..\WP8.SL.CaptionSettings\Bin\Release\Microsoft.PlayerFramework.CaptionSettings.xml					Microsoft.PlayerFramework.CaptionSettings\References\CommonConfiguration\neutral\

@popd

@echo.
@echo Done.
@echo.