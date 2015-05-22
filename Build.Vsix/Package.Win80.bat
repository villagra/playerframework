@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd Win80

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

cd ..\Microsoft.PlayerFramework.Dash
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

move Microsoft.PlayerFramework.Js.Core.zip Microsoft.PlayerFramework.Js.Core.Win80.vsix
move Microsoft.PlayerFramework.Js.Adaptive.zip Microsoft.PlayerFramework.Js.Adaptive.Win80.vsix
move Microsoft.PlayerFramework.Js.TimedText.zip Microsoft.PlayerFramework.Js.TimedText.Win80.vsix
move Microsoft.PlayerFramework.Js.Advertising.zip Microsoft.PlayerFramework.Js.Advertising.Win80.vsix
move Microsoft.PlayerFramework.Js.Analytics.zip Microsoft.PlayerFramework.Js.Analytics.Win80.vsix
move Microsoft.PlayerFramework.Dash.zip Microsoft.PlayerFramework.Dash.Win80.vsix
move Microsoft.PlayerFramework.Analytics.Advertising.zip Microsoft.PlayerFramework.Analytics.Advertising.Win80.vsix
move Microsoft.PlayerFramework.Analytics.AudienceInsight.zip Microsoft.PlayerFramework.Analytics.AudienceInsight.Win80.vsix
move Microsoft.Media.AudienceInsight.zip Microsoft.Media.AudienceInsight.Win80.vsix

move Microsoft.PlayerFramework.Xaml.Core.zip Microsoft.PlayerFramework.Xaml.Core.Win80.vsix
move Microsoft.PlayerFramework.Xaml.Adaptive.zip Microsoft.PlayerFramework.Xaml.Adaptive.Win80.vsix
move Microsoft.PlayerFramework.Xaml.TimedText.zip Microsoft.PlayerFramework.Xaml.TimedText.Win80.vsix
move Microsoft.PlayerFramework.Xaml.TTML.Settings.zip Microsoft.PlayerFramework.Xaml.TTML.Settings.Win80.vsix
move Microsoft.PlayerFramework.Xaml.WebVTT.zip Microsoft.PlayerFramework.Xaml.WebVTT.Win80.vsix
move Microsoft.PlayerFramework.Xaml.WebVTT.Settings.zip Microsoft.PlayerFramework.Xaml.WebVTT.Settings.Win80.vsix
move Microsoft.PlayerFramework.Xaml.CaptionSettings.zip Microsoft.PlayerFramework.Xaml.CaptionSettings.Win80.vsix
move Microsoft.PlayerFramework.Xaml.Advertising.zip Microsoft.PlayerFramework.Xaml.Advertising.Win80.vsix
move Microsoft.PlayerFramework.Xaml.Analytics.zip Microsoft.PlayerFramework.Xaml.Analytics.Win80.vsix

@popd

@echo.
@echo Done.
@echo.
