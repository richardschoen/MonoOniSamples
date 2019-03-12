# MonoPase

**Overview**<br>
Mono .Net Pase and Qshell Command Helper Assembly for IBM i<br> 
This .Net assembly project is meant to show how to wrap PASE and QSHELL commands in to a DLL assembly to be used from 
a Mono .Net application. This is a good example of code re-use in a .Net assembly. This assembly is meant to be used 
natively on IBM i from a Mono .Net application. However is you can adapt the code to something else then that's awesome.
Note: Many of the samples have a copy of the PaseCommandHelper object in the source code, but for production apps it would
be a good idea to use this assembly so that you dont need to embed the PaseCommandHelper source in your apps.
<br>

-You can run an IBM i CL system commmand.<br>
-You can run the Qshell db2 CLI and export data to CSV,XML or JSON.<br>
-You can also return the db2 resultset as a DataTable in your .Net code to 
 be used for additional application processing. Using the db2 cli command you don't
 need an external database driver to interact with the database.
-You can run a Pase command with parameters.<br>
-You can run a Qshell command line. <br>

**Using MonoPase.dll in a .Net application. 
Simply add the MonoPase.DLL to your project, instantiate teh PaseCommandHelper object and all the power of PASE is at your fingertips.
Note: This DLL will end up in a Nuget package at some point.

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
`cd /MonoOniSamples/MonoPase`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled DLL in your build dir of:<br>
`/MonoOniSamples/MonoPase/MonoPase/bin/Debug`

2.) Compiling solution Using Visual Studio:<br>
The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.
