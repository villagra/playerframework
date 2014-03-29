@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd WP81

cd Microsoft.PlayerFramework.Js.Core
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Core.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Adaptive
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Adaptive.zip "*"

cd ..\Microsoft.PlayerFramework.Js.TimedText
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Advertising
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Analytics
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Analytics.zip "*"

cd Microsoft.PlayerFramework.Dash
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Dash.zip "*"

cd ..\Microsoft.PlayerFramework.Analytics.Advertising
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Analytics.Advertising.zip "*"

cd ..\Microsoft.Media.AudienceInsight
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.Media.AudienceInsight.zip "*"

cd ..\Microsoft.PlayerFramework.Analytics.AudienceInsight
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Analytics.AudienceInsight.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Core
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Core.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Adaptive
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Adaptive.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.TimedText
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.TTML.Settings
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.TTML.Settings.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.WebVTT
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.WebVTT.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.WebVTT.Settings
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.WebVTT.Settings.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.CaptionSettings
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.CaptionSettings.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Advertising
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Analytics
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Analytics.zip "*"

cd ..\..\Microsoft.PlayerFramework

move Microsoft.PlayerFramework.Js.Core.zip Microsoft.PlayerFramework.Js.Core.WP81.vsix
move Microsoft.PlayerFramework.Js.Adaptive.zip Microsoft.PlayerFramework.Js.Adaptive.WP81.vsix
move Microsoft.PlayerFramework.Js.TimedText.zip Microsoft.PlayerFramework.Js.TimedText.WP81.vsix
move Microsoft.PlayerFramework.Js.Advertising.zip Microsoft.PlayerFramework.Js.Advertising.WP81.vsix
move Microsoft.PlayerFramework.Js.Analytics.zip Microsoft.PlayerFramework.Js.Analytics.WP81.vsix
move Microsoft.PlayerFramework.Dash.zip Microsoft.PlayerFramework.Dash.WP81.vsix
move Microsoft.PlayerFramework.Analytics.Advertising.zip Microsoft.PlayerFramework.Analytics.Advertising.WP81.vsix
move Microsoft.PlayerFramework.Analytics.AudienceInsight.zip Microsoft.PlayerFramework.Analytics.AudienceInsight.WP81.vsix
move Microsoft.Media.AudienceInsight.zip Microsoft.Media.AudienceInsight.WP81.vsix

move Microsoft.PlayerFramework.Xaml.Core.zip Microsoft.PlayerFramework.Xaml.Core.WP81.vsix
move Microsoft.PlayerFramework.Xaml.Adaptive.zip Microsoft.PlayerFramework.Xaml.Adaptive.WP81.vsix
move Microsoft.PlayerFramework.Xaml.TimedText.zip Microsoft.PlayerFramework.Xaml.TimedText.WP81.vsix
move Microsoft.PlayerFramework.Xaml.TTML.Settings.zip Microsoft.PlayerFramework.Xaml.TTML.Settings.WP81.vsix
move Microsoft.PlayerFramework.Xaml.WebVTT.zip Microsoft.PlayerFramework.Xaml.WebVTT.WP81.vsix
move Microsoft.PlayerFramework.Xaml.WebVTT.Settings.zip Microsoft.PlayerFramework.Xaml.WebVTT.Settings.WP81.vsix
move Microsoft.PlayerFramework.Xaml.CaptionSettings.zip Microsoft.PlayerFramework.Xaml.CaptionSettings.WP81.vsix
move Microsoft.PlayerFramework.Xaml.Advertising.zip Microsoft.PlayerFramework.Xaml.Advertising.WP81.vsix
move Microsoft.PlayerFramework.Xaml.Analytics.zip Microsoft.PlayerFramework.Xaml.Analytics.WP81.vsix

@popd

@echo.
@echo Done.
@echo.
