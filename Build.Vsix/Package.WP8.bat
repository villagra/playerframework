@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd Microsoft.PlayerFramework.WP8
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Adaptive
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Adaptive.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.TimedText
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.WebVTT
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.WebVTT.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Advertising
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Analytics
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Analytics.zip "*"

cd ..\Microsoft.PlayerFramework.WP8.Dash
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.WP8.Dash.zip "*"


cd ..\Microsoft.PlayerFramework

move Microsoft.PlayerFramework.WP8.zip Microsoft.PlayerFramework.WP8.vsix
move Microsoft.PlayerFramework.WP8.Adaptive.zip Microsoft.PlayerFramework.WP8.Adaptive.vsix
move Microsoft.PlayerFramework.WP8.TimedText.zip Microsoft.PlayerFramework.WP8.TimedText.vsix
move Microsoft.PlayerFramework.WP8.WebVTT.zip Microsoft.PlayerFramework.WP8.WebVTT.vsix
move Microsoft.PlayerFramework.WP8.Advertising.zip Microsoft.PlayerFramework.WP8.Advertising.vsix
move Microsoft.PlayerFramework.WP8.Analytics.zip Microsoft.PlayerFramework.WP8.Analytics.vsix
move Microsoft.PlayerFramework.WP8.Dash.zip Microsoft.PlayerFramework.WP8.Dash.vsix

@popd

@echo.
@echo Done.
@echo.
