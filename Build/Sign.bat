@pushd %~dp0%

@set SN=%ProgramFiles(x86)%\Microsoft SDKs\Windows\v8.0A\Bin\NETFX 4.0 Tools\x64\sn.exe

cd..

"%SN%" -R Win8.Xaml.Adaptive\bin\x86\Release\Microsoft.AdaptiveStreaming.winmd							Win8.Xaml.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Adaptive\bin\x64\Release\Microsoft.AdaptiveStreaming.winmd							Win8.Xaml.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Adaptive\bin\ARM\Release\Microsoft.AdaptiveStreaming.winmd							Win8.Xaml.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.Advertising\bin\Release\Microsoft.VideoAdvertising.winmd							Win8.Xaml.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Xaml.WebVTT\bin\Release\Microsoft.WebVTT.winmd											Win8.Xaml.WebVTT\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\x86\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\x64\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Analytics\bin\ARM\Release\Microsoft.AdaptiveStreaming.Analytics.winmd	Win8.AdaptiveStreaming.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\x86\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\x64\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming\bin\ARM\Release\Microsoft.AdaptiveStreaming.winmd						Win8.AdaptiveStreaming\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.winmd								Win8.VideoAnalytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.winmd							Win8.VideoAdvertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Analytics\bin\Release\Microsoft.PlayerFramework.Js.Analytics.winmd					Win8.Js.Analytics\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Advertising\bin\Release\Microsoft.PlayerFramework.Js.Advertising.winmd				Win8.Js.Advertising\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Adaptive\bin\ARM\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd					Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Adaptive\bin\x86\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd					Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.Js.Adaptive\bin\x64\Release\Microsoft.PlayerFramework.Js.Adaptive.winmd					Win8.Js.Adaptive\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Dash\bin\x86\Release\Microsoft.AdaptiveStreaming.Dash.winmd			Win8.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Dash\bin\x64\Release\Microsoft.AdaptiveStreaming.Dash.winmd			Win8.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk
"%SN%" -R Win8.AdaptiveStreaming.Dash\bin\ARM\Release\Microsoft.AdaptiveStreaming.Dash.winmd			Win8.AdaptiveStreaming.Dash\Microsoft.PlayerFramework.snk

@popd

@echo.
@echo Done.
@echo.
@pause
