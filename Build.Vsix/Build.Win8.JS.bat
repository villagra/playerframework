@set SN=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\x64\sn.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\VSWinExpress.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

"%DEVENV%" /rebuild "Release|AnyCPU" Microsoft.Win8.PlayerFramework.Js.sln
"%DEVENV%" /rebuild "Release|x86"    Microsoft.Win8.PlayerFramework.Js.sln
"%DEVENV%" /rebuild "Release|x64"    Microsoft.Win8.PlayerFramework.Js.sln
"%DEVENV%" /rebuild "Release|ARM"    Microsoft.Win8.PlayerFramework.Js.sln

"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\x86\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\x64\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\ARM\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.winmd								Win8.VideoAnalytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.winmd							Win8.VideoAdvertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.VideoAnalytics.VideoAdvertising\bin\Release\Microsoft.VideoAnalytics.VideoAdvertising.winmd								Win8.VideoAnalytics.VideoAdvertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.VideoAnalytics.AudienceInsight\bin\Release\Microsoft.VideoAnalytics.AudienceInsight.winmd								Win8.VideoAnalytics.AudienceInsight\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.winmd					Win8.Js.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.winmd				Win8.Js.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd					Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd					Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd					Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.AdaptiveStreaming.Dash.winmd			Win8.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Dash\bin\x64\Release\Microsoft.AdaptiveStreaming.Dash.winmd			Win8.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Dash\bin\ARM\Release\Microsoft.AdaptiveStreaming.Dash.winmd			Win8.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AudienceInsight\bin\Release\Microsoft.AudienceInsight.winmd			Win8.AudienceInsight\Microsoft.PlayerFramework.snk


@popd

@pushd %~dp0%

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
mkdir "Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming"
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
mkdir "Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.VideoAdvertising"
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
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming.Analytics"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics"
mkdir "Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics"
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
mkdir "Microsoft.PlayerFramework.Win8.Dash\Redist"
mkdir "Microsoft.PlayerFramework.Win8.Dash\References"
mkdir "Microsoft.PlayerFramework.Win8.Dash\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Win8.Dash\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Win8.Dash\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming.Dash"
mkdir "Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration\x86"
mkdir "Microsoft.AudienceInsight.Win8\Redist"
mkdir "Microsoft.AudienceInsight.Win8\References"
mkdir "Microsoft.AudienceInsight.Win8\Redist\CommonConfiguration"
mkdir "Microsoft.AudienceInsight.Win8\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.AudienceInsight.Win8\Redist\CommonConfiguration\neutral\Microsoft.AudienceInsight"
mkdir "Microsoft.AudienceInsight.Win8\References\CommonConfiguration"
mkdir "Microsoft.AudienceInsight.Win8\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\Redist"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\References"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics.VideoAdvertising"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.Advertising\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\Redist"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics.AudienceInsight"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References\CommonConfiguration\x86"

copyencoded ..\Win8.Js\css\Default-base.css+..\Win8.Js\css\Default-dark.css																								Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-dark.css
copyencoded ..\Win8.Js\css\Default-base.css+..\Win8.Js\css\Default-light.css																							Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-light.css
copyencoded ..\Win8.Js\css\Default-base.css+..\Win8.Js\css\Default-dark.css+..\Win8.Js\css\Entertainment-base.css+..\Win8.Js\css\Entertainment-dark.css					Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-dark.css
copyencoded ..\Win8.Js\css\Default-base.css+..\Win8.Js\css\Default-light.css+..\Win8.Js\css\Entertainment-base.css+..\Win8.Js\css\Entertainment-light.css				Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-light.css
copyencoded ..\Win8.Js\js\PlayerFramework.js+..\Win8.Js\js\InteractiveViewModel.js+..\Win8.Js\js\MediaPlayer.js+..\Win8.Js\js\DynamicTextTrack.js+..\Win8.Js\js\ui\Button.js+..\Win8.Js\js\ui\ControlPanel.js+..\Win8.Js\js\ui\Indicator.js+..\Win8.Js\js\ui\Meter.js+..\Win8.Js\js\ui\Slider.js+..\Win8.Js\js\plugins\PluginBase.js+..\Win8.Js\js\plugins\TrackingPluginBase.js+..\Win8.Js\js\plugins\BufferingPlugin.js+..\Win8.Js\js\plugins\ControlPlugin.js+..\Win8.Js\js\plugins\ErrorPlugin.js+..\Win8.Js\js\plugins\LoaderPlugin.js+..\Win8.Js\js\plugins\PlaylistPlugin.js+..\Win8.Js\js\plugins\PlayTimeTrackingPlugin.js+..\Win8.Js\js\plugins\PositionTrackingPlugin.js+..\Win8.Js\js\plugins\MediaControlPlugin.js+..\Win8.Js\js\plugins\ChaptersPlugin.js+..\Win8.Js\js\plugins\DisplayRequestPlugin.js+..\Win8.Js\js\plugins\CaptionSelectorPlugin.js+..\Win8.Js\js\plugins\AudioSelectorPlugin.js Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\PlayerFramework.js
copy ..\Win8.Js\images\thumb-dark.png																																	Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\images\
copy ..\Win8.Js\images\thumb-light.png																																	Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\images\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-dark.css										Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-light.css										Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-dark.css						Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\PlayerFramework-Entertainment-light.css						Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\css\
copy Microsoft.PlayerFramework.Js\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\PlayerFramework.js												Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\js\
copy ..\Win8.Js\bin\Release\Microsoft.PlayerFramework.Js.pri																											Microsoft.PlayerFramework.Js\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js\
@rem copy ..\Win8.Js\bin\Release\Microsoft.PlayerFramework.Js.winmd																										Microsoft.PlayerFramework.Js\References\CommonConfiguration\neutral\

