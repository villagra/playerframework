@set SN=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\x64\sn.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\VSWinExpress.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

rem "%DEVENV%" /rebuild "Release|AnyCPU" Microsoft.PlayerFramework.Win8.Js.sln
rem "%DEVENV%" /rebuild "Release|x86"    Microsoft.PlayerFramework.Win8.Js.sln
rem "%DEVENV%" /rebuild "Release|x64"    Microsoft.PlayerFramework.Win8.Js.sln
rem "%DEVENV%" /rebuild "Release|ARM"    Microsoft.PlayerFramework.Win8.Js.sln

"%SN%" -R Win8.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd										Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Helper\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd				Win8.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd										Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Helper\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd				Win8.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd										Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Helper\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd				Win8.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Advertising\bin\Release\Microsoft.Media.Advertising.winmd												Win8.WinRT.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.winmd									Win8.Js.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd													Win8.WinRT.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.winmd										Win8.Js.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd			Win8.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd			Win8.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd			Win8.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd					Win8.WinRT.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Dash\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd					Win8.WinRT.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AdaptiveStreaming.Dash\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd					Win8.WinRT.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Analytics.Advertising\bin\x86\Release\Microsoft.Media.Analytics.Advertising.winmd						Win8.WinRT.Analytics.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Analytics.Advertising\bin\x64\Release\Microsoft.Media.Analytics.Advertising.winmd						Win8.WinRT.Analytics.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Analytics.Advertising\bin\ARM\Release\Microsoft.Media.Analytics.Advertising.winmd						Win8.WinRT.Analytics.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Analytics.AudienceInsight\bin\x86\Release\Microsoft.Media.Analytics.AudienceInsight.winmd				Win8.WinRT.Analytics.AudienceInsight\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Analytics.AudienceInsight\bin\x64\Release\Microsoft.Media.Analytics.AudienceInsight.winmd				Win8.WinRT.Analytics.AudienceInsight\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.Analytics.AudienceInsight\bin\ARM\Release\Microsoft.Media.Analytics.AudienceInsight.winmd				Win8.WinRT.Analytics.AudienceInsight\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.WinRT.AudienceInsight\bin\Release\Microsoft.Media.AudienceInsight.winmd										Win8.WinRT.AudienceInsight\Microsoft.PlayerFramework.snk

@popd

@pushd %~dp0%

cd Win80

mkdir "Microsoft.PlayerFramework.Js\DesignTime"
mkdir "Microsoft.PlayerFramework.Js\Redist"
mkdir "Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js"
mkdir "Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css"
mkdir "Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js"
mkdir "Microsoft.PlayerFramework.Js\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js"
mkdir "Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css"
mkdir "Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\images"
mkdir "Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\DesignTime"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\Redist"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\References"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Helper"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Js.Advertising\DesignTime"
mkdir "Microsoft.PlayerFramework.Js.Advertising\Redist"
mkdir "Microsoft.PlayerFramework.Js.Advertising\References"
mkdir "Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising"
mkdir "Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css"
mkdir "Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js"
mkdir "Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising"
mkdir "Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Advertising"
mkdir "Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css"
mkdir "Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js"
mkdir "Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Analytics\DesignTime"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist"
mkdir "Microsoft.PlayerFramework.Js.Analytics\References"
mkdir "Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics"
mkdir "Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Analytics"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js"
mkdir "Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Js.TimedText\DesignTime"
mkdir "Microsoft.PlayerFramework.Js.TimedText\Redist"
mkdir "Microsoft.PlayerFramework.Js.TimedText\References"
mkdir "Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText"
mkdir "Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css"
mkdir "Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js"
mkdir "Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText"
mkdir "Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css"
mkdir "Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js"
mkdir "Microsoft.PlayerFramework.Js.TimedText\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.TimedText\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Dash\Redist"
mkdir "Microsoft.PlayerFramework.Dash\References"
mkdir "Microsoft.PlayerFramework.Dash\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Dash\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Dash\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Dash"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Dash\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\Redist"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics.Advertising"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics.AudienceInsight"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\x86"
mkdir "Microsoft.Media.AudienceInsight\Redist"
mkdir "Microsoft.Media.AudienceInsight\References"
mkdir "Microsoft.Media.AudienceInsight\Redist\CommonConfiguration"
mkdir "Microsoft.Media.AudienceInsight\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.Media.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.Media.AudienceInsight"
mkdir "Microsoft.Media.AudienceInsight\References\CommonConfiguration"
mkdir "Microsoft.Media.AudienceInsight\References\CommonConfiguration\neutral"

..\copyencoded ..\..\Win8.Js\css\Default-base.css+..\..\Win8.Js\css\Default-dark.css																						Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-dark.css
..\copyencoded ..\..\Win8.Js\css\Default-base.css+..\..\Win8.Js\css\Default-light.css																						Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-light.css
..\copyencoded ..\..\Win8.Js\css\Default-base.css+..\..\Win8.Js\css\Default-dark.css+..\..\Win8.Js\css\Entertainment-base.css+..\..\Win8.Js\css\Entertainment-dark.css		Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-dark.css
..\copyencoded ..\..\Win8.Js\css\Default-base.css+..\..\Win8.Js\css\Default-light.css+..\..\Win8.Js\css\Entertainment-base.css+..\..\Win8.Js\css\Entertainment-light.css	Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-light.css
..\copyencoded ..\..\Win8.Js\js\PlayerFramework.js+..\..\Win8.Js\js\InteractiveViewModel.js+..\..\Win8.Js\js\MediaPlayer.js+..\..\Win8.Js\js\DynamicTextTrack.js+..\..\Win8.Js\js\ui\Button.js+..\..\Win8.Js\js\ui\ControlPanel.js+..\..\Win8.Js\js\ui\Indicator.js+..\..\Win8.Js\js\ui\Meter.js+..\..\Win8.Js\js\ui\Slider.js+..\..\Win8.Js\js\plugins\PluginBase.js+..\..\Win8.Js\js\plugins\TrackingPluginBase.js+..\..\Win8.Js\js\plugins\BufferingPlugin.js+..\..\Win8.Js\js\plugins\ControlPlugin.js+..\..\Win8.Js\js\plugins\ErrorPlugin.js+..\..\Win8.Js\js\plugins\LoaderPlugin.js+..\..\Win8.Js\js\plugins\PlaylistPlugin.js+..\..\Win8.Js\js\plugins\PlayTimeTrackingPlugin.js+..\..\Win8.Js\js\plugins\PositionTrackingPlugin.js+..\..\Win8.Js\js\plugins\MediaControlPlugin.js+..\..\Win8.Js\js\plugins\ChaptersPlugin.js+..\..\Win8.Js\js\plugins\DisplayRequestPlugin.js+..\..\Win8.Js\js\plugins\CaptionSelectorPlugin.js+..\..\Win8.Js\js\plugins\AudioSelectorPlugin.js Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\PlayerFramework.js
copy ..\..\Win8.Js\images\thumb-dark.png																																Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\images\
copy ..\..\Win8.Js\images\thumb-light.png																																Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\images\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-dark.css										Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-light.css										Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-dark.css						Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-light.css						Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\PlayerFramework.js												Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\
copy ..\..\Win8.Js\bin\Release\Microsoft.PlayerFramework.Js.pri																											Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\

..\copyencoded ..\..\Win8.Js.Adaptive\js\AdaptivePlugin.js																												Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\PlayerFramework.Adaptive.js
copy Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\PlayerFramework.Adaptive.js					Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\
copy ..\..\Win8.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.pri																					Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Helper\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.pri																Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Helper\
copy ..\..\Win8.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																					Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x86\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Helper\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd															Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x86\
copy ..\..\Win8.Js.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																					Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x64\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Helper\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd															Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x64\
copy ..\..\Win8.Js.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																					Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\ARM\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Helper\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd															Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\ARM\

..\copyencoded ..\..\Win8.Js.Advertising\css\Advertising.css																											Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\PlayerFramework.Advertising.css
..\copyencoded ..\..\Win8.Js.Advertising\js\AdHandlerPlugin.js+..\..\Win8.Js.Advertising\js\AdPayloadHandlerPluginBase.js+..\..\Win8.Js.Advertising\js\AdPlayerFactoryPluginBase.js+..\..\Win8.Js.Advertising\js\AdPlayerFactoryPlugin.js+..\..\Win8.Js.Advertising\js\AdSchedulerPlugin.js+..\..\Win8.Js.Advertising\js\Advertisement.js+..\..\Win8.Js.Advertising\js\FreeWheelPlugin.js+..\..\Win8.Js.Advertising\js\MastSchedulerPlugin.js+..\..\Win8.Js.Advertising\js\MediaPlayerAdapter.js+..\..\Win8.Js.Advertising\js\VmapSchedulerPlugin.js+..\..\Win8.Js.Advertising\js\VpaidAdPlayerBase.js+..\..\Win8.Js.Advertising\js\VpaidNonLinearAdPlayers.js+..\..\Win8.Js.Advertising\js\VpaidVideoAdPlayer.js+..\..\Win8.Js.Advertising\js\VpaidAdapter.js+..\..\Win8.Js.Advertising\js\VpaidLinearAdViewModel.js+..\..\Win8.Js.Advertising\js\VpaidNonLinearAdViewModel.js Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\PlayerFramework.Advertising.js
copy Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\PlayerFramework.Advertising.css		Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\
copy Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\PlayerFramework.Advertising.js			Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\
copy ..\..\Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.pri																					Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\
copy ..\..\Win8.WinRT.Advertising\bin\Release\Microsoft.Media.Advertising.pri																							Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Advertising\
copy ..\..\Win8.WinRT.Advertising\bin\Release\Microsoft.Media.Advertising.winmd																							Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.winmd																				Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.xml 																				Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
..\copyencoded ..\..\Win8.Js.TimedText\css\TimedText.css																												Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\PlayerFramework.TimedText.css
..\copyencoded ..\..\Win8.Js.TimedText\js\CaptionsPlugin.js+..\..\Win8.Js.TimedText\js\TtmlParser.js																	Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\PlayerFramework.TimedText.js
copy Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\PlayerFramework.TimedText.css				Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\
copy Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\PlayerFramework.TimedText.js				Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\
copy ..\..\Win8.Js.TimedText\bin\Release\Microsoft.PlayerFramework.Js.TimedText.pri																						Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\

..\copyencoded ..\..\Win8.Js.Analytics\js\ErrorLogger.js+..\..\Win8.Js.Analytics\js\MediaPlayerAdapter.js+..\..\Win8.Js.Analytics\js\AdaptiveAnalyticsPlugin.js+..\..\Win8.Js.Analytics\js\AnalyticsPlugin.js	Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\PlayerFramework.Analytics.js
copy Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\PlayerFramework.Analytics.js				Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\
copy ..\..\Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.pri																						Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\
copy ..\..\Win8.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.pri																								Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics\
copy ..\..\Win8.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd																								Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Win8.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.xml																								Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.winmd																					Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.xml 																					Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.pri														Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Analytics\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd														Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\x86\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd														Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\x64\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd														Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\ARM\

copy ..\..\Win8.WinRT.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Dash.pri																	Microsoft.PlayerFramework.Dash\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Dash\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd																Microsoft.PlayerFramework.Dash\References\CommonConfiguration\x86\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Dash\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd																Microsoft.PlayerFramework.Dash\References\CommonConfiguration\x64\
copy ..\..\Win8.WinRT.AdaptiveStreaming.Dash\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Dash.winmd																Microsoft.PlayerFramework.Dash\References\CommonConfiguration\ARM\
copy ..\..\Lib\Portable\Microsoft.Media.ISO\bin\Release\Microsoft.Media.ISO.dll																							Microsoft.PlayerFramework.Dash\References\CommonConfiguration\neutral\

copy ..\..\Win8.WinRT.Analytics.Advertising\bin\x86\Release\Microsoft.Media.Analytics.Advertising.pri																	Microsoft.PlayerFramework.Analytics.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics.Advertising\
copy ..\..\Win8.WinRT.Analytics.Advertising\bin\x86\Release\Microsoft.Media.Analytics.Advertising.winmd																	Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\x86\
copy ..\..\Win8.WinRT.Analytics.Advertising\bin\x64\Release\Microsoft.Media.Analytics.Advertising.winmd																	Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\x64\
copy ..\..\Win8.WinRT.Analytics.Advertising\bin\ARM\Release\Microsoft.Media.Analytics.Advertising.winmd																	Microsoft.PlayerFramework.Analytics.Advertising\References\CommonConfiguration\ARM\

copy ..\..\Win8.WinRT.Analytics.AudienceInsight\bin\x86\Release\Microsoft.Media.Analytics.AudienceInsight.pri															Microsoft.PlayerFramework.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics.AudienceInsight\
copy ..\..\Win8.WinRT.Analytics.AudienceInsight\bin\x86\Release\Microsoft.Media.Analytics.AudienceInsight.winmd															Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\x86\
copy ..\..\Win8.WinRT.Analytics.AudienceInsight\bin\x64\Release\Microsoft.Media.Analytics.AudienceInsight.winmd															Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\x64\
copy ..\..\Win8.WinRT.Analytics.AudienceInsight\bin\ARM\Release\Microsoft.Media.Analytics.AudienceInsight.winmd															Microsoft.PlayerFramework.Analytics.AudienceInsight\References\CommonConfiguration\ARM\

copy ..\..\Win8.WinRT.AudienceInsight\bin\Release\Microsoft.Media.AudienceInsight.pri																					Microsoft.Media.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.Media.AudienceInsight\
copy ..\..\Win8.WinRT.AudienceInsight\bin\Release\Microsoft.Media.AudienceInsight.winmd																					Microsoft.Media.AudienceInsight\References\CommonConfiguration\neutral\

@popd

@echo.
@echo Done.
@echo.
