@set SN=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\x64\sn.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\VSWinExpress.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

"%DEVENV%" /build "Release|AnyCPU" Microsoft.PlayerFramework.Win8.Xaml.sln
"%DEVENV%" /build "Release|x86"    Microsoft.PlayerFramework.Win8.Xaml.sln
"%DEVENV%" /build "Release|x64"    Microsoft.PlayerFramework.Win8.Xaml.sln
"%DEVENV%" /build "Release|ARM"    Microsoft.PlayerFramework.Win8.Xaml.sln

"%SN%" -R Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 							Win8.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 							Win8.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 							Win8.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.winmd 											Win8.WinRT.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd 												Win8.WinRT.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 		Win8.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 		Win8.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 		Win8.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.winmd														Win8.WinRT.WebVTT\Microsoft.PlayerFramework.snk

@popd

@pushd %~dp0%

cd Win80

mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.Core\References"
mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework"
mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes"
mkdir "Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Helper"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Adaptive"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\References"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Advertising"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\themes"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Analytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Analytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\References"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TimedText"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.Media.TimedText"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.Media.TimedText\themes"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.TTML.Settings\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.TTML.Settings\References"
mkdir "Microsoft.PlayerFramework.Xaml.TTML.Settings\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.TTML.Settings\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.TTML.Settings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TTML.Settings"
mkdir "Microsoft.PlayerFramework.Xaml.TTML.Settings\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.TTML.Settings\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT\themes"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT.Settings\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT.Settings\References"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT.Settings\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT.Settings\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT.Settings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT.Settings"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT.Settings\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT.Settings\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\References"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Controls"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Themes"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration\neutral"

copy ..\..\Win8.Xaml.Core\bin\Release\Microsoft.PlayerFramework.dll										Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.Core\bin\Release\Microsoft.PlayerFramework.xml										Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.Core\bin\Release\Microsoft.PlayerFramework.pri										Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\
copy ..\..\Win8.Xaml.Core\bin\Release\Themes\generic.xaml												Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\Win8.Xaml.Core\bin\Release\Themes\EntertainmentTheme.xaml									Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\Win8.Xaml.Core\bin\Release\Themes\Classic.xaml												Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes

copy ..\..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.pri					Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Adaptive\
copy ..\..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.pri				Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Helper\
copy ..\..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy ..\..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy ..\..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 			Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy ..\..\Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
copy ..\..\Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
copy ..\..\Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 			Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
copy ..\..\Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
copy ..\..\Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
copy ..\..\Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 			Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\

copy ..\..\Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll						Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.xml						Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.pri						Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TimedText\
copy ..\..\Win8.Xaml.TimedText\bin\Release\Microsoft.Media.TimedText.dll								Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.TimedText\bin\Release\Microsoft.Media.TimedText.pri								Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.Media.TimedText\
copy ..\..\Win8.WinRT.TimedText\bin\Release\Themes\generic.xaml											Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.Media.TimedText\themes\

copy ..\..\Win8.Xaml.TTML.Settings\bin\Release\Microsoft.PlayerFramework.TTML.Settings.xml				Microsoft.PlayerFramework.Xaml.TTML.Settings\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.TTML.Settings\bin\Release\Microsoft.PlayerFramework.TTML.Settings.dll				Microsoft.PlayerFramework.Xaml.TTML.Settings\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.TTML.Settings\bin\Release\Microsoft.PlayerFramework.TTML.Settings.pri				Microsoft.PlayerFramework.Xaml.TTML.Settings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TTML.Settings\

copy ..\..\Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll					Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.xml					Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.pri					Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\
copy ..\..\Win8.Xaml.Advertising\bin\Release\Themes\generic.xaml										Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\themes\
copy ..\..\Win8.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.winmd 							Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.pri							Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Advertising\

copy ..\..\Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll						Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml						Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.pri						Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Analytics\
copy ..\..\Win8.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd 							Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Win8.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.pri								Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics\

copy ..\..\Win8.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.pri				Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Analytics\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x64\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM\

copy ..\..\Win8.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll							Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.pri							Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT\
copy ..\..\Win8.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.winmd									Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.pri										Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT\
copy ..\..\Win8.WinRT.WebVTT\bin\Release\Themes\generic.xaml											Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT\themes\

copy ..\..\Win8.Xaml.WebVTT.Settings\bin\Release\Microsoft.PlayerFramework.WebVTT.Settings.xml			Microsoft.PlayerFramework.Xaml.WebVTT.Settings\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.WebVTT.Settings\bin\Release\Microsoft.PlayerFramework.WebVTT.Settings.dll			Microsoft.PlayerFramework.Xaml.WebVTT.Settings\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.WebVTT.Settings\bin\Release\Microsoft.PlayerFramework.WebVTT.Settings.pri			Microsoft.PlayerFramework.Xaml.WebVTT.Settings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT.Settings\

copy ..\..\Win8.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.dll			Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.xml			Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration\neutral\
copy ..\..\Win8.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.pri			Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\
copy ..\..\Win8.Xaml.CaptionSettings\bin\Release\Controls\CaptionSettingsControl.xaml					Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Controls\
copy ..\..\Win8.Xaml.CaptionSettings\bin\Release\Themes\Generic.xaml									Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Themes\

@popd

@echo.
@echo Done.
@echo.