copyencoded ..\Win8.Js.Adaptive\js\AdaptivePlugin.js																													Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\PlayerFramework.Adaptive.js
copy Microsoft.PlayerFramework.Js.Adaptive\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\PlayerFramework.Adaptive.js					Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\js\
copy ..\Win8.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.pri																						Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Adaptive\
copy ..\Win8.AdaptiveStreaming\bin\x86\Release\Microsoft.AdaptiveStreaming.pri																							Microsoft.PlayerFramework.Js.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming\
copy ..\Win8.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																					Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x86\
@rem copy ..\Win8.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.xml 																				Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x86\
copy ..\Win8.AdaptiveStreaming\bin\x86\Release\Microsoft.AdaptiveStreaming.winmd																						Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x86\
copy ..\Win8.Js.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																					Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x64\
@rem copy ..\Win8.Js.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Js.Adaptive.xml 																				Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x64\
copy ..\Win8.AdaptiveStreaming\bin\x64\Release\Microsoft.AdaptiveStreaming.winmd																						Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\x64\
copy ..\Win8.Js.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd																					Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\ARM\
@rem copy ..\Win8.Js.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Js.Adaptive.xml 																				Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\ARM\
copy ..\Win8.AdaptiveStreaming\bin\ARM\Release\Microsoft.AdaptiveStreaming.winmd																						Microsoft.PlayerFramework.Js.Adaptive\References\CommonConfiguration\ARM\

copyencoded ..\Win8.Js.Advertising\css\Advertising.css																													Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\PlayerFramework.Advertising.css
copyencoded ..\Win8.Js.Advertising\js\AdHandlerPlugin.js+..\Win8.Js.Advertising\js\AdPayloadHandlerPluginBase.js+..\Win8.Js.Advertising\js\AdPlayerFactoryPluginBase.js+..\Win8.Js.Advertising\js\AdPlayerFactoryPlugin.js+..\Win8.Js.Advertising\js\AdSchedulerPlugin.js+..\Win8.Js.Advertising\js\Advertisement.js+..\Win8.Js.Advertising\js\FreeWheelPlugin.js+..\Win8.Js.Advertising\js\MastSchedulerPlugin.js+..\Win8.Js.Advertising\js\MediaPlayerAdapter.js+..\Win8.Js.Advertising\js\VmapSchedulerPlugin.js+..\Win8.Js.Advertising\js\VpaidAdPlayerBase.js+..\Win8.Js.Advertising\js\VpaidNonLinearAdPlayers.js+..\Win8.Js.Advertising\js\VpaidVideoAdPlayer.js+..\Win8.Js.Advertising\js\VpaidAdapter.js+..\Win8.Js.Advertising\js\VpaidLinearAdViewModel.js+..\Win8.Js.Advertising\js\VpaidNonLinearAdViewModel.js Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\PlayerFramework.Advertising.js
copy Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\PlayerFramework.Advertising.css		Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\css\
copy Microsoft.PlayerFramework.Js.Advertising\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\PlayerFramework.Advertising.js			Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\js\
copy ..\Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.pri																					Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Advertising\
copy ..\Win8.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.pri																								Microsoft.PlayerFramework.Js.Advertising\Redist\CommonConfiguration\neutral\Microsoft.VideoAdvertising\
copy ..\Win8.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.winmd																								Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
copy ..\Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.winmd																					Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
copy ..\Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.xml 																					Microsoft.PlayerFramework.Js.Advertising\References\CommonConfiguration\neutral\
copyencoded ..\Win8.Js.TimedText\css\TimedText.css																														Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\PlayerFramework.TimedText.css
copyencoded ..\Win8.Js.TimedText\js\CaptionsPlugin.js+..\Win8.Js.TimedText\js\TtmlParser.js																				Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\PlayerFramework.TimedText.js
copy Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\PlayerFramework.TimedText.css				Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\css\
copy Microsoft.PlayerFramework.Js.TimedText\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\PlayerFramework.TimedText.js				Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\js\
copy ..\Win8.Js.TimedText\bin\Release\Microsoft.PlayerFramework.Js.TimedText.pri																						Microsoft.PlayerFramework.Js.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.TimedText\
@rem copy ..\Win8.Js.TimedText\bin\Release\Microsoft.PlayerFramework.Js.TimedText.winmd																					Microsoft.PlayerFramework.Js.TimedText\References\CommonConfiguration\neutral\

copyencoded ..\Win8.Js.Analytics\js\ErrorLogger.js+..\Win8.Js.Analytics\js\MediaPlayerAdapter.js+..\Win8.Js.Analytics\js\AdaptiveAnalyticsPlugin.js+..\Win8.Js.Analytics\js\AnalyticsPlugin.js	Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\PlayerFramework.Analytics.js
copy Microsoft.PlayerFramework.Js.Analytics\DesignTime\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\PlayerFramework.Analytics.js				Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\js\
copy ..\Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.pri																						Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Js.Analytics\
copy ..\Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.pri																									Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics\
copy ..\Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.winmd																									Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.xml																									Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.winmd																						Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.xml 																						Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\neutral\
copy ..\Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.pri																		Microsoft.PlayerFramework.Js.Analytics\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming.Analytics\
copy ..\Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.winmd																	Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\x86\
copy ..\Win8.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.AdaptiveStreaming.Analytics.winmd																	Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\x64\
copy ..\Win8.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.AdaptiveStreaming.Analytics.winmd																	Microsoft.PlayerFramework.Js.Analytics\References\CommonConfiguration\ARM\

copy ..\Win8.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.AdaptiveStreaming.Dash.pri																				Microsoft.PlayerFramework.Win8.Dash\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming.Dash\
copy ..\Win8.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.AdaptiveStreaming.Dash.winmd																				Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration\x86\
copy ..\Win8.AdaptiveStreaming.Dash\bin\x64\Release\Microsoft.AdaptiveStreaming.Dash.winmd																				Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration\x64\
copy ..\Win8.AdaptiveStreaming.Dash\bin\ARM\Release\Microsoft.AdaptiveStreaming.Dash.winmd																				Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration\ARM\
copy ..\Lib\Portable\Microsoft.Media.ISO\bin\Release\Microsoft.Media.ISO.dll																							Microsoft.PlayerFramework.Win8.Dash\References\CommonConfiguration\neutral\

copy ..\Win8.AudienceInsight\bin\Release\Microsoft.AudienceInsight.pri																															Microsoft.AudienceInsight.Win8\Redist\CommonConfiguration\neutral\Microsoft.AudienceInsight\
copy ..\Win8.AudienceInsight\bin\Release\Microsoft.AudienceInsight.winmd																															Microsoft.AudienceInsight.Win8\References\CommonConfiguration\neutral\
copy ..\Win8.VideoAnalytics.VideoAdvertising\bin\x86\Release\Microsoft.VideoAnalytics.VideoAdvertising.pri																		Microsoft.PlayerFramework.Win8.Analytics.Advertising\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics.VideoAdvertising\
copy ..\Win8.VideoAnalytics.VideoAdvertising\bin\x86\Release\Microsoft.VideoAnalytics.VideoAdvertising.winmd																		Microsoft.PlayerFramework.Win8.Analytics.Advertising\References\CommonConfiguration\x86\
copy ..\Win8.VideoAnalytics.VideoAdvertising\bin\x64\Release\Microsoft.VideoAnalytics.VideoAdvertising.winmd																		Microsoft.PlayerFramework.Win8.Analytics.Advertising\References\CommonConfiguration\x64\
copy ..\Win8.VideoAnalytics.VideoAdvertising\bin\ARM\Release\Microsoft.VideoAnalytics.VideoAdvertising.winmd																		Microsoft.PlayerFramework.Win8.Analytics.Advertising\References\CommonConfiguration\ARM\

copy ..\Win8.VideoAnalytics.AudienceInsight\bin\x86\Release\Microsoft.VideoAnalytics.AudienceInsight.pri																		Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics.AudienceInsight\
copy ..\Win8.VideoAnalytics.AudienceInsight\bin\x86\Release\Microsoft.VideoAnalytics.AudienceInsight.winmd																		Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References\CommonConfiguration\x86\
copy ..\Win8.VideoAnalytics.AudienceInsight\bin\x64\Release\Microsoft.VideoAnalytics.AudienceInsight.winmd																		Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References\CommonConfiguration\x64\
copy ..\Win8.VideoAnalytics.AudienceInsight\bin\ARM\Release\Microsoft.VideoAnalytics.AudienceInsight.winmd																		Microsoft.PlayerFramework.Win8.Analytics.AudienceInsight\References\CommonConfiguration\ARM\


@popd

@echo.
@echo Done.
@echo.
