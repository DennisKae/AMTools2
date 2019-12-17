@echo off
call Skriptkonfiguration.cmd

echo Es wird zum nächsten Desktop gewechselt.

curl -X POST -H "accept: text/plain" -H "Content-Length: 0" "%SERVERADRESSE%/api/Desktop/SwitchRight"
echo.

set arg1=%1
IF "%arg1%" == "nopause" GOTO END

pause

:END