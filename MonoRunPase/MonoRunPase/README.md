# MonoRunPase

**Overview**<br>
This example is meant to show how to interact with PASE and Qshell commands from 
a .Net application.<br>

-You can run an IBM i CL system commmand.<br>
-You can run the Qshell db2 CLI and export data to CSV,XML or JSON.<br>
-You can run a Pase command with parameters.<br>
-You can run a Qshell command line. <br>

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamplses/MonoRunPase (or the directory where the EXE was created.)`
`mono MonoRunPase.exe "Action" "Command" "Arguments" "DB2OutputType-CSV,JSON,XML" "DB2IFSOutputFile-Ex: /tmp/monotest.txt"`

**Running the program using the MONO CL command**<br>
```
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('Action' 'Command' 'Arguments' 'DB2OutputType-CSV,JSON,XML' 'DB2IFSOutputFile-Ex: /tmp/monotest.txt')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             
```
**Running db2 Query Example:**<br>
Note: No special escaping is required on SQL statement. Handled by PaseCommandHelper class.<br>
```
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('db2' 'select * from qiws.qcustcdt' ' ' 'CSV' '/tmp/monocsv.txt')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             
```
**Running CL command Example:**<br>
Note: No special escaping is required on CL command. Handled by PaseCommandHelper class.<br>
```
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('system' ' ' 'SNDMSG MSG(TEST) TOUSR(QSYSOPR)' ' ' ' ')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             
```
**Running Pase command with arguments Example:**<br>
Note: Notice that the PASE command/program to run is in P2 and the command arguments go in P3. They must be separated for PASE calls.<br>
```
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('pase' 'ls' '/tmp' ' ' ' ')                              
     DSPSTDOUT(*YES)                             
     DLTSTDOUT(*YES)                             
```
**Running QSHELL command Example:**<br>
Note: Notice that the entire Qshell command/program to run plus arguments are in P2. They are NOT separated for Qshell calls.<br>
```
MONO WORKDIR('/MonoOniSamples/MonoRunPase')   
     EXEFILE(MonoRunPase.exe)                 
     ARGS('qsh' 'ls /tmp ' ' ' ' ' ' ')                              
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

Change to the selected app folder where EXE is located:
`cd /MonoOniSamples/MonoRunPase`

Call the following command line to build the Visual Studio Solution natively on i:
`xbuild /p:CscToolExe=mcs`

If all ran scuccessfully you should have a compiled EXE or DLL in your build dir.


