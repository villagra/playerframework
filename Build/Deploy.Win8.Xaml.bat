@pushd %~dp0%

rmdir /s /q "Microsoft.PlayerFramework.Xaml\Redist"
rmdir /s /q "Microsoft.PlayerFramework.Xaml\References"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.Adaptive\Redist"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.Adaptive\References"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.Advertising\Redist"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.Advertising\References"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.Analytics\Redist"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.Analytics\References"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.CaptionMarkers\References"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.TimedText\Redist"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.TimedText\References"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.WebVTT\Redist"
rmdir /s /q "Microsoft.PlayerFramework.Xaml.WebVTT\References"

pause

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
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.TimedText"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.TimedText\themes"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.WebVTT"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.WebVTT\themes"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral"

cd..

copy Win8.Xaml\bin\Release\Microsoft.PlayerFramework.dll  Build\Microsoft.PlayerFramework.Xaml\References\CommonConfiguration\neutral\
copy Win8.Xaml\bin\Release\Microsoft.PlayerFramework.xml  Build\Microsoft.PlayerFramework.Xaml\References\CommonConfiguration\neutral\
copy Win8.Xaml\bin\Release\Microsoft.PlayerFramework.pri  Build\Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\
copy Win8.Xaml\bin\Release\Themes\generic.xaml            Build\Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy Win8.Xaml\bin\Release\Themes\EntertainmentTheme.xaml Build\Microsoft.PlayerFramework.Xaml\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes

copy Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.pri					Build\Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Adaptive\
copy Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.AdaptiveStreaming.pri						    Build\Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming\

copy Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.dll					Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.xml					Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
copy Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.AdaptiveStreaming.winmd 					    Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
@rem copy Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.AdaptiveStreaming.xml				    Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\

copy Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Adaptive.dll					Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
copy Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Adaptive.xml					Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
copy Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.AdaptiveStreaming.winmd 					    Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
@rem copy Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.AdaptiveStreaming.xml				    Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\

copy Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.dll					Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
copy Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.xml					Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
copy Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.AdaptiveStreaming.winmd 					    Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
@rem copy Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.AdaptiveStreaming.xml				    Build\Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\

@rem copy Win8.Xaml.CaptionMarkers\bin\Release\Microsoft.PlayerFramework.CaptionMarkers.dll Build\Microsoft.PlayerFramework.Xaml.CaptionMarkers\References\CommonConfiguration\neutral\
@rem copy Win8.Xaml.CaptionMarkers\bin\Release\Microsoft.PlayerFramework.CaptionMarkers.xml Build\Microsoft.PlayerFramework.Xaml.CaptionMarkers\References\CommonConfiguration\neutral\
@rem copy Win8.Xaml.CaptionMarkers\bin\Release\Microsoft.PlayerFramework.CaptionMarkers.pri Build\Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionMarkers\
@rem copy Win8.Xaml.CaptionMarkers\bin\Release\Themes\generic.xaml                          Build\Microsoft.PlayerFramework.Xaml.CaptionMarkers\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.CaptionMarkers\themes\

copy Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll Build\Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.xml Build\Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy Win8.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.pri Build\Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TimedText\
copy Win8.Xaml.TimedText\bin\Release\Microsoft.TimedText.dll				 Build\Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
@rem copy Win8.Xaml.TimedText\bin\Release\Microsoft.TimedText.xml			 Build\Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy Win8.Xaml.TimedText\bin\Release\Microsoft.TimedText.pri				 Build\Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.TimedText\
copy Win8.TimedText\bin\Release\Themes\generic.xaml							 Build\Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.TimedText\themes\

copy Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll		Build\Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.xml		Build\Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy Win8.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.pri		Build\Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\
copy Win8.Xaml.Advertising\bin\Release\Themes\generic.xaml								Build\Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\themes\
copy Win8.Xaml.Advertising\bin\Release\Microsoft.VideoAdvertising.winmd 				Build\Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
@rem copy Win8.Xaml.Advertising\bin\Release\Microsoft.VideoAdvertising.xml				Build\Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy Win8.Xaml.Advertising\bin\Release\Microsoft.VideoAdvertising.pri					Build\Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.VideoAdvertising\

copy Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll	Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml	Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy Win8.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.pri	Build\Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Analytics\
copy Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.winmd 			Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
@rem copy Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.xml			Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.pri				Build\Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.VideoAnalytics\

copy Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.pri					Build\Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.AdaptiveStreaming.Analytics\
copy Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.winmd 				Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86\
@rem copy Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.xml			Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86\
copy Win8.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.AdaptiveStreaming.Analytics.winmd 				Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x64\
@rem copy Win8.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.AdaptiveStreaming.Analytics.xml			Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x64\
copy Win8.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.AdaptiveStreaming.Analytics.winmd 				Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM\
@rem copy Win8.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.AdaptiveStreaming.Analytics.xml			Build\Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM\

copy Win8.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll		Build\Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
@rem copy Win8.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.xml	Build\Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy Win8.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.pri		Build\Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT\
copy Win8.Xaml.WebVTT\bin\Release\Microsoft.WebVTT.winmd					Build\Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
@rem copy Win8.Xaml.WebVTT\bin\Release\Microsoft.WebVTT.xml					Build\Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy Win8.Xaml.WebVTT\bin\Release\Microsoft.WebVTT.pri						Build\Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.WebVTT\
copy Win8.WebVTT\bin\Release\Themes\generic.xaml							Build\Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.WebVTT\themes\

@popd

@echo.
@echo Done.
@echo.
@pause
