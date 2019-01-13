# MonoRunPase

**Overview**
This example is meant to show how to interact with PASE and Qshell commands from 
a .Net application. 

-You can run an IBM i CL system commmand
-You can run the Qshell db2 CLI and export data to CSV,XML or JSON.
-You can run a Pase command with parameters.
-You can run a Qshell command line. 

**Running the program from a PASE or QSHELL command line screen**
mono RunPaseCommand "Action" "Command" "Arguments" "DB2OutputType-CSV,JSON,XML" "DB2IFSOutputFile-Ex: /tmp/monotest.txt"

**Running the program using the MONO CL command**<br>
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('Action' 'Command' 'Arguments' 'DB2OutputType-CSV,JSON,XML' 'DB2IFSOutputFile-Ex: /tmp/monotest.txt')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             

**Running db2 Query Example:**<br>
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('db2' 'select * from qiws.qcustcdt' ' ' 'CSV' '/tmp/monocsv.txt')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             

**Running CL command Example:**<br>
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('system' ' ' 'SNDMSG MSG(TEST) TOUSR(QSYSOPR)' ' ' ' ')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             

**Running Pase command with arguments Example:**<br>
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('pase' 'ls' '/tmp' ' ' ' ')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             

**Running QSHELL command Example:**<br>
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('qsh' 'ls /tmp ' ' ' ' ' ' ')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             

**Compiling and building this program solution**

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


