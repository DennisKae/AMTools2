@echo off

..\AMTools.Batch.exe startup

set arg1=%1
IF "%arg1%" == "nopause" GOTO END

pause

:END