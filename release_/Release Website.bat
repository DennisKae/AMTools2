@echo off

echo Cleane Ausgabeverzeichnis...
echo.

RMDIR /S /Q .\Website
echo.

cd ..\src\Web\Frontend

echo Installiere keine NPM Pakete...
REM call npm install
echo.

echo Kompiliere die Angular Anwendung...
call npm run serverbuild
echo.

cd ..

echo Kompiliere und veroeffentliche die gesamte Webanwendung...
dotnet publish -c Release -o ..\..\release_\Website .\Web.csproj
echo.

cd ..\..\release_

echo Entferne unnoetigen Frontend-Ordner im Ausgabeverzeichnis...
echo.

RMDIR /S /Q .\Website\Frontend
echo.

echo Kopiere README.md...
echo.

copy ..\README.md .\Website\README.md


set arg1=%1
IF "%arg1%" == "nopause" GOTO END

pause

:END