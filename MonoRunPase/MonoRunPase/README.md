﻿# MonoRunPase

**Overview**<br>
This example is meant to show how to interact with PASE and Qshell commands from 
a .Net application.<br>

-You can run an IBM i CL system commmand.<br>
-You can run the Qshell db2 CLI and export data to CSV,XML or JSON.<br>
 You can also return the db2 resultset as a DataTable in your .Net code to 
 be used for additional application processing. Using the db2 cli command you don't
 need an external database driver to interact with the database.
-You can run a Pase command with parameters.<br>
-You can run a Qshell command line. <br>

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamples/MonoRunPase/MonoRunPase/bin/Debug (or the directory where the EXE was created.)`

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

**Compiling solution Using Visual Studio**<br>
The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.

**Compiling and running this program solution**<br>

1.) Build the solution from the PASE command line:

Start pase command line terminal:<br>
`CALL QP2TERM`

Set path to Mono<br>
`PATH=/opt/mono/bin:$PATH`

Export path to Mono<br>
`export PATH`

Change to the selected app folder where the Visual Studio solution .SLN file is located:<br>
`cd /MonoOniSamples/MonoRunPase`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled EXE or DLL in your build dir of:<br>
`/MonoOniSamples/MonoRunPase/MonoRunPase/bin/Debug`

Change to EXE directory:<br>
`cd /MonoOniSamples/MonoRunPase/MonoRunPase/bin/Debug`

Run EXE:<br>
`mono MonoRunPase.exe "db2" "select * from qiws.qcustcdt" " " "CSV" "/tmp/testcsv.txt"`

If all works as expected you should see a list of messages displayed on the console along with the parms.<br>
Also CSV file /tmp/testcsv.txt should exist with the results of our query. 

2.) Compiling solution Using Visual Studio:<br>
The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.
