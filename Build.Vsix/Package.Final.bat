@pushd %~dp0%

@set ZIP=%ProgramFiles%\7-Zip\7z.exe

cd Microsoft.PlayerFramework

"%ZIP%" a ..\Microsoft.PlayerFramework.zip "*"

cd ..\
move Microsoft.PlayerFramework.zip Microsoft.PlayerFramework.vsix

@popd

@echo.
@echo Done.
@echo.
