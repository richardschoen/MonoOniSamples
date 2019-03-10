# MonoRunPase

**Overview**<br>
This example is meant to show how to crawl a selected IFS directory tree.
Crawling a directory tree can be useful when looking for large IFS objects or trying 
to determine overall IFS disk space usage. This process should avoid endless looping
potential issues by using the FileAttributes.Reparsepoint attribute to identify symbolic 
links during processing.
The process is very quick so it has the ability to throttle itself and sleep as needed<br>
during processing to avoid overloading the CPU. Results are placed in an IFS file and a 
DB2 outfile table in MONOTEMP library.<br>
I crawled about 106000 files by listing /QOpenSys in about 2-3 minutes. Nice.<br>

**Program parameters**<br>
IFS directory to crawl/list - This is the directory to crawl. File list attributes will be directory, file name, file size, file create date, changed date and last access date.

File filter - *.* for all files. (Note: This parm is not functional currently so always pass *.*. You can filter the OUTFILE with a query after.)

IncludSubDirs - true-crawl directory and all child directories. false-just crawl top level directory.

OutputIFSFile - Out file in the IFS to receive file list. All contents will also be copied to outfile: MONOTEMP/MONOLISTDIR. 

Replace - true-replace output file. false-do not replace existing output file.

Sleepthrottlems - Sleep throttle in milliseconds. 0-no sleep go as fast as possible, 3000-Sleep 3 seconds every 1000 files listed to lower CPU usage.

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamples/MonoDirList/MonoDirList/bin/Debug (or the directory where the EXE was created.)`

`mono MonoRunDirList.exe "IFSDir Ex: /tmp" "FileFilter-*.*" "IncludeSubDirs-true/false" "OutputIFSFile-Ex: /tmp/filelist.txt" "Replace-true/false" "Sleep throttle in ms-Ex:0-no sleep"`

**Running the program using the MONO CL command**<br>
```
MONO WORKDIR('/MonoOniSamples/MonoDirList/MonoDirList/bin/Debug')   
     EXEFILE(MonoDirList.exe)                 
     ARGS('/tmp' '*.*' 'true' '/tmp/filelist.txt' 'true' '0')                              
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
`cd /MonoOniSamples/MonoDirList`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled EXE or DLL in your build dir of:<br>
`/MonoOniSamples/MonoDirList/MonoDirList/bin/Debug`

Change to EXE directory:<br>
`cd /MonoOniSamples/MonoDirList/MonoDirList/bin/Debug`

Run EXE:<br>
`mono MonoDirList.exe "/tmp" "*.*" "true" "/tmp/filelist.txt" "true" "0"`

If all works as expected you should have an IFS output file named /tmp/filelist.txt with a list of files from /tmp.<br>
There should also be an outfile named: MONOTEMP/MONODIRLST which is a physical file copy of the IFS list.<br>
The outfile can be queried and sorted as needed to possibly determine disk space usage and largest file in the dir tree perhaps.<br>

2.) Compiling solution Using Visual Studio:<br>
The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.
