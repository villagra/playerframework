@pushd %~dp0%

rmdir /s /q "Microsoft.PlayerFramework.WP8\References"
rmdir /s /q "Microsoft.PlayerFramework.WP8.Adaptive\References"
rmdir /s /q "Microsoft.PlayerFramework.WP8.Advertising\References"
rmdir /s /q "Microsoft.PlayerFramework.WP8.Analytics\References"
rmdir /s /q "Microsoft.PlayerFramework.WP8.TimedText\References"
rmdir /s /q "Microsoft.PlayerFramework.WP8.WebVTT\References"
rmdir /s /q "Microsoft.PlayerFramework.WP8.Dash\References"

mkdir "Microsoft.PlayerFramework.WP8\References"
mkdir "Microsoft.PlayerFramework.WP8\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.Adaptive\References"
mkdir "Microsoft.PlayerFramework.WP8.Adaptive\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.Adaptive\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.Advertising\References"
mkdir "Microsoft.PlayerFramework.WP8.Advertising\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.Advertising\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.Analytics\References"
mkdir "Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.TimedText\References"
mkdir "Microsoft.PlayerFramework.WP8.TimedText\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.TimedText\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.WebVTT\References"
mkdir "Microsoft.PlayerFramework.WP8.WebVTT\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.WebVTT\References\CommonConfiguration\neutral"
mkdir "Microsoft.PlayerFramework.WP8.Dash\References"
mkdir "Microsoft.PlayerFramework.WP8.Dash\References\CommonConfiguration"
mkdir "Microsoft.PlayerFramework.WP8.Dash\References\CommonConfiguration\neutral"

cd..

copy Phone.SL\bin\Release\Microsoft.PlayerFramework.dll  Build\Microsoft.PlayerFramework.WP8\References\CommonConfiguration\neutral\
copy Phone.SL\bin\Release\Microsoft.PlayerFramework.xml  Build\Microsoft.PlayerFramework.WP8\References\CommonConfiguration\neutral\

copy Phone.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.dll  Build\Microsoft.PlayerFramework.WP8.Adaptive\References\CommonConfiguration\neutral\
copy Phone.SL.Adaptive\bin\Release\Microsoft.PlayerFramework.Adaptive.xml  Build\Microsoft.PlayerFramework.WP8.Adaptive\References\CommonConfiguration\neutral\

copy Phone.SL.TimedText\bin\Release\Microsoft.PlayerFramework.TimedText.dll  Build\Microsoft.PlayerFramework.WP8.TimedText\References\CommonConfiguration\neutral\
copy Phone.TimedText\bin\Release\Microsoft.TimedText.dll					 Build\Microsoft.PlayerFramework.WP8.TimedText\References\CommonConfiguration\neutral\

copy Phone.SL.Advertising\bin\Release\Microsoft.PlayerFramework.Advertising.dll  Build\Microsoft.PlayerFramework.WP8.Advertising\References\CommonConfiguration\neutral\
copy Phone.VideoAdvertising\bin\Release\Microsoft.VideoAdvertising.dll			 Build\Microsoft.PlayerFramework.WP8.Advertising\References\CommonConfiguration\neutral\

copy Phone.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.dll					      Build\Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy Phone.SL.Analytics\bin\Release\Microsoft.PlayerFramework.Analytics.xml					      Build\Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy Phone.SL.Adaptive.Analytics\bin\Release\Microsoft.PlayerFramework.Adaptive.Analytics.dll     Build\Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy Phone.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.dll								  Build\Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy Phone.VideoAnalytics\bin\Release\Microsoft.VideoAnalytics.xml								  Build\Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\
copy Lib\Portable\ZLib\bin\Release\ZLib.dll														  Build\Microsoft.PlayerFramework.WP8.Analytics\References\CommonConfiguration\neutral\

copy Phone.SL.WebVTT\bin\Release\Microsoft.PlayerFramework.WebVTT.dll  Build\Microsoft.PlayerFramework.WP8.WebVTT\References\CommonConfiguration\neutral\
copy Phone.WebVTT\bin\Release\Microsoft.WebVTT.dll					 Build\Microsoft.PlayerFramework.WP8.WebVTT\References\CommonConfiguration\neutral\

copy Phone.SL.Adaptive.Dash\bin\Release\Microsoft.PlayerFramework.Adaptive.Dash.dll		Build\Microsoft.PlayerFramework.WP8.Dash\References\CommonConfiguration\neutral\
copy Lib\Portable\Microsoft.Media.ISO\bin\Release\Microsoft.Media.ISO.dll				Build\Microsoft.PlayerFramework.WP8.Dash\References\CommonConfiguration\neutral\

@popd

@echo.
@echo Done.
@echo.
@pause
