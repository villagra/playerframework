@set SN=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\x64\sn.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

"%DEVENV%" /rebuild "Release|AnyCPU" Microsoft.PlayerFramework.Universal.Js.sln
"%DEVENV%" /rebuild "Release|x86"    Microsoft.PlayerFramework.Universal.Js.sln
"%DEVENV%" /rebuild "Release|x64"    Microsoft.PlayerFramework.Universal.Js.sln
"%DEVENV%" /rebuild "Release|ARM"    Microsoft.PlayerFramework.Universal.Js.sln

"%SN%" -R Universal.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd										Universal.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.Js.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd										Universal.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.Js.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd										Universal.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.winmd									Universal.Js.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Universal.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.winmd										Universal.Js.Analytics\Microsoft.PlayerFramework.snk

@popd

@pushd %~dp0%

cd WP81

mkdir "Microsoft.PlayerFramework.Js.Core\DesignTime"
mkdir "Microsoft.PlayerFramework.Js.Core\Redist"
mkdir "Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js"
mkdir "Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css"
mkdir "Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js"
mkdir "Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js"
mkdir "Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css"
mkdir "Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\images"
mkdir "Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js"
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

..\copyencoded ..\..\Universal.Js.Core\css\Default-base.css+..\..\Universal.Js.Core\css\Default-dark.css															Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-dark.css
..\copyencoded ..\..\Universal.Js.Core\css\Default-base.css+..\..\Universal.Js.Core\css\Default-light.css															Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-light.css
..\copyencoded ..\..\Universal.Js.Core\css\Default-base.css+..\..\Universal.Js.Core\css\Default-dark.css+..\..\Universal.Js.Core\css\Entertainment-base.css+..\..\Universal.Js.Core\css\Entertainment-dark.css		Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-dark.css
..\copyencoded ..\..\Universal.Js.Core\css\Default-base.css+..\..\Universal.Js.Core\css\Default-light.css+..\..\Universal.Js.Core\css\Entertainment-base.css+..\..\Universal.Js.Core\css\Entertainment-light.css	Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-light.css
..\copyencoded ..\..\Universal.Js.Core\js\PlayerFramework.js+..\..\Universal.Js.Core\js\InteractiveViewModel.js+..\..\Universal.Js.Core\js\MediaPlayer.js+..\..\Universal.Js.Core\js\DynamicTextTrack.js+..\..\Universal.Js.Core\js\ui\Button.js+..\..\Universal.Js.Core\js\ui\ControlPanel.js+..\..\Universal.Js.Core\js\ui\Indicator.js+..\..\Universal.Js.Core\js\ui\Meter.js+..\..\Universal.Js.Core\js\ui\Slider.js+..\..\Universal.Js.Core\js\plugins\PluginBase.js+..\..\Universal.Js.Core\js\plugins\TrackingPluginBase.js+..\..\Universal.Js.Core\js\plugins\BufferingPlugin.js+..\..\Universal.Js.Core\js\plugins\ControlPlugin.js+..\..\Universal.Js.Core\js\plugins\ErrorPlugin.js+..\..\Universal.Js.Core\js\plugins\LoaderPlugin.js+..\..\Universal.Js.Core\js\plugins\PlaylistPlugin.js+..\..\Universal.Js.Core\js\plugins\PlayTimeTrackingPlugin.js+..\..\Universal.Js.Core\js\plugins\PositionTrackingPlugin.js+..\..\Universal.Js.Core\js\plugins\SystemTransportControlsPlugin.js+..\..\Universal.Js.Core\js\plugins\ChaptersPlugin.js+..\..\Universal.Js.Core\js\plugins\DisplayRequestPlugin.js+..\..\Universal.Js.Core\js\plugins\CaptionSelectorPlugin.js+..\..\Universal.Js.Core\js\plugins\AudioSelectorPlugin.js Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\PlayerFramework.js
copy ..\..\Universal.Js.Core\images\thumb-dark.png																													Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\images\
copy ..\..\Universal.Js.Core\images\thumb-light.png																													Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\images\
copy Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-dark.css								Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-light.css							Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-dark.css				Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-light.css				Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js.Core\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\PlayerFramework.js									Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\
copy ..\..\Universal.Js.Core\bin\Release\Microsoft.PlayerFramework.Js.pri																							Microsoft.PlayerFramework.Js.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\

..\copyencoded ..\..\Universal.Js.Adaptive\js\AdaptivePlugin.js																											Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\PlayerFramework.Adaptive.js
copy Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\PlayerFramework.Adaptive.js					Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\
copy ..\..\Universal.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.pri																				Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\
copy ..\..\Win81.WinRT.AdaptiveStreaming.Helper\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.pri															Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Helper\
copy ..\..\Universal.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																			Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x86\
copy ..\..\Win81.WinRT.AdaptiveStreaming.Helper\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd															Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x86\
copy ..\..\Universal.Js.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																			Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x64\
copy ..\..\Win81.WinRT.AdaptiveStreaming.Helper\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd															Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x64\
copy ..\..\Universal.Js.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																			Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\ARM\
copy ..\..\Win81.WinRT.AdaptiveStreaming.Helper\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd															Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\ARM\

..\copyencoded ..\..\Universal.Js.Advertising\css\Advertising.css																										Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\PlayerFramework.Advertising.css
..\copyencoded ..\..\Universal.Js.Advertising\js\AdHandlerPlugin.js+..\..\Universal.Js.Advertising\js\AdPayloadHandlerPluginBase.js+..\..\Universal.Js.Advertising\js\AdPlayerFactoryPluginBase.js+..\..\Universal.Js.Advertising\js\AdPlayerFactoryPlugin.js+..\..\Universal.Js.Advertising\js\AdSchedulerPlugin.js+..\..\Universal.Js.Advertising\js\Advertisement.js+..\..\Universal.Js.Advertising\js\FreeWheelPlugin.js+..\..\Universal.Js.Advertising\js\MastSchedulerPlugin.js+..\..\Universal.Js.Advertising\js\MediaPlayerAdapter.js+..\..\Universal.Js.Advertising\js\VmapSchedulerPlugin.js+..\..\Universal.Js.Advertising\js\VpaidAdPlayerBase.js+..\..\Universal.Js.Advertising\js\VpaidNonLinearAdPlayers.js+..\..\Universal.Js.Advertising\js\VpaidVideoAdPlayer.js+..\..\Universal.Js.Advertising\js\VpaidAdapter.js+..\..\Universal.Js.Advertising\js\VpaidLinearAdViewModel.js+..\..\Universal.Js.Advertising\js\VpaidNonLinearAdViewModel.js Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\PlayerFramework.Advertising.js
copy Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\PlayerFramework.Advertising.css		Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\
copy Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\PlayerFramework.Advertising.js			Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\
copy ..\..\Universal.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.pri																			Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\
copy ..\..\Universal.WinRT.Advertising\bin\Release\Microsoft.Media.Advertising.pri																						Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Advertising\
copy ..\..\Universal.WinRT.Advertising\bin\Release\Microsoft.Media.Advertising.winmd																					Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Universal.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.winmd																			Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
copy ..\..\Universal.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.xml 																			Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
..\copyencoded ..\..\Universal.Js.TimedText\css\TimedText.css																											Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\PlayerFramework.TimedText.css
..\copyencoded ..\..\Universal.Js.TimedText\js\CaptionsPlugin.js+..\..\Universal.Js.TimedText\js\TtmlParser.js															Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\PlayerFramework.TimedText.js
copy Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\PlayerFramework.TimedText.css				Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\
copy Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\PlayerFramework.TimedText.js				Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\
copy ..\..\Universal.Js.TimedText\bin\Release\Microsoft.PlayerFramework.Js.TimedText.pri																				Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\

..\copyencoded ..\..\Universal.Js.Analytics\js\ErrorLogger.js+..\..\Universal.Js.Analytics\js\MediaPlayerAdapter.js+..\..\Universal.Js.Analytics\js\AdaptiveAnalyticsPlugin.js+..\..\Universal.Js.Analytics\js\AnalyticsPlugin.js	Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\PlayerFramework.Analytics.js
copy Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\PlayerFramework.Analytics.js				Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\
copy ..\..\Universal.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.pri																				Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\
copy ..\..\Universal.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.pri																							Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics\
copy ..\..\Universal.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd																						Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Universal.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.xml																							Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Universal.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.winmd																				Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Universal.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.xml 																				Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\..\Win81.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.pri														Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Analytics\
copy ..\..\Win81.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd													Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\x86\
copy ..\..\Win81.WinRT.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd													Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\x64\
copy ..\..\Win81.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd													Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\ARM\

@popd

@echo.
@echo Done.
@echo.