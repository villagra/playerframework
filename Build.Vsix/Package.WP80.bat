@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd WP80

cd Microsoft.PlayerFramework.Core
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Core.zip "*"

cd ..\Microsoft.PlayerFramework.Adaptive
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Adaptive.zip "*"

cd ..\Microsoft.PlayerFramework.TimedText
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.TTML.Settings
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.TTML.Settings.zip "*"

cd ..\Microsoft.PlayerFramework.WebVTT
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WebVTT.zip "*"

cd ..\Microsoft.PlayerFramework.WebVTT.Settings
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WebVTT.Settings.zip "*"

cd ..\Microsoft.PlayerFramework.CaptionSettings
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.CaptionSettings.zip "*"

cd ..\Microsoft.PlayerFramework.Advertising
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.Analytics
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Analytics.zip "*"

cd ..\Microsoft.PlayerFramework.Dash
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Dash.zip "*"

cd ..\Microsoft.PlayerFramework.Analytics.Advertising
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Analytics.Advertising.zip "*"

cd ..\Microsoft.AudienceInsight
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.AudienceInsight.zip "*"

cd ..\Microsoft.PlayerFramework.Analytics.AudienceInsight
"%ZIP%" a ..\..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Analytics.AudienceInsight.zip "*"


cd ..\..\Microsoft.PlayerFramework

move Microsoft.PlayerFramework.Core.zip Microsoft.PlayerFramework.Core.WP80.vsix
move Microsoft.PlayerFramework.Adaptive.zip Microsoft.PlayerFramework.Adaptive.WP80.vsix
move Microsoft.PlayerFramework.TimedText.zip Microsoft.PlayerFramework.TimedText.WP80.vsix
move Microsoft.PlayerFramework.TTML.Settings.zip Microsoft.PlayerFramework.TTML.Settings.WP80.vsix
move Microsoft.PlayerFramework.WebVTT.zip Microsoft.PlayerFramework.WebVTT.WP80.vsix
move Microsoft.PlayerFramework.WebVTT.Settings.zip Microsoft.PlayerFramework.WebVTT.Settings.WP80.vsix
move Microsoft.PlayerFramework.CaptionSettings.zip Microsoft.PlayerFramework.CaptionSettings.WP80.vsix
move Microsoft.PlayerFramework.Advertising.zip Microsoft.PlayerFramework.Advertising.WP80.vsix
move Microsoft.PlayerFramework.Analytics.zip Microsoft.PlayerFramework.Analytics.WP80.vsix
move Microsoft.PlayerFramework.Dash.zip Microsoft.PlayerFramework.Dash.WP80.vsix
move Microsoft.PlayerFramework.Analytics.Advertising.zip Microsoft.PlayerFramework.Analytics.Advertising.WP80.vsix
move Microsoft.AudienceInsight.zip Microsoft.AudienceInsight.WP80.vsix
move Microsoft.PlayerFramework.Analytics.AudienceInsight.zip Microsoft.PlayerFramework.Analytics.AudienceInsight.WP80.vsix

@popd

@echo.
@echo Done.
@echo.
