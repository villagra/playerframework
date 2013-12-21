@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd Microsoft.PlayerFramework.WP8
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Adaptive
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Adaptive.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.TimedText
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.TTML.Settings
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.TTML.Settings.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.WebVTT
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.WebVTT.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.WebVTT.Settings
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.WebVTT.Settings.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.CaptionSettings
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.CaptionSettings.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Advertising
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Analytics
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Analytics.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Dash
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Dash.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Analytics.Advertising
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Analytics.Advertising.zip "*"

@rem cd ..\Microsoft.AudienceInsight.WP8
@rem "%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.AudienceInsight.WP8.zip "*"

@rem cd ..\Microsoft.PlayerFramework.WP8.Analytics.AudienceInsight
@rem "%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Analytics.AudienceInsight.zip "*"


cd ..\Microsoft.PlayerFramework

move Microsoft.PlayerFramework.WP8.zip Microsoft.PlayerFramework.WP8.vsix
move Microsoft.PlayerFramework.WP8.Adaptive.zip Microsoft.PlayerFramework.WP8.Adaptive.vsix
move Microsoft.PlayerFramework.WP8.TimedText.zip Microsoft.PlayerFramework.WP8.TimedText.vsix
move Microsoft.PlayerFramework.WP8.TTML.Settings.zip Microsoft.PlayerFramework.WP8.TTML.Settings.vsix
move Microsoft.PlayerFramework.WP8.WebVTT.zip Microsoft.PlayerFramework.WP8.WebVTT.vsix
move Microsoft.PlayerFramework.WP8.WebVTT.Settings.zip Microsoft.PlayerFramework.WP8.WebVTT.Settings.vsix
move Microsoft.PlayerFramework.WP8.CaptionSettings.zip Microsoft.PlayerFramework.WP8.CaptionSettings.vsix
move Microsoft.PlayerFramework.WP8.Advertising.zip Microsoft.PlayerFramework.WP8.Advertising.vsix
move Microsoft.PlayerFramework.WP8.Analytics.zip Microsoft.PlayerFramework.WP8.Analytics.vsix
move Microsoft.PlayerFramework.WP8.Dash.zip Microsoft.PlayerFramework.WP8.Dash.vsix
move Microsoft.PlayerFramework.WP8.Analytics.Advertising.zip Microsoft.PlayerFramework.WP8.Analytics.Advertising.vsix
@rem move Microsoft.AudienceInsight.WP8.zip Microsoft.AudienceInsight.WP8.vsix
@rem move Microsoft.PlayerFramework.WP8.Analytics.AudienceInsight.zip Microsoft.PlayerFramework.WP8.Analytics.AudienceInsight.vsix

@popd

@echo.
@echo Done.
@echo.
