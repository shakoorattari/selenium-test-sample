@ECHO OFF

REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

echo Uninstallingnstalling AMZTrackingService...
net stop AMZTrackingService
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u %~dp0..\AMZTrackingService.exe
echo ---------------------------------------------------

echo Installing IEPPAMS Win Service...
echo ---------------------------------------------------
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil "%~dp0..\AMZTrackingService.exe"

net start AMZTrackingService
echo ---------------------------------------------------
pause
echo Done.

rem username: DESKTOP-3EAHNA4\admin
rem password: P@ss1234
rem C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe D:\github\DeGNotification\src\app\AMZTrackingService\bin\Debug\AMZTrackingService.exe