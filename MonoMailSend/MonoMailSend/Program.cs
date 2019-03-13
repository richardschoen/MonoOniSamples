using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MimeKit;
using MailKit;
using MailKit.Net;
using MailKit.Net.Smtp;

namespace MonoMailSend
{
    class Program
    {
        static void Main(string[] args)
        {

            // Add certificates to store
            //https://forum.openpetra.org/d/417-sending-emails-from-mono-on-linux
            //https://developercommunity.visualstudio.com/content/problem/89672/visual-studio-for-mac-nuget-restore-ignores-mono-c.html
            //Office365 Certs
            //https://support.office.com/en-us/article/office-365-certificate-chains-0c03e6b3-e73f-4316-9e2b-bf4091ae96bb
            //mozroots--import--sync
            //certmgr -ssl smtps://smtp.office365.com:587
            //certmgr -ssl imap://imap.office365.com:993 

            try
            {


                // Set TZ environment variable for desired timezone
                // TODO: Timezone is set at offset 00:00:00. Need to pick up UTC from setting somewhere. ?? See mono source perhaps. 
                // TODO: Still need to figure best way to do timezone. Can we make TZ env variable work ????
                // TODO: List all possible timezone choices to a file
                Environment.SetEnvironmentVariable("TZ", "America/Chicago");

                // Get command line parms and bail out if less than expected number of parms passed. 
                // Parms should be passed with double quotes around values and single space between parms.
                // Example Windows command line call: pgm1.exe "parm1" "parm2" "parm3"
                // Example IBM i PASE or Qshell command line call: mono pgm1.exe "parm1" "parm2" "parm3"
                if (args.Length < 14)
                {
                    throw new Exception(14 + " required parms: [From] [To] [CC] [BCC} [SUBJECT] [BodyText] [BodyFile] [Attachments] [smtphost] [smtpport-25] [smtpauthtype] [smtpauth-true/false] [smtpuser] [smtppass] ");
                }


                //Extract parms from command line
                var pfrom = args[0];
                var pto = args[1];
                var pcc = args[2];
                var pbcc = args[3];
                var psubject = args[4];
                var pbody = args[5];
                var pbodyfile = args[6]; 
                var pattachments = args[7]; 
                var psmtphost = args[8];
                var psmtpport = Convert.ToInt32(args[9]);
                var psmtpauthtype = args[10];
                var psmtpauth = Convert.ToBoolean(args[11]);
                var psmtpuser = args[12];
                var psmtppass = args[13];

                Console.WriteLine("Start of new email message");

                // Create new message 
                var message = new MimeMessage();

                //Add the From address
                Console.WriteLine("Adding From address:" + pfrom);
                message.From.Add(new MailboxAddress(pfrom));

                //Add To addresses
                var arrTo = pto.Split(';');
                for (int ct=0; ct < arrTo.Length; ct++)
                {
                    //Add address if not blank
                    if (arrTo[ct].Trim() != "")
                    {
                        message.To.Add(new MailboxAddress(arrTo[ct]));
                        Console.WriteLine("Adding To address:" + arrTo[ct]);
                    }
                }
                //Add CC addresses
                var arrCc = pcc.Split(';');
                for (int ct = 0; ct < arrCc.Length; ct++)
                {
                    //Add address if not blank
                    if (arrCc[ct].Trim() != "")
                    {
                        message.Cc.Add(new MailboxAddress(arrCc[ct]));
                        Console.WriteLine("Adding CC address:" + arrCc[ct]);
                    }
                }
                //Add BCC addresses
                var arrBcc = pbcc.Split(';');
                for (int ct = 0; ct < arrBcc.Length; ct++)
                {
                    //Add address if not blank
                    if (arrBcc[ct].Trim() != "")
                    {
                        message.Bcc.Add(new MailboxAddress(arrBcc[ct]));
                        Console.WriteLine("Adding BCC address:" + arrBcc[ct]);
                    }
                }

                // Set subject
                message.Subject = psubject;
                Console.WriteLine("Adding subject:" + psubject);

                //Create mail body builder to create message body with attachment
                var builder = new BodyBuilder();

                // Add initial body text if not blank
                if (pbody.Trim()!="")
                {
                    Console.WriteLine("Adding body text:" + pbody);
                    builder.TextBody = pbody.Replace("<crlf>","\r\n").Replace("<CRLF>","\r\n");
                }
                else
                {
                    Console.WriteLine("No body text:" + pbody);
                }

                //Append text to the message body from file if found
                if (pbodyfile.Trim() != "")
                {
                    if (File.Exists(pbodyfile))
                    {
                        Console.WriteLine("Appending body text from file:" + pbodyfile);
                        builder.TextBody = builder.TextBody + "\r\n" + File.ReadAllText(pbodyfile);
                    }
                    else
                    {
                        throw new Exception("Body text file " + pbodyfile + " not found.");
                    }
                }

                //Add file attachments to message body
                var arrAttachments = pattachments.Split(';');
                for (int ct = 0; ct < arrAttachments.Length; ct++)
                {
                    //Add attachment if not blank
                    if (arrAttachments[ct].Trim() != "")
                    {
                        //Make sure attachment exists. Otherwise bail with error.
                        if (File.Exists(arrAttachments[ct]))
                        {
                            Console.WriteLine("Adding body attachment file:" + arrAttachments[ct]);
                            builder.Attachments.Add(arrAttachments[ct]);
                        } else
                        {
                            throw new Exception("Attachment file " + arrAttachments[ct] + " not found.");
                        }
                    }

                }

                //Set final message body
                message.Body = builder.ToMessageBody();

                // Now let's attempt to send our message
                using (var client = new SmtpClient())
                {

                    // Bypass secure connection auth
                    // https://github.com/jstedfast/MailKit/blob/master/FAQ.md#InvalidSslCertificate
                    // TODO - resolve auto cert auth problem. Without this line SSL connections fail to offie365/gmail
                    //client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    // Determine if secure connection needed or not
                    Console.WriteLine("Auth type:" + psmtpauthtype);
                    if (psmtpauthtype.Trim().ToLower() == "none") {
                        client.Connect(psmtphost, psmtpport, MailKit.Security.SecureSocketOptions.None);
                    }
                    else if (psmtpauthtype.Trim().ToLower() == "auto") {
                        client.Connect(psmtphost, psmtpport, MailKit.Security.SecureSocketOptions.Auto);
                    }
                    else if (psmtpauthtype.Trim().ToLower() == "sslonconnect") {
                        client.Connect(psmtphost, psmtpport, MailKit.Security.SecureSocketOptions.SslOnConnect);
                    }
                    else if (psmtpauthtype.Trim().ToLower() == "starttls")
                    {
                        client.Connect(psmtphost, psmtpport, MailKit.Security.SecureSocketOptions.StartTls);
                    }
                    else if (psmtpauthtype.Trim().ToLower() == "starttlswhenavailable")
                    {
                        client.Connect(psmtphost, psmtpport, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
                    }
                    else
                    {
                        throw new Exception("Unable to determine auth type. Auth Type:" + psmtpauthtype + ". Email cancelled.");
                    }

                    // Authentication only needed if the SMTP server requires authentication
                    Console.WriteLine("Authentication required:" + psmtpauth);
                    if (psmtpauth)
                    {
                        //client.AuthenticationMechanisms.Remove("XOAUTH2");
                        client.Authenticate(psmtpuser,psmtppass);
                    }

                    // Send the message 
                    Console.WriteLine("Sending message.");
                    client.Send(message);

                    // Disconnect from server
                    Console.WriteLine("Disconnecting from server.");
                    client.Disconnect(true);

                    // Successful email send
                    Environment.ExitCode = 0;
                    Console.WriteLine("Email sent successfully");

                }
            } catch (Exception ex)
            {
                Console.WriteLine("Email error: " + ex.Message);
                Environment.ExitCode = 99;
            }
            finally
            {
                Environment.Exit(Environment.ExitCode);
            }


        }
    }
}
