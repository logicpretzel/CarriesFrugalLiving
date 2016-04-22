using System;
using System.Net;
using System.Net.Mail;

namespace CarriesFrugalLiving.utils
{
    public class EmailSender
    {
        private static string _port;
        private static string _accountName;
        private static string _password;
        private static string _servername;

        public string GetNotifyMsgBody(string sTitle = "", string sBody = "")
        {
            string font = "font-family: Verdana, Arial, sans-serif; font-size: 1.5em; ";
            string rc = "<body style=\"" 
                + font + " margin: 0; padding: 0; min-width: 100%; margin: 0; padding: 0; min-width: 100%; width: 100%;\">" 
                + sTitle + "<hr/><table style=\"width: 100%; max-width: 600px; background-color: #AC5865; color: maroon;\"><tr><td width=\"10px\">&nbsp;&nbsp;&nbsp;</td><table style=\"" 
                + font + " width: 100%; max-width: 600px; background-color: #AC5865; color: maroon;\"  ><tr>" 
                + String.Format("<td>\n{0}\n</td>",sBody) 
                + " </tr></table></td></tr></table></body>";
 
            
            return rc;
        }

        public string Send(string to, string subject, string body, bool isHtml, string[] attachments)
        {
            MailAddressCollection To = new MailAddressCollection();
            To.Add(to);
            return Send(To, subject, body, isHtml, attachments);
        }


        public string  Send(MailAddressCollection toAddresses, string subject, string body, bool isHtml, string[] attachments)
        {
            string  rc = "";
            string sBCC = "";
            int port = 0;


            _accountName = System.Configuration.ConfigurationManager.AppSettings["FLMailUserName"];
            _password = System.Configuration.ConfigurationManager.AppSettings["FLMailPassWord"];
            sBCC =  System.Configuration.ConfigurationManager.AppSettings["FLMailBCCAddress"];
            _servername = System.Configuration.ConfigurationManager.AppSettings["FLServerName"];
            _port = System.Configuration.ConfigurationManager.AppSettings["FLServerPort"];

            if (!int.TryParse(_port, out port))
            {
                port = 25;
            }

            MailAddress from = new MailAddress(_accountName);
            MailAddress bcc;
            bcc = new MailAddress(sBCC);

            MailMessage mailMessage = new MailMessage()
            {
                From = from,
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
 
            try
            {
                foreach (var a in toAddresses)
                {
                    mailMessage.To.Add(a);
                }

                mailMessage.Bcc.Add(bcc);

            }  catch (Exception eCatch) {
                rc = eCatch.Message;
            }

            try {
                // Add attachments
                if (attachments != null)
                {
                    for (int i = 0; i < attachments.Length; i++)
                    {
                        mailMessage.Attachments.Add(new Attachment(attachments[i]));
                    }
                }
            } catch (Exception e)
            {
                rc = e.Message;
            }

            try
            {
                SmtpClient smtp = new SmtpClient(_servername, port);

                smtp.Timeout = 90000;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_accountName, _password);
                smtp.EnableSsl = false;

                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtp.Send(mailMessage);
            }
            catch (Exception e) {
                rc = e.Message;  // send back error message if any
            }

            return rc;

        }


    }

}
