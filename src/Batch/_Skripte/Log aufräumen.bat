@echo off

..\AMTools.Batch.exe clean-log

set arg1=%1
IF "%arg1%" == "nopause" GOTO END

pause

:END