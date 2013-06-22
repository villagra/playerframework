@pushd %~dp0%

rmdir /s /q "Microsoft.PlayerFramework\lib"
rmdir /s /q "Microsoft.PlayerFramework.Adaptive\lib"
rmdir /s /q "Microsoft.PlayerFramework.Advertising\lib"
rmdir /s /q "Microsoft.PlayerFramework.Analytics\lib"
rmdir /s /q "Microsoft.PlayerFramework.TimedText\lib"
rmdir /s /q "Microsoft.PlayerFramework.WebVTT\lib"

mkdir "Microsoft.PlayerFramework\lib"
mkdir "Microsoft.PlayerFramework\lib\wp71"
mkdir "Microsoft.PlayerFramework\lib\windowsphone8"
mkdir "Microsoft.PlayerFramework.Adaptive\lib"
mkdir "Microsoft.PlayerFramework.Adaptive\lib\wp71"
mkdir "Microsoft.PlayerFramework.Adaptive\lib\windowsphone8"
mkdir "Microsoft.PlayerFramework.Adaptive.Dash\lib"
mkdir "Microsoft.PlayerFramework.Adaptive.Dash\lib\wp71"
mkdir "Microsoft.PlayerFramework.Adaptive.Dash\lib\windowsphone8"
mkdir "Microsoft.PlayerFramework.Advertising\lib"
mkdir "Microsoft.PlayerFramework.Advertising\lib\wp71"
mkdir "Microsoft.PlayerFramework.Advertising\lib\windowsphone8"
mkdir "Microsoft.PlayerFramework.Analytics\lib"
mkdir "Microsoft.PlayerFramework.Analytics\lib\wp71"
mkdir "Microsoft.PlayerFramework.Analytics\lib\windowsphone8"
mkdir "Microsoft.PlayerFramework.TimedText\lib"
mkdir "Microsoft.PlayerFramework.TimedText\lib\wp71"
mkdir "Microsoft.PlayerFramework.TimedText\lib\windowsphone8"
mkdir "Microsoft.PlayerFramework.WebVTT\lib"
mkdir "Microsoft.PlayerFramework.WebVTT\lib\wp71"
mkdir "Microsoft.PlayerFramework.WebVTT\lib\windowsphone8"

@popd

@echo.
@echo Done.
@echo.
@pause
