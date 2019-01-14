# MonoListTimeZones

**Overview**<br>
This example is meant to list all time zone values that you can possibly select to STDOUT.<br>
Mono does not correctly observe the IBMi or PASE timezone settings so we need to set a local
environment variable named: TZ to set the desired local timezone before any date/time functions are called.

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamples/MonoListTimeZones (or the directory where the EXE was created.)`
`mono MonoHelloWorld.exe`

**Running the program using the MONO CL command**<br>
```
MONO WORKDIR('/MonoOniSamples/MonoListTimeZones/MonoListTimeZones/bin/Debug')   
     EXEFILE(MonoListTimeZones.exe)                 
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)
```
**Compiling and building this program solution**<br>

1.) The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.

2.) An alternative option is to build the solution from the PASE command line:

Start pase command line terminal:<br>
`CALL QP2TERM`

Set path to Mono<br>
`PATH=/opt/mono/bin:$PATH`

Export path to Mono<br>
`export PATH`

Change to the selected app folder where the Visual Studio solution .SLN file is located:<br>
`cd /MonoOniSamples/MonoListTimeZones`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled EXE or DLL in your build dir of:<br>
`/MonoOniSamples/MonoListTimeZones/MonoListTimeZones/bin/Debug`

Change to EXE directory:<br>
`cd /MonoOniSamples/MonoListTimeZones/MonoListTimeZones/bin/Debug`

Run EXE:<br>
`mono MonoListTimeZones.exe`

If all works as expected you should see a list of timezones displayed on the console. 


