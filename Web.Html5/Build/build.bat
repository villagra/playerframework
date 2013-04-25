@echo off
setlocal

set CURL=tools\curl.exe

rem *** Setup ***
cd /d "%~dp0"
set CONFIG=%~1
set TARGET=%~2

if "%CONFIG%"=="" set CONFIG=Debug
set OUTDIR=%~dp0output

if exist "%OUTDIR%" rmdir /s /q "%OUTDIR%"

rem *** Create working folder ***
xcopy /e "..\src" "%OUTDIR%\"

rem *** Remove unnecessary source files ***
del "%OUTDIR%\js\LogWriterPlugin.js"
del "%OUTDIR%\js\ConsoleLogWriterPlugin.js"
del "%OUTDIR%\js\DocumentLogWriterPlugin.js"

rem *** Order necessary source files ***
ren "%OUTDIR%\css\playerframework.css" "01_playerframework.css"
ren "%OUTDIR%\css\control-strip.css" "02_control-strip.css"
ren "%OUTDIR%\js\PlayerFramework.js" "01_PlayerFramework.js" 
ren "%OUTDIR%\js\Class.js" "02_Class.js"
ren "%OUTDIR%\js\Object.js" "03_Object.js"
ren "%OUTDIR%\js\Plugin.js" "04_Plugin.js"
ren "%OUTDIR%\js\ModulePlugin.js" "05_ModulePlugin.js"
ren "%OUTDIR%\js\MediaPlugin.js" "06_MediaPlugin.js"
ren "%OUTDIR%\js\VideoMediaPlugin.js" "07_VideoMediaPlugin.js"
ren "%OUTDIR%\js\ControlPlugin.js" "08_ControlPlugin.js"
ren "%OUTDIR%\js\VideoElementMediaPluginBase.js" "09_VideoElementMediaPluginBase.js"
ren "%OUTDIR%\js\SilverlightMediaPluginBase.js" "10_SilverlightMediaPluginBase.js"
ren "%OUTDIR%\js\StaticContentMediaPlugin.js" "11_StaticContentMediaPlugin.js"
ren "%OUTDIR%\js\PlaylistPlugin.js" "12_PlaylistPlugin.js"
ren "%OUTDIR%\js\TrackPlugin.js" "13_TrackPlugin.js"
ren "%OUTDIR%\js\TrackDataProviderPlugin.js" "14_TrackDataProviderPlugin.js"
ren "%OUTDIR%\js\ChapterTrackPlugin.js" "15_ChapterTrackPlugin.js"
ren "%OUTDIR%\js\TimelineTrackPlugin.js" "16_TimelineTrackPlugin.js"
ren "%OUTDIR%\js\CaptionTrackPlugin.js" "17_CaptionTrackPlugin.js"
ren "%OUTDIR%\js\ControlStripPlugin.js" "18_ControlStripPlugin.js"

rem *** Concatenate source files ***
copy /b "%OUTDIR%\css" "%OUTDIR%\css\playerframework.debug.css"
copy /b "%OUTDIR%\js" "%OUTDIR%\js\playerframework.debug.js"

rem *** Minify source files ***
if "%CONFIG%"=="Release" (
	"%CURL%" --form compressfile[]=@"%OUTDIR%\css\playerframework.debug.css" --form type=CSS --form redirect=1 --location "http://refresh-sf.com/yui/" > "%OUTDIR%\css\playerframework.min.css"
	"%CURL%" --data-urlencode js_code@"%OUTDIR%\js\playerframework.debug.js" --data compilation_level=SIMPLE_OPTIMIZATIONS --data output_info=compiled_code --data output_format=text "http://closure-compiler.appspot.com/compile" > "%OUTDIR%\js\playerframework.min.js"
) else (
	copy "%OUTDIR%\css\playerframework.debug.css" "%OUTDIR%\css\playerframework.min.css"
	copy "%OUTDIR%\js\playerframework.debug.js" "%OUTDIR%\js\playerframework.min.js"
)

rem *** Deploy output files ***
xcopy /y /r "%OUTDIR%\css\playerframework.*.css" "..\css\"
xcopy /y /r "%OUTDIR%\js\playerframework.*.js" "..\js\"

rem *** Clean up ***
rmdir /s /q "%OUTDIR%\"

endlocal
pause