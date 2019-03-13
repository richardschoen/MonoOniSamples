# MonoMailSend

**Overview**<br>
This example is meant to show you how to create a mail sender program using the Mailkit/Mimekit API.
The program is a pretty functional production-ready email sender client app.
<br>

**Program parameters
From email address.<br>
One or more To email addresses delimited by semicolon;.<br>
One or more Cc email addresses delimited by semicolon;.<br>
One or more Bcc email addresses delimited by semicolon;.<br>
Subject line - Enter email subject line.<br>
Body text - Enter body text message. If you enter a <CRLF> tag in the text it will be expanded to a CR/LF in the text.<br>
Body file - If the file exists the text will get appended after the body text or if body text is blank, this is the message.<br>
SMTP host name or IP address (smtp.office365.com, smtp.gmail.com, etc.)<br>
SMTP port - 25/587, etc.<br>
SMTP authentication type - none, auto, sslonconnect, starttls, starttlswhenavailable. Normally use non for non-auth and auto for authenticated. See Mailkit docs with any deeper questions.<br>
SMTP authentication enabled - True/False<br>
SMTP user if authentication enabled. Otherwise leave blank.<br>
SMTP password if authentication enabled. Otherwise leave blank.<br>

**Running the program from a PASE or QSHELL command line screen**<br>
`cd /MonoOniSamples/MonoMailSend/MonoMailSend/bin/Debug (or the directory where the EXE was created.)`

`mono MonoMailSend.exe "user1@domain.com" "user1@domain.com" " " " " 
      "Mailkit Test" " " "/MonooniSamples/MonoMailSend/MonoMailSend/bodytext1.txt" "/tmp/report.pdf" "smtp.office365.com"
	  "587" "auto" "true" "user1@domain.com" "password1"`

**Running the program using the MONO CL command**<br>
```
 MONO WORKDIR('/MonooniSamples/MonoMailSend/MonoMailSend/bin/Debug')                    
     EXEFILE(MonoMailSend.exe)                                  
     ARGS(user1@domain.com user1@domain.com    
          ' ' ' ' 'Mailkit Test' ' ' '/MonooniSa    
          mples/MonoMailSend/MonoMailSend/bodytext1.txt' '/tmp/report.pdf    
          smtp.office365.com 587 auto true user1@domain.com     
          password1)                                           
     DSPSTDOUT(*YES)                                            
```
**Compiling and running this program solution**<br>

1.) Build the solution from the PASE command line:

Start pase command line terminal:<br>
`CALL QP2TERM`

Set path to Mono<br>
`PATH=/opt/mono/bin:$PATH`

Export path to Mono<br>
`export PATH`

Change to the selected app folder where the Visual Studio solution .SLN file is located:<br>
`cd /MonoMailSend/MonoMailSend`

Call the following command line to build the Visual Studio Solution natively on i:<br>
`xbuild /p:CscToolExe=mcs`

If all ran successfully you should have a compiled EXE or DLL in your build dir of:<br>
`/MonoOniSamples/MonoMailSend/MonoMailSend/bin/Debug`

Change to EXE directory:<br>
`cd /MonoOniSamples/MonoMailSend/MonoMailSend/bin/Debug`

Run EXE:<br>
`mono MonoMailSend.exe "user1@domain.com" "user1@domain.com" " " " " 
      "Mailkit Test" " " "/MonooniSamples/MonoMailSend/MonoMailSend/bodytext1.txt" "/tmp/report.pdf" "smtp.office365.com"
	  "587" "auto" "true" "user1@domain.com" "password1"`

If all works as expected you should see a list of messages displayed on the 
console saying the conversion worked successfully.

2.) Compiling solution Using Visual Studio:<br>
The easiest way to build this .Net code for IBMi is to compile locally on a PC in Visual Studio
and then copy the EXE and any associated DLL files to the associated app directory in the IFS. 
I typically put each app and DLL files in its own directory. This way each app can have different
version of a class library or other DLL being used without worrying about DLL versioning issues.
