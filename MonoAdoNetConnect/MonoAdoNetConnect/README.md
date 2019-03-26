# MonoAdoNetConnect

**Overview**<br>
This example is meant to show you how to use the IBM.Data.DB2.dll ADO.Net driver to access IBM i data with 
with Mono.Net on IBM i. The sample program exercises the DB2 database actions. The sample also uses Calvin Buckleys
tweaked version of the IBM.Data.DB2 driver that will work with libdb400 under PASE in IBM i. 
This version of the ADO.Net database driver only runs on an IBM i that has Mono .Net installed, so coding 
cannot be directly tested against IBM i unless the compiled .Net binary is precompiled and placed in the 
IFS before running using the mono command in PASE.
<br>
Github repo for IBM.Data.DB2.dll - DB2 LUW ADO.NET provider, adapted to work with libdb400 under IBM i 
https://github.com/MonoOni/db2i-ado.net
<br>
Note: The IBM.Data.DB2.Dll binary is packaged in the dll folder under this project or you can build it yourself from the github link.
<br>

**Program parameters**<br>
**P1-DB2 Connection string** Example connection that should work on any IBM i with Mono .Net installed:<br>
```DSN=*LOCAL;UID=CURRUSER;``` (where CURRUSER is the PASE IBMi user running the program)<br>
Blank connection string defaults to ```DSN=*LOCAL;``` (Current user id will be automatically derived from current user logged in.<br>
Use **WRKRDBDIRE** command to find the *LOCAL entry name and other remote accessible database names. The DB2 driver should be able to access data on other IBM i systems.
<br>
**P2-SQL SELECT query** Enter a query to use to select records. Recommended first test: ```select * from qiws.qcustcdt```
<br>
**P3-Maximum records to select** 0=All. Enter a value greater than one to limit the number of records read during a query to limit the records returned. Recommended first test: Use ```10```

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamples/MonoAdoNetConnect/MonoAdoNetConnect/bin/Debug (or the directory where the EXE was created.)`

`mono MonoAdoNetConnect.exe "DSN=*LOCAL;" "select * from qiws.qcustcdt" "10" `

**Running the program using the MONO CL command**<br>
`
 MONO WORKDIR('/MonoOniSamples/MonoAdoNetConnect/MonoAdoNetConnect/bin/Debug')                    
     EXEFILE(MonoAdoNetConnect.exe)                                  
     ARGS('DSN=*LOCAL;' 'select * from qiws.qcustcdt' '10')
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
`cd /MonoOniSamples/MonoAdoNetConnect`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled EXE or DLL in your build dir of:<br>
`/MonoOniSamples/MonoAdoNetConnect/MonoAdoNetConnect/bin/Debug`

Change to EXE directory:<br>
`cd /MonoOniSamples/MonoAdoNetConnect/MonoAdoNetConnect/bin/Debug`

Run EXE:<br>
`mono MonoAdoNetConnect.exe "DSN=*LOCAL;" "select * from qiws.qcustcdt" "10"
`

If all works as expected you should see a list of messages displayed on the 
console saying the database connection and tests worked successfully.

2.) Compiling solution Using Visual Studio:<br>
The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.
