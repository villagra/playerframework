@pushd %~dp0%

.nuget\nuget pack Microsoft.PlayerFramework\Microsoft.PlayerFramework.nuspec
.nuget\nuget pack Microsoft.PlayerFramework.Adaptive\Microsoft.PlayerFramework.Adaptive.nuspec
.nuget\nuget pack Microsoft.PlayerFramework.Adaptive.Dash\Microsoft.PlayerFramework.Adaptive.Dash.nuspec
.nuget\nuget pack Microsoft.PlayerFramework.Advertising\Microsoft.PlayerFramework.Advertising.nuspec
.nuget\nuget pack Microsoft.PlayerFramework.Analytics\Microsoft.PlayerFramework.Analytics.nuspec
.nuget\nuget pack Microsoft.PlayerFramework.TimedText\Microsoft.PlayerFramework.TimedText.nuspec
.nuget\nuget pack Microsoft.PlayerFramework.WebVTT\Microsoft.PlayerFramework.WebVTT.nuspec

@popd

@echo.
@echo Done.
@echo.
@pause
