@set SN=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\x64\sn.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

"%DEVENV%" /rebuild "Release|AnyCPU" Microsoft.PlayerFramework.Universal.Xaml.sln
"%DEVENV%" /rebuild "Release|x86"    Microsoft.PlayerFramework.Universal.Xaml.sln
"%DEVENV%" /rebuild "Release|ARM"    Microsoft.PlayerFramework.Universal.Xaml.sln

"%SN%" -R WP81.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 							WP81.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R WP81.Xaml.Adaptive\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 							WP81.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.winmd 										Universal.WinRT.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd 										Universal.WinRT.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R WP81.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 	WP81.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R WP81.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 	WP81.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.winmd												Universal.WinRT.WebVTT\Microsoft.PlayerFramework.snk
"%SN%" -R WP81.WinRT.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd				WP81.WinRT.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R WP81.WinRT.AdaptiveStreaming.Dash\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd				WP81.WinRT.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.WinRT.Analytics.Advertising\bin\x86\Release\Microsoft.Media.Analytics.Advertising.winmd				Universal.WinRT.Analytics.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.WinRT.Analytics.Advertising\bin\ARM\Release\Microsoft.Media.Analytics.Advertising.winmd				Universal.WinRT.Analytics.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.WinRT.Analytics.AudienceInsight\bin\x86\Release\Microsoft.Media.Analytics.AudienceInsight.winmd		Universal.WinRT.Analytics.AudienceInsight\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.WinRT.Analytics.AudienceInsight\bin\ARM\Release\Microsoft.Media.Analytics.AudienceInsight.winmd		Universal.WinRT.Analytics.AudienceInsight\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.WinRT.AudienceInsight\bin\Release\Microsoft.Media.AudienceInsight.winmd								Universal.WinRT.AudienceInsight\Microsoft.PlayerFramework.snk

@popd

@pushd %~dp0%

cd WP81

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
mkdir "Microsoft.PlayerFramework.Dash\Redist"
mkdir "Microsoft.PlayerFramework.Dash\References"
mkdir "Microsoft.PlayerFramework.Dash\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Dash\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Dash\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Dash"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\Redist"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics.Advertising"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics.AudienceInsight"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\x86"
mkdir "Microsoft.Media.AudienceInsight\Redist"
mkdir "Microsoft.Media.AudienceInsight\References"
mkdir "Microsoft.Media.AudienceInsight\Redist\CommonConfiguration"
mkdir "Microsoft.Media.AudienceInsight\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.Media.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.Media.AudienceInsight"
mkdir "Microsoft.Media.AudienceInsight\References\CommonConfiguration"
mkdir "Microsoft.Media.AudienceInsight\References\CommonConfiguration\neutral"

copy ..\..\Universal.Xaml.Core\bin\Release\Microsoft.PlayerFramework.dll														Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.Core\bin\Release\Microsoft.PlayerFramework.xml														Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.Core\bin\Release\Microsoft.PlayerFramework.pri														Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\
copy ..\..\Universal.Xaml.Core\bin\Release\Themes\Generic.xbf																	Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\Universal.Xaml.Core\bin\Release\Themes\Entertainment.xbf																Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\Universal.Xaml.Core\bin\Release\Themes\Phone.xbf																		Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\Universal.Xaml.Core\bin\Release\Themes\Classic.xbf																	Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes

copy ..\..\WP81.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.pri											Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Adaptive\
copy ..\..\WP81.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.pri										Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Helper\
copy ..\..\WP81.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.dll											Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy ..\..\WP81.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.xml											Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy ..\..\WP81.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 									Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy ..\..\WP81.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.dll											Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
copy ..\..\WP81.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.xml											Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
copy ..\..\WP81.Xaml.Adaptive\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 									Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\

copy ..\..\Universal.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll											Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.xml											Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.pri											Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TimedText\
copy ..\..\Universal.Xaml.TimedText\bin\Release\Microsoft.Media.TimedText.dll													Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.TimedText\bin\Release\Microsoft.Media.TimedText.pri													Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.Media.TimedText\
copy ..\..\Universal.WinRT.TimedText\bin\Release\Themes\generic.xbf																Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.Media.TimedText\themes\

copy ..\..\WP81.Xaml.TTML.Settings\bin\Release\Microsoft.PlayerFramework.TTML.Settings.xml										Microsoft.PlayerFramework.Xaml.TTML.Settings\References\CommonConfiguration\neutral\
copy ..\..\WP81.Xaml.TTML.Settings\bin\Release\Microsoft.PlayerFramework.TTML.Settings.dll										Microsoft.PlayerFramework.Xaml.TTML.Settings\References\CommonConfiguration\neutral\
copy ..\..\WP81.Xaml.TTML.Settings\bin\Release\Microsoft.PlayerFramework.TTML.Settings.pri										Microsoft.PlayerFramework.Xaml.TTML.Settings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TTML.Settings\

copy ..\..\Universal.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll										Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.xml										Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.pri										Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\
copy ..\..\Universal.Xaml.Advertising\bin\Release\Themes\generic.xbf															Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\themes\
copy ..\..\Universal.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.winmd 											Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.pri												Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Advertising\

copy ..\..\Universal.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll											Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml											Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.pri											Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Analytics\
copy ..\..\Universal.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd 												Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Universal.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.pri													Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics\

copy ..\..\WP81.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.pri				Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Analytics\
copy ..\..\WP81.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86\
copy ..\..\WP81.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM\

copy ..\..\Universal.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll												Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.pri												Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT\
copy ..\..\Universal.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.winmd														Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\..\Universal.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.pri															Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT\
copy ..\..\Universal.WinRT.WebVTT\bin\Release\Themes\generic.xbf																Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT\themes\

copy ..\..\WP81.Xaml.WebVTT.Settings\bin\Release\Microsoft.PlayerFramework.WebVTT.Settings.xml									Microsoft.PlayerFramework.Xaml.WebVTT.Settings\References\CommonConfiguration\neutral\
copy ..\..\WP81.Xaml.WebVTT.Settings\bin\Release\Microsoft.PlayerFramework.WebVTT.Settings.dll									Microsoft.PlayerFramework.Xaml.WebVTT.Settings\References\CommonConfiguration\neutral\
copy ..\..\WP81.Xaml.WebVTT.Settings\bin\Release\Microsoft.PlayerFramework.WebVTT.Settings.pri									Microsoft.PlayerFramework.Xaml.WebVTT.Settings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT.Settings\

copy ..\..\WP81.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.dll									Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration\neutral\
copy ..\..\WP81.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.xml									Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration\neutral\
copy ..\..\WP81.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.pri									Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\
copy ..\..\WP81.Xaml.CaptionSettings\bin\Release\CaptionSettingFlyout.xbf														Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\
copy ..\..\WP81.Xaml.CaptionSettings\bin\Release\Themes\Generic.xbf																Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Themes\

copy ..\..\WP81.WinRT.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Dash.pri							Microsoft.PlayerFramework.Dash\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Dash\
copy ..\..\WP81.WinRT.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd						Microsoft.PlayerFramework.Dash\References\CommonConfiguration\x86\
copy ..\..\WP81.WinRT.AdaptiveStreaming.Dash\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd						Microsoft.PlayerFramework.Dash\References\CommonConfiguration\ARM\
copy ..\..\Lib\Portable\Microsoft.Media.ISO\bin\Release\Microsoft.Media.ISO.dll													Microsoft.PlayerFramework.Dash\References\CommonConfiguration\neutral\

copy ..\..\Universal.WinRT.Analytics.Advertising\bin\x86\Release\Microsoft.Media.Analytics.Advertising.pri						Microsoft.PlayerFramework.Analytics.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics.Advertising\
copy ..\..\Universal.WinRT.Analytics.Advertising\bin\x86\Release\Microsoft.Media.Analytics.Advertising.winmd					Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\x86\
copy ..\..\Universal.WinRT.Analytics.Advertising\bin\ARM\Release\Microsoft.Media.Analytics.Advertising.winmd					Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\ARM\

copy ..\..\Universal.WinRT.Analytics.AudienceInsight\bin\x86\Release\Microsoft.Media.Analytics.AudienceInsight.pri				Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics.AudienceInsight\
copy ..\..\Universal.WinRT.Analytics.AudienceInsight\bin\x86\Release\Microsoft.Media.Analytics.AudienceInsight.winmd			Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\x86\
copy ..\..\Universal.WinRT.Analytics.AudienceInsight\bin\ARM\Release\Microsoft.Media.Analytics.AudienceInsight.winmd			Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\ARM\

copy ..\..\Universal.WinRT.AudienceInsight\bin\Release\Microsoft.Media.AudienceInsight.pri										Microsoft.Media.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.Media.AudienceInsight\
copy ..\..\Universal.WinRT.AudienceInsight\bin\Release\Microsoft.Media.AudienceInsight.winmd									Microsoft.Media.AudienceInsight\References\CommonConfiguration\neutral\

@popd

@echo.
@echo Done.
@echo.