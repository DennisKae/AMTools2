@echo off

echo Cleane Ausgabeverzeichnis...
echo.

RMDIR /S /Q .\Batch
echo.


echo Veroeffentliche das Batch-Programm...
echo.

dotnet publish -c release -o ..\release_\Batch ..\src\Batch\Batch.csproj 
echo.


echo Kopiere README.md...
echo.

copy ..\README.md .\Batch\README.md


set arg1=%1
IF "%arg1%" == "nopause" GOTO END

pause

:END