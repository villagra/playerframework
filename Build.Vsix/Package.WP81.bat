@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd WP81

cd Microsoft.PlayerFramework.Dash
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Dash.zip "*"

cd ..\Microsoft.PlayerFramework.Analytics.Advertising
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Analytics.Advertising.zip "*"

cd ..\Microsoft.Media.AudienceInsight
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.Media.AudienceInsight.zip "*"

cd ..\Microsoft.PlayerFramework.Analytics.AudienceInsight
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Analytics.AudienceInsight.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.zip "*"

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

move Microsoft.PlayerFramework.Dash.zip Microsoft.PlayerFramework.Dash.WP81.vsix
move Microsoft.PlayerFramework.Analytics.Advertising.zip Microsoft.PlayerFramework.Analytics.Advertising.WP81.vsix
move Microsoft.PlayerFramework.Analytics.AudienceInsight.zip Microsoft.PlayerFramework.Analytics.AudienceInsight.WP81.vsix
move Microsoft.Media.AudienceInsight.zip Microsoft.Media.AudienceInsight.WP81.vsix

move Microsoft.PlayerFramework.Xaml.zip Microsoft.PlayerFramework.Xaml.WP81.vsix
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
