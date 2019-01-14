# MonoHelloWorld

**Overview**<br>
This example is meant to show a basic structure for creating C# console apps 
that run on IBM i.<br>

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamples/MonoHelloWorld (or the directory where the EXE was created.)`
`mono MonoHelloWorld.exe "P1" "P2" "P3-true/false"`

**Running the program using the MONO CL command**<br>
```
MONO WORKDIR('/MonoOniSamples/MonoHelloWorld')   
     EXEFILE(MonoHelloWorld.exe)                 
     ARGS('A' 'B' 'true')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)
```
**Compiling and running this program solution**<br>

1.) Build the solution from the PASE command line:

Start pase command line terminal:<br>
`CALL QP2TERM`

Set path to Mono<br>
`PATH=/opt/mono/bin:$PATH`

Export path to Mono<br>
`export PATH`

Change to the selected app folder where the Visual Studio solution .SLN file is located:<br>
`cd /MonoOniSamples/MonoHelloWorld`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled EXE or DLL in your build dir of:<br>
`/MonoOniSamples/MonoHelloWorld/MonoHelloWorld/bin/Debug`

Change to EXE directory:<br>
`cd /MonoOniSamples/MonoHelloWorld/MonoHelloWorld/bin/Debug`

Run EXE:<br>
`mono MonoHelloWorld.exe "Parm1" "Parm2" "true"`

If all works as expected you should see a list of messages displayed on the console along with the parms.

2.) The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.
