@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd Microsoft.PlayerFramework.Js
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Adaptive
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Adaptive.zip "*"

cd ..\Microsoft.PlayerFramework.Js.TimedText
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Advertising
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.Js.Analytics
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Js.Analytics.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Adaptive
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Adaptive.zip "*"

@rem cd ..\Microsoft.PlayerFramework.Xaml.CaptionMarkers
@rem "%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.CaptionMarkers.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.TimedText
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.TimedText.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.WebVTT
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.WebVTT.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Advertising
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Advertising.zip "*"

cd ..\Microsoft.PlayerFramework.Xaml.Analytics
"%ZIP%" a ..\Microsoft.PlayerFramework\Microsoft.PlayerFramework.Xaml.Analytics.zip "*"

cd ..\Microsoft.PlayerFramework.WP8
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


cd ..\Microsoft.PlayerFramework
move Microsoft.PlayerFramework.Js.zip Microsoft.PlayerFramework.Js.vsix
move Microsoft.PlayerFramework.Js.Adaptive.zip Microsoft.PlayerFramework.Js.Adaptive.vsix
move Microsoft.PlayerFramework.Js.TimedText.zip Microsoft.PlayerFramework.Js.TimedText.vsix
move Microsoft.PlayerFramework.Js.Advertising.zip Microsoft.PlayerFramework.Js.Advertising.vsix
move Microsoft.PlayerFramework.Js.Analytics.zip Microsoft.PlayerFramework.Js.Analytics.vsix

move Microsoft.PlayerFramework.Xaml.zip Microsoft.PlayerFramework.Xaml.vsix
move Microsoft.PlayerFramework.Xaml.Adaptive.zip Microsoft.PlayerFramework.Xaml.Adaptive.vsix
move Microsoft.PlayerFramework.Xaml.TimedText.zip Microsoft.PlayerFramework.Xaml.TimedText.vsix
move Microsoft.PlayerFramework.Xaml.WebVTT.zip Microsoft.PlayerFramework.Xaml.WebVTT.vsix
move Microsoft.PlayerFramework.Xaml.Advertising.zip Microsoft.PlayerFramework.Xaml.Advertising.vsix
move Microsoft.PlayerFramework.Xaml.Analytics.zip Microsoft.PlayerFramework.Xaml.Analytics.vsix
@rem move Microsoft.PlayerFramework.Xaml.CaptionMarkers.zip Microsoft.PlayerFramework.Xaml.CaptionMarkers.vsix

move Microsoft.PlayerFramework.WP8.zip Microsoft.PlayerFramework.WP8.vsix
move Microsoft.PlayerFramework.WP8.Adaptive.zip Microsoft.PlayerFramework.WP8.Adaptive.vsix
move Microsoft.PlayerFramework.WP8.TimedText.zip Microsoft.PlayerFramework.WP8.TimedText.vsix
move Microsoft.PlayerFramework.WP8.WebVTT.zip Microsoft.PlayerFramework.WP8.WebVTT.vsix
move Microsoft.PlayerFramework.WP8.Advertising.zip Microsoft.PlayerFramework.WP8.Advertising.vsix
move Microsoft.PlayerFramework.WP8.Analytics.zip Microsoft.PlayerFramework.WP8.Analytics.vsix

"%ZIP%" a ..\Microsoft.PlayerFramework.zip "*"

cd ..\
move Microsoft.PlayerFramework.zip Microsoft.PlayerFramework.vsix

@popd

@echo.
@echo Done.
@echo.
@pause
