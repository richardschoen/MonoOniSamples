# MonoOniSamples

**Mono on IBM i Code Samples**<br>
This repository will be used to publish .Net and IBM i code samples related to running .Net on IBM i.<br>

**Feel free to contribute your own .Net samples and I will publish them here.**<br>

**Mono on i**<br>
You will first need to have the Mono on i environment installed on your IBM i.<br>
Mono for IBM i binary save file distribution can be downloaded from here:<br>
https://github.com/MonoOni/binarydist

**Mono on i Library**<br>
If you want to easily call .Net applications on IBM i and integrate into standard job streams such as CL or RPG, download and install this library on your IBM i as well.<br>
https://github.com/richardschoen/MonoOniLibrary

If you don't install the MONOI library you will need to call your .Net applications from a PASE QP2TERM command line for testing. The MONO command makes things much easier.<br>

**Sample List ReadMe Files**<br>
**MonoHelloWorld** - Sample template for Mono .Net console apps<br>
https://github.com/richardschoen/MonoOniSamples/tree/master/MonoHelloWorld/MonoHelloWorld

**MonoListTimeZones** - Sample for listing all the time zones available to a Mono .Net App since standard system values or PASE environment values don't work.<br> 
https://github.com/richardschoen/MonoOniSamples/tree/master/MonoListTimeZones/MonoListTimeZones

**MonoRunPase** - Sample for running db2 cli, CL system commands, Qshell or PASE commands from a .Net App.<br> 
https://github.com/richardschoen/MonoOniSamples/tree/master/MonoRunPase/MonoRunPase


**Installing Samples in IFS Using Git**<br>
You can download the zip file from github and unzip Visual Studio projects or place in the IFS by using the following command sequences to clone the repository.<br>

**Start QShell or PASE command line terminal:**<br>
`STRQSH or CALL QP2TERM`

**Change to the root folder. For this example we will be cloning code to /MonoOniSamples IFS directory:**<br>
`cd /`

**Call the following command line to clone the repository to /MonoOniSamples folder**<br>
`git -c http.sslVerify=false clone --recurse-submodules https://github.com/richardschoen/MonoOniSamples.git`

If all ran successfully you should have a new folder named /MonoOniSamples available on your IFS.<br>

 