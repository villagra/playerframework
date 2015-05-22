@set SN=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v10.0A\Bin\NETFX 4.6 Tools\x64\sn.exe
@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\Common7\IDE\VSWinExpress.exe

@pushd %~dp0%
cd..

"%DEVENV%" /build "Release|AnyCPU" Microsoft.PlayerFramework.UWP.Xaml.sln
"%DEVENV%" /build "Release|x86"    Microsoft.PlayerFramework.UWP.Xaml.sln
"%DEVENV%" /build "Release|x64"    Microsoft.PlayerFramework.UWP.Xaml.sln
"%DEVENV%" /build "Release|ARM"    Microsoft.PlayerFramework.UWP.Xaml.sln

@rem "%SN%" -R UWP.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 							UWP.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
@rem "%SN%" -R UWP.Xaml.Adaptive\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 							UWP.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
@rem "%SN%" -R UWP.Xaml.Adaptive\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 							UWP.WinRT.AdaptiveStreaming.Helper\Microsoft.PlayerFramework.snk
"%SN%" -R UWP.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.winmd 										UWP.WinRT.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R UWP.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd 										UWP.WinRT.Analytics\Microsoft.PlayerFramework.snk
@rem "%SN%" -R UWP.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 	UWP.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
@rem "%SN%" -R UWP.WinRT.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 	UWP.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
@rem "%SN%" -R UWP.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 	UWP.WinRT.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R UWP.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.winmd												UWP.WinRT.WebVTT\Microsoft.PlayerFramework.snk

@popd

@pushd %~dp0%

cd Win10

mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.Core\References"
mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework"
mkdir "Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes"
mkdir "Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Core\DesignTime\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.Core\DesignTime\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.Core\DesignTime\CommonConfiguration\neutral\themes"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Helper"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Adaptive"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86"
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
@rem mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Analytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Analytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics"
mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration"
@rem mkdir "Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM"
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
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT\themes"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral"

copy ..\..\UWP.Xaml.Core\bin\Release\Microsoft.PlayerFramework.dll								Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.Core\bin\Release\Microsoft.PlayerFramework.xml								Microsoft.PlayerFramework.Xaml.Core\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.Core\bin\Release\Microsoft.PlayerFramework.pri								Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\
copy ..\..\UWP.Xaml.Core\bin\Release\Themes\Generic.xbf											Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\UWP.Xaml.Core\bin\Release\Themes\Entertainment.xbf									Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\UWP.Xaml.Core\bin\Release\Themes\Phone.xbf											Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\UWP.Xaml.Core\bin\Release\Themes\Classic.xbf											Microsoft.PlayerFramework.Xaml.Core\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework\Themes
copy ..\..\Design\UWP.Xaml.Core.Design\bin\Release\Microsoft.PlayerFramework.Design.dll			Microsoft.PlayerFramework.Xaml.Core\DesignTime\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.Core\Themes\Generic.xaml													Microsoft.PlayerFramework.Xaml.Core\DesignTime\CommonConfiguration\neutral\Themes
copy ..\..\UWP.Xaml.Core\Themes\Entertainment.xaml												Microsoft.PlayerFramework.Xaml.Core\DesignTime\CommonConfiguration\neutral\Themes
copy ..\..\UWP.Xaml.Core\Themes\Phone.xaml														Microsoft.PlayerFramework.Xaml.Core\DesignTime\CommonConfiguration\neutral\Themes
copy ..\..\UWP.Xaml.Core\Themes\Classic.xaml													Microsoft.PlayerFramework.Xaml.Core\DesignTime\CommonConfiguration\neutral\Themes

@rem copy ..\..\UWP.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.pri					Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Adaptive\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.pri			Microsoft.PlayerFramework.Xaml.Adaptive\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Helper\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 		Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x86\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 		Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\x64\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.dll					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Adaptive.xml					Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\
@rem copy ..\..\UWP.Xaml.Adaptive\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Helper.winmd 		Microsoft.PlayerFramework.Xaml.Adaptive\References\CommonConfiguration\ARM\

copy ..\..\UWP.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll					Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.xml					Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.pri					Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.TimedText\
copy ..\..\UWP.Xaml.TimedText\bin\Release\Microsoft.Media.TimedText.dll								Microsoft.PlayerFramework.Xaml.TimedText\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.TimedText\bin\Release\Microsoft.Media.TimedText.pri								Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.Media.TimedText\
copy ..\..\UWP.WinRT.TimedText\bin\Release\Themes\generic.xbf										Microsoft.PlayerFramework.Xaml.TimedText\Redist\CommonConfiguration\neutral\Microsoft.Media.TimedText\themes\

copy ..\..\UWP.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll				Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.xml				Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.pri				Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\
copy ..\..\UWP.Xaml.Advertising\bin\Release\Themes\generic.xbf										Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Advertising\themes\
copy ..\..\UWP.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.winmd 						Microsoft.PlayerFramework.Xaml.Advertising\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.Advertising\bin\Release\Microsoft.Media.Advertising.pri							Microsoft.PlayerFramework.Xaml.Advertising\Redist\CommonConfiguration\neutral\Microsoft.Media.Advertising\

copy ..\..\UWP.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll					Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml					Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.pri					Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.Analytics\
copy ..\..\UWP.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.winmd 							Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\neutral\
copy ..\..\UWP.WinRT.Analytics\bin\Release\Microsoft.Media.Analytics.pri							Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.Analytics\

@rem copy ..\..\UWP.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.pri			Microsoft.PlayerFramework.Xaml.Analytics\Redist\CommonConfiguration\neutral\Microsoft.Media.AdaptiveStreaming.Analytics\
@rem copy ..\..\UWP.WinRT.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x86\
@rem copy ..\..\UWP.WinRT.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\x64\
@rem copy ..\..\UWP.WinRT.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.Media.AdaptiveStreaming.Analytics.winmd 			Microsoft.PlayerFramework.Xaml.Analytics\References\CommonConfiguration\ARM\

copy ..\..\UWP.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll						Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.pri						Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.PlayerFramework.WebVTT\
copy ..\..\UWP.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.winmd								Microsoft.PlayerFramework.Xaml.WebVTT\References\CommonConfiguration\neutral\
copy ..\..\UWP.Xaml.WebVTT\bin\Release\Microsoft.Media.WebVTT.pri								Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT\
copy ..\..\UWP.WinRT.WebVTT\bin\Release\Themes\generic.xbf										Microsoft.PlayerFramework.Xaml.WebVTT\Redist\CommonConfiguration\neutral\Microsoft.Media.WebVTT\themes\

@popd

@echo.
@echo Done.
@echo.