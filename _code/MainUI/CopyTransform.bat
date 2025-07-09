@echo off
echo Copying transform file

if not exist %1 goto File1NotFound
if not exist %2 goto File2NotFound

dir %1 >> file1.txt
dir %2 >> file2.txt
xcopy %1 %2 /Y
echo copied file el:%ERRORLEVEL%

echo Done.