@echo off
setlocal ENABLEEXTENSIONS
if {%1}=={-start} (set JustStart=1& shift) else (set JustStart=0)
if {%1}=={-stop}  (set JustStop=1&  shift) else (set JustStop=0)
if {%1}=={} (echo usage: %~nx0 out.etl&goto :EOF)
set TraceFile=%~1

set XPerfCmd=xperf.exe
set LoggersToStop=

rem Kernel Logger
set LoggersToStop=%LoggersToStop% -stop
rem User/IE Logger
set UserFlags=PerfTrack
set UserFlags=%UserFlags%+Microsoft-Windows-AppHost:0x3
set UserFlags=%UserFlags%+Microsoft-Windows-COM:0x3
set UserFlags=%UserFlags%+Microsoft-Windows-COMRuntime:0x3
set UserFlags=%userFlags%+Microsoft-Jscript:0x3
set UserFlags=%UserFlags%+Microsoft-Windows-Immersive-Shell

set LoggersToStop=%LoggersToStop% -stop browser
set XPerfCmd=%XPerfCmd% -start browser -on %UserFlags%
set XPerfCmd=%XPerfCmd% -f _browser.etl
set XPerfCmd=%XPerfCmd% -BufferSize 1024 -MinBuffers 64

if %JustStop%==1 goto :JustStop
:JustStart

rem Stop any existing left-over loggers...
xperf %LoggersToStop% 1> NUL 2> NUL

echo Starting tracing...
%XPerfCmd%

if %JustStart%==1 goto :EOF
set /P Dummy=Please hit enter to stop tracing...
:JustStop

echo Outputting trace to: "%TraceFile%"
echo INFO: Warnings due to lost events may be due to WinINet/DWM providers. As long
echo INFO: as there are no lost buffers or hundreds of events lost, it might be OK.

xperf %LoggersToStop% -d "%TraceFile%"
