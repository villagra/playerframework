@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd Microsoft.PlayerFramework.Js.Win81
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Win81.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Win81.Adaptive
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Win81.Adaptive.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Win81.TimedText
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Win81.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Win81.Advertising
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Win81.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Win81.Analytics
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Win81.Analytics.zip "*"

cd ..\Microsoft.PlayerFramework.Win81.Dash
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Win81.Dash.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Win81
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Win81.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Win81.Adaptive
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Win81.Adaptive.zip "*"

@rem cd ..\Microsoft.PlayerFramework.Xaml.Win81.CaptionMarkers
@rem "%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Win81.CaptionMarkers.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Win81.TimedText
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Win81.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Win81.WebVTT
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Win81.WebVTT.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Win81.CaptionSettings
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Win81.CaptionSettings.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Win81.Advertising
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Win81.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Win81.Analytics
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Win81.Analytics.zip "*"

cd ..\Microsoft.PlayerFramework

move Microsoft.PlayerFramework.Js.Win81.zip Microsoft.PlayerFramework.Js.Win81.vsix
move Microsoft.PlayerFramework.Js.Win81.Adaptive.zip Microsoft.PlayerFramework.Js.Win81.Adaptive.vsix
move Microsoft.PlayerFramework.Js.Win81.TimedText.zip Microsoft.PlayerFramework.Js.Win81.TimedText.vsix
move Microsoft.PlayerFramework.Js.Win81.Advertising.zip Microsoft.PlayerFramework.Js.Win81.Advertising.vsix
move Microsoft.PlayerFramework.Js.Win81.Analytics.zip Microsoft.PlayerFramework.Js.Win81.Analytics.vsix
move Microsoft.PlayerFramework.Win81.Dash.zip Microsoft.PlayerFramework.Win81.Dash.vsix

move Microsoft.PlayerFramework.Xaml.Win81.zip Microsoft.PlayerFramework.Xaml.Win81.vsix
move Microsoft.PlayerFramework.Xaml.Win81.Adaptive.zip Microsoft.PlayerFramework.Xaml.Win81.Adaptive.vsix
move Microsoft.PlayerFramework.Xaml.Win81.TimedText.zip Microsoft.PlayerFramework.Xaml.Win81.TimedText.vsix
move Microsoft.PlayerFramework.Xaml.Win81.WebVTT.zip Microsoft.PlayerFramework.Xaml.Win81.WebVTT.vsix
move Microsoft.PlayerFramework.Xaml.Win81.CaptionSettings.zip Microsoft.PlayerFramework.Xaml.Win81.CaptionSettings.vsix
move Microsoft.PlayerFramework.Xaml.Win81.Advertising.zip Microsoft.PlayerFramework.Xaml.Win81.Advertising.vsix
move Microsoft.PlayerFramework.Xaml.Win81.Analytics.zip Microsoft.PlayerFramework.Xaml.Win81.Analytics.vsix
@rem move Microsoft.PlayerFramework.Xaml.Win81.CaptionMarkers.zip Microsoft.PlayerFramework.Xaml.Win81.CaptionMarkers.vsix

@popd

@echo.
@echo Done.
@echo.
