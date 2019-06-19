# MonoOdbcConnect

**Overview**<br>
This example is meant to show you how to use the new IBM i Access ODBC driver for PASE to access IBM i data with 
with Mono.Net on IBM i. The sample program exercises the ODBC database query actions. 
<br>
**Note:** It is assumed that you already have the IBM i Access ODBC installed and running in PASE before using this sample program.
<br>

**Program parameters**<br>
**P1-DB2 Connection string** Example connection that should work on any IBM i with Mono .Net installed:<br>
```DSN=*LOCAL;UID=USER1;PWD=PASS1``` (where USER1 is the PASE IBMi user running the program and PASS1 is their password.)<br>
<br>
**P2-SQL SELECT query** Enter a query to use to select records. Recommended first test: ```select * from qiws.qcustcdt```
<br>

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamples/MonoOdbcConnect/MonoOdbcConnect/bin/Debug (or the directory where the EXE was created.)`

`mono MonoOdbcConnect.exe "DSN=*LOCAL;UID=USER1;PWD=PASS1;" "select * from qiws.qcustcdt"`

**Running the program using the MONO CL command**<br>
`
 MONO WORKDIR('/MonoOniSamples/MonoOdbcConnect/MonoOdbcConnect/bin/Debug')                    
     EXEFILE(MonoOdbcConnect.exe)                                  
     ARGS('DSN=*LOCAL;UID=USER1;PWD=PASS1' 'select * from qiws.qcustcdt')
     DSPSTDOUT(*YES)                                            
`
**Compiling and running this program solution**<br>

1.) Build the solution from the PASE command line:

Start pase command line terminal:<br>
`CALL QP2TERM`

Set path to Mono<br>
`PATH=/opt/mono/bin:$PATH`

Export path to Mono<br>
`export PATH`

Change to the selected app folder where the Visual Studio solution .SLN file is located:<br>
`cd /MonoOniSamples/MonoOdbcConnect`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled EXE or DLL in your build dir of:<br>
`/MonoOniSamples/MonoOdbcConnect/MonoOdbcConnect/bin/Debug`

Change to EXE directory:<br>
`cd /MonoOniSamples/MonoOdbcConnect/MonoOdbcConnect/bin/Debug`

Run EXE:<br>
`mono MonoOdbcConnect.exe "DSN=*LOCAL;" "select * from qiws.qcustcdt" "10"
`

If all works as expected you should see a list of messages displayed on the 
console saying the database connection and tests worked successfully.

2.) Compiling solution Using Visual Studio:<br>
The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.