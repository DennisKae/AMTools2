@echo off

nuget.exe restore ..\src\AMTools.sln

CALL "Release Batch.bat" nopause

CALL "Release Website.bat" nopause

echo Release all beendet.
echo.

set arg1=%1
IF "%arg1%" == "nopause" GOTO END

pause

:END