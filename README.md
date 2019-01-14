# MonoOniSamples

**Mono on IBM i Code Samples**<br>
This repository will be used to publish .Net and IBM i code samples related to running .Net on IBM i.<br>

**Mono on i**<br>
You will first need to have the Mono on i environment installed on your IBM i.<br>
Mono for IBM i binary save file distribution can be downloaded from here:<br>
https://github.com/MonoOni/binarydist

**Mono on i Library**<br>
If you want to easily call .Net applications on IBM i and integrate into standard job streams such as CL or RPG, download and install this library on your IBM i as well.<br>
https://github.com/richardschoen/MonoOniLibrary

If you don't install the MONOI library you will need to call your .Net applications from a PASE QP2TERM command line for testing. The MONO command makes things much easier.<br>

**Sample List ReadMe Files**<br>
MonoHelloWorld - Sample template for Mono .Net console apps<br>
https://github.com/richardschoen/MonoOniSamples/tree/master/MonoHelloWorld/MonoHelloWorld

MonoRunPase - Sample for running db2 cli, CL system commands, Qshell or PASE commands from a .Net App.<br> 
https://github.com/richardschoen/MonoOniSamples/tree/master/MonoRunPase/MonoRunPase


**Installing Samples in IFS Using Git**<br>
You can download the zip file from github and unzip Visual Studio projects or place in the IFS by using the following command sequences to clone the repository.<br>

**Start pase command line terminal:**<br>
`CALL QP2TERM`

**Change to the root folder. For this example we will be cloning code to /MonoOniSamples IFS directory:**<br>
`cd /`

**Call the following command line to clone the repository to /MonoOniSamples folder**<br>
`git -c http.sslVerify=false clone --recurse-submodules https://github.com/richardschoen/MonoOniSamples.git`

If all ran successfully you should have a new folder named /MonoOniSamples available on your IFS.<br>

 