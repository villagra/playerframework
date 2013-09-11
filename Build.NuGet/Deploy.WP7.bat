@pushd %~dp0%

cd..

@set DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\devenv.exe
@IF NOT EXIST "%DEVENV%" SET DEVENV=%ProgramFiles(x86)%\Microsoft Visual Studio 11.0\Common7\IDE\VSWinExpress.exe

"%DEVENV%" /rebuild "Release|AnyCPU" Microsoft.WP7.PlayerFramework.SL.sln

copy Phone.SL\bin\Release\Microsoft.PlayerFramework.dll											Build.NuGet\Microsoft.PlayerFramework\lib\wp71\
copy Phone.SL\bin\Release\Microsoft.PlayerFramework.xml											Build.NuGet\Microsoft.PlayerFramework\lib\wp71\

copy Phone.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.dll						Build.NuGet\Microsoft.PlayerFramework.Adaptive\lib\wp71\
copy Phone.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.xml						Build.NuGet\Microsoft.PlayerFramework.Adaptive\lib\wp71\

copy Phone.SL.Adaptive.Dash\bin\Release\Microsoft.PlayerFramework.Adaptive.Dash.dll				Build.NuGet\Microsoft.PlayerFramework.Adaptive.Dash\lib\wp71\

copy Phone.SL.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll						Build.NuGet\Microsoft.PlayerFramework.TimedText\lib\wp71\
copy Phone.TimedText\bin\Release\Microsoft.TimedText.dll										Build.NuGet\Microsoft.PlayerFramework.TimedText\lib\wp71\

copy Phone.SL.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll					Build.NuGet\Microsoft.PlayerFramework.Advertising\lib\wp71\
copy Phone.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.dll							Build.NuGet\Microsoft.PlayerFramework.Advertising\lib\wp71\

copy Phone.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll						Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\wp71\
copy Phone.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml					    Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\wp71\
copy Phone.SL.Adaptive.Analytics\bin\Release\Microsoft.PlayerFramework.Adaptive.Analytics.dll   Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\wp71\
copy Phone.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.dll								Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\wp71\
copy Phone.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.xml								Build.NuGet\Microsoft.PlayerFramework.Analytics\lib\wp71\

copy Phone.AudienceInsight\bin\Release\Microsoft.AudienceInsight.dll   Build.NuGet\Microsoft.AudienceInsight\lib\wp71\
copy Phone.VideoAnalytics.VideoAdvertising\bin\Release\Microsoft.VideoAnalytics.VideoAdvertising.dll   Build.NuGet\Microsoft.Analytics.Advertising\lib\wp71\
copy Phone.VideoAnalytics.AudienceInsight\bin\Release\Microsoft.VideoAnalytics.AudienceInsight.dll   Build.NuGet\Microsoft.Analytics.AudienceInsight\lib\wp71\

copy Phone.SL.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll							Build.NuGet\Microsoft.PlayerFramework.WebVTT\lib\wp71\
copy Phone.WebVTT\bin\Release\Microsoft.WebVTT.dll												Build.NuGet\Microsoft.PlayerFramework.WebVTT\lib\wp71\

@popd

@echo.
@echo Done.
@echo.
@pause
