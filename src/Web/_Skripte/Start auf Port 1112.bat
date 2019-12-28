@echo off

..\AmTools.Web.exe --urls=http://localhost:1112

set arg1=%1
IF "%arg1%" == "nopause" GOTO END

pause

:END