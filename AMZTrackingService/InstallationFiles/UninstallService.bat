@ECHO OFF
echo Uninstallingnstalling AMZTrackingService...
REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319
set PATH=%PATH%;%DOTNETFX2%

net stop AMZTrackingService
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u %~dp0..\AMZTrackingService.exe
echo ---------------------------------------------------
pause
echo Done.

rem C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe D:\github\DeGNotification\src\app\AMZTrackingService\bin\Debug\AMZTrackingService.exe