# MonoHelloWorld

Overview
This example is meant to show a basic structure for creating C# console apps 
that run on IBM i.

**Running the program from a PASE or QSHELL command line screen**<br>
`mono MonoHelloWold "P1" "P2" "P3-true/false"`

**Running the program using the MONO CL command**<br>
```
MONO WORKDIR('/MonoOniSamples/MonoHelloWorld')   
     EXEFILE(MonoHelloWorld.exe)                 
     ARGS('A' 'B' 'true')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)
```
Compiling and building this program solution

1.) The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.

2.) An alternative option is to build the solution from the PASE command line:

Start pase command line terminal:
CALL QP2TERM

Change to the selected app folder:
cd /MonoOniSamplses/MonoRunPase

Call the following command line to build the Visual Studio Solution natively on i:
xbuild /p:CscToolExe=mcs

If all ran scuccessfully you should have a compiled EXE or DLL in your build dir.

