@echo off

..\AMTools.Batch.exe switch-desktop --desktop=2

set arg1=%1
IF "%arg1%" == "nopause" GOTO END

pause

:END