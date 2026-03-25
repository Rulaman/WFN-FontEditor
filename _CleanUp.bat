@echo off
echo Before running this script, make sure to run a 'Clean Solution' from Visual Studio for both Debug and Release. Then,
echo close the program and run this script for cleaning the following leftovers before uploading the source code to GitHub:
echo.
echo - .vs\
echo - AGS.Plugin.FontEditor\bin\
echo - AGS.Plugin.FontEditor\obj\
echo - WFN-FontEditor\bin\
echo - WFN-FontEditor\obj\

:START
echo.
set /P PARAM_VALUE=Do you wish to proceed? (Y/N)

if /I "%PARAM_VALUE%"=="y" goto BEGIN
if /I "%PARAM_VALUE%"=="n" goto END

echo Invalid input, please enter Y or N.
goto START

:BEGIN
if exist ".vs" rmdir ".vs" /S /Q
if exist "AGS.Plugin.FontEditor\bin" rmdir "AGS.Plugin.FontEditor\bin" /S /Q
if exist "AGS.Plugin.FontEditor\obj" rmdir "AGS.Plugin.FontEditor\obj" /S /Q
if exist "WFN-FontEditor\bin" rmdir "WFN-FontEditor\bin" /S /Q
if exist "WFN-FontEditor\obj" rmdir "WFN-FontEditor\obj" /S /Q

:END
exit