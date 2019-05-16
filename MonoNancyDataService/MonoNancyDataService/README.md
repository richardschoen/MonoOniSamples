# MonoNancyDataService

**Overview**<br>
This web service sample showcases using the NancyFx Web Framework to create an
IBM i data access web service that runs on Windows server as a self-hosted web service or natively on IBM i via Mono .Net.
* When used from Windows, the application uses the IBM i Client Access/400 ODBC driver to access data on IBM i
so Client Access/400, IBM i Access or the IBM i ACS ODBC Driver must be loaded and configured on the Windows server.
* When used natively from IBM i , the application uses the PASE db2 cli client for data access. 
Nothing special should need to be loaded in order to use the db2 cli for data access. However QSHELL and PASE are required and should be already installed on recent IBM i releases such as V7R1-V7R3.
<br>

**Prerequisites**<br>
* The MONOI libary must exist on the IBMi. <br>
Within the library the following objects are needed if the AUthEnabled setting = true:<br><br>
* DB2 Table: MONOI.MNSESSIONS must be created from DDS or SQL source in the MONOI library.<br><br>
* Program: MONOI.MNUSRCHK01 must be created from source in the MONOI library.<br><br>

**The .Net app AuthEnabled setting = false by default so auth is not required so any token value can be sent in the Authorization header until AuthEnabled = true. AuthEnabled should be set = true for any production usage.**<br>

**When compiling to run natively on IBMi, uncomment the following line in Ibmiodule.cs to use db2 cli for data access:<br>
```#define MonoIbmi```
**When compiling to run on Windows, comment out the following line in Ibmiodule.cs to use Client Access/400 ODBC for data access:<br>
```#define MonoIbmi```

**Service Calls**<br>
**Get** - ```/api/ibmi/login/{user}/{password}``` - Login to service and create token.<br>
**Get** - ```/api/ibmi/logout``` - Logout from service. <br>
Token must be sent in Authorization header as well.<br>
**Get** - ```/api/ibmi/shutdown``` - End service.<br> 
      Token must be sent in Authorization header as well.<br>
**Get** - ```/api/ibmi/querytest``` - Query all records from QIWS.QCUSTCDT as a service test.<br> 
      Token must be sent in Authorization header as well.<br>
**Get** - ```/api/ibmi/querytest2/{cusnum-use 938472}``` - Query select record from QIWS.QCUSTCDT as a service test.<br> 
      Token must be sent in Authorization header as well.<br>
**Post** - ```/api/ibmi/execquery``` - Query data. SQL statement passed in a JSON query data packet.<br> 
      Token must be sent in Authorization header as well.<br>
 ```
      --12345
      Content-Disposition: form-data; name="jsondata"

      {
         "action": "query",
        "query": "select * from qiws.qcustcdt fetch first 12 rows only"
      }
      --12345
```

**Post** - ```/api/ibmi/execnonquery``` - Perform insert/update/delete or other action query in a JSON query data packet.<br> 
      Token must be sent in Authorization header as well.<br>
```
      --12345
      Content-Disposition: form-data; name="jsondata"

      {
        "action": "nonquery",
        "query": "insert into qiws.qcustcdt (cusnum,lstnam) values(123456,'MonoSvc')",
        "query": "update qiws.qcustcdt set lstnam = 'MonoDt2' where cusnum=123456",
        "query": "delete from qiws.qcustcdt where cusnum=123456"
      }
      --12345
```
**Post** - ```/api/ibmi/execclcommand``` - Run selected CL command. Command passed in a JSON query data packet.<br> 
      Token must be sent in Authorization header as well.<br>
```
      --12345
      Content-Disposition: form-data; name="jsondata"

      {
        "action": "clcommand",
        "command": "SNDMSG MSG('Test Message') TOUSR(QSYSOPR)"
      }
      --12345
```

**Program parameters
No special parms. All settings are part of the .Net project.<br>

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamples/MonoNancyDataService/MonoNancyDataService/bin/Debug (or the directory where the EXE was created.)`

`mono MonoNancyDataService.exe`

**Running the program using the MONO CL command**<br>
```
 MONO WORKDIR('/MonooniSamples/MonoNancyDataService/MonoNancyDataService/bin/Debug')                    
     EXEFILE(MonoNancyDataService.exe)                                  
     DSPSTDOUT(*NO)                                            
```
**Note - you probably want to consider running the service as a batch job by using the SBMJOB command to submit the MONO command call to QSYSNOMAX so the job runs in subsystem QSYSWRK or QUSRWRK. 

**Compiling and running this program solution**<br>

1.) Build the solution from the PASE command line:

Start pase command line terminal:<br>
`CALL QP2TERM`

Set path to Mono<br>
`PATH=/opt/mono/bin:$PATH`

Export path to Mono<br>
`export PATH`

Change to the selected app folder where the Visual Studio solution .SLN file is located:<br>
`cd /MonoNancyDataService/MonoNancyDataService`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled EXE or DLL in your build dir of:<br>
`/MonoOniSamples/MonoNancyDataService/MonoNancyDataService/bin/Debug`

Change to EXE directory:<br>
`cd /MonoOniSamples/MonoNancyDataService/MonoNancyDataService/bin/Debug`

Run EXE:<br>
`mono MonoNancyDataService.exe`

If all works as expected you should see a list of messages displayed on the 
console saying the conversion worked successfully.

2.) Compiling solution Using Visual Studio:<br>
The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.
