@set SN=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\x64\sn.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 12.0\Common7\IDE\VSWinExpress.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

"%DEVENV%" /rebuild "Release|AnyCPU" Microsoft.Win8.PlayerFramework.Xaml.sln
"%DEVENV%" /rebuild "Release|x86"    Microsoft.Win8.PlayerFramework.Xaml.sln
"%DEVENV%" /rebuild "Release|x64"    Microsoft.Win8.PlayerFramework.Xaml.sln
"%DEVENV%" /rebuild "Release|ARM"    Microsoft.Win8.PlayerFramework.Xaml.sln

"%SN%" -R Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.AdaptiveStreaming.winmd							Win8.Xaml.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.AdaptiveStreaming.winmd							Win8.Xaml.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.AdaptiveStreaming.winmd							Win8.Xaml.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Advertising\bin\Release\Microsoft.VideoAdvertising.winmd								Win8.Xaml.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.WebVTT\bin\Release\Microsoft.WebVTT.winmd												Win8.Xaml.WebVTT\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\x86\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\x64\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\ARM\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.winmd									Win8.VideoAnalytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.winmd								Win8.VideoAdvertising\Microsoft.PlayerFramework.snk

@popd

@pushd %~dp0%

mkdir "Microsoft.PlayerFramework.Xaml\Redist"
mkdir "Microsoft.PlayerFramework.Xaml\References"
mkdir "Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework"
mkdir "Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes"
mkdir "Microsoft.PlayerFramework.Xaml\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming"
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
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.VideoAdvertising"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\themes"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming.Analytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Analytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x64"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionMarkers\References"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionMarkers"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionMarkers\themes"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionMarkers\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionMarkers\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\References"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TimedText"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TTML.CaptionSettings"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.TimedText"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.TimedText\themes"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT.CaptionSettings"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.WebVTT"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.WebVTT\themes"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\References"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Controls"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Themes"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration\neutral"

copy ..\Win8.Xaml\bin\Release\Microsoft.PlayerFramework.dll  Microsoft.PlayerFramework.Xaml\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml\bin\Release\Microsoft.PlayerFramework.xml  Microsoft.PlayerFramework.Xaml\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml\bin\Release\Microsoft.PlayerFramework.pri  Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\
copy ..\Win8.Xaml\bin\Release\Themes\generic.xaml            Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\Win8.Xaml\bin\Release\Themes\EntertainmentTheme.xaml Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes

copy ..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.pri					Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Adaptive\
copy ..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.AdaptiveStreaming.pri						    Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming\

copy ..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy ..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy ..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.AdaptiveStreaming.winmd 					    Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
@rem copy ..\Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.AdaptiveStreaming.xml						Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\

copy ..\Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
copy ..\Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
copy ..\Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.AdaptiveStreaming.winmd 					    Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
@rem copy ..\Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.AdaptiveStreaming.xml						Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\

copy ..\Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
copy ..\Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
copy ..\Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.AdaptiveStreaming.winmd 					    Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
@rem copy ..\Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.AdaptiveStreaming.xml						Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\

@rem copy ..\Win8.Xaml.CaptionMarkers\bin\Release\Microsoft.PlayerFramework.CaptionMarkers.dll Microsoft.PlayerFramework.Xaml.CaptionMarkers\References\CommonConfiguration\neutral\
@rem copy ..\Win8.Xaml.CaptionMarkers\bin\Release\Microsoft.PlayerFramework.CaptionMarkers.xml Microsoft.PlayerFramework.Xaml.CaptionMarkers\References\CommonConfiguration\neutral\
@rem copy ..\Win8.Xaml.CaptionMarkers\bin\Release\Microsoft.PlayerFramework.CaptionMarkers.pri Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionMarkers\
@rem copy ..\Win8.Xaml.CaptionMarkers\bin\Release\Themes\generic.xaml                          Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionMarkers\themes\

copy ..\Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll	Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.xml	Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.pri	Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TimedText\
copy ..\Win8.Xaml.TimedText\bin\Release\Microsoft.TimedText.dll					Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
@rem copy ..\Win8.Xaml.TimedText\bin\Release\Microsoft.TimedText.xml			Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.TimedText\bin\Release\Microsoft.TimedText.pri					Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.TimedText\
copy ..\Win8.TimedText\bin\Release\Themes\generic.xaml							Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.TimedText\themes\

copy ..\Win8.Xaml.TTML.CaptionSettings\bin\Release\Microsoft.PlayerFramework.TTML.CaptionSettings.xml	Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.TTML.CaptionSettings\bin\Release\Microsoft.PlayerFramework.TTML.CaptionSettings.dll	Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.TTML.CaptionSettings\bin\Release\Microsoft.PlayerFramework.TTML.CaptionSettings.pri	Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TTML.CaptionSettings\

copy ..\Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll		Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.xml		Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.pri		Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\
copy ..\Win8.Xaml.Advertising\bin\Release\Themes\generic.xaml							Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\themes\
copy ..\Win8.Xaml.Advertising\bin\Release\Microsoft.VideoAdvertising.winmd 				Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
@rem copy ..\Win8.Xaml.Advertising\bin\Release\Microsoft.VideoAdvertising.xml			Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.Advertising\bin\Release\Microsoft.VideoAdvertising.pri				Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.VideoAdvertising\

copy ..\Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll	Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml	Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.pri	Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Analytics\
copy ..\Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
@rem copy ..\Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.xml		Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.pri			Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics\

copy ..\Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.pri				Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming.Analytics\
copy ..\Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86\
@rem copy ..\Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.xml			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86\
copy ..\Win8.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x64\
@rem copy ..\Win8.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.AdaptiveStreaming.Analytics.xml			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x64\
copy ..\Win8.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM\
@rem copy ..\Win8.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.AdaptiveStreaming.Analytics.xml			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM\

copy ..\Win8.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll		Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
@rem copy ..\Win8.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.xml	Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.pri		Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT\
copy ..\Win8.Xaml.WebVTT\bin\Release\Microsoft.WebVTT.winmd						Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
@rem copy ..\Win8.Xaml.WebVTT\bin\Release\Microsoft.WebVTT.xml					Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.WebVTT\bin\Release\Microsoft.WebVTT.pri						Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.WebVTT\
copy ..\Win8.WebVTT\bin\Release\Themes\generic.xaml								Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.WebVTT\themes\

copy ..\Win8.Xaml.WebVTT.CaptionSettings\bin\Release\Microsoft.PlayerFramework.WebVTT.CaptionSettings.xml	Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.WebVTT.CaptionSettings\bin\Release\Microsoft.PlayerFramework.WebVTT.CaptionSettings.dll	Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.WebVTT.CaptionSettings\bin\Release\Microsoft.PlayerFramework.WebVTT.CaptionSettings.pri	Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT.CaptionSettings\

copy ..\Win8.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.dll					Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.xml					Microsoft.PlayerFramework.Xaml.CaptionSettings\References\CommonConfiguration\neutral\
copy ..\Win8.Xaml.CaptionSettings\bin\Release\Microsoft.PlayerFramework.CaptionSettings.pri					Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\
copy ..\Win8.Xaml.CaptionSettings\bin\Release\Controls\CaptionSettingsControl.xaml							Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Controls\
copy ..\Win8.Xaml.CaptionSettings\bin\Release\Controls\ColorPickerControl.xaml								Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Controls\
copy ..\Win8.Xaml.CaptionSettings\bin\Release\Controls\PreviewControl.xaml									Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Controls\
copy ..\Win8.Xaml.CaptionSettings\bin\Release\Themes\Generic.xaml											Microsoft.PlayerFramework.Xaml.CaptionSettings\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionSettings\Themes\

@popd

@echo.
@echo Done.
@echo.
