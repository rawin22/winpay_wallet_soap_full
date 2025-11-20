using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Tsg.UI.Main.App_LocalResources;

namespace Tsg.UI.Main.Extensions
{
    public static class EmailExtension
    {

        static log4net.ILog _logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static System.Net.Mail.SmtpClient _smtp = new System.Net.Mail.SmtpClient()
        {
            Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"] ?? "25"),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Host = ConfigurationManager.AppSettings["smtpHost"] ?? "",
            EnableSsl = false,
            Credentials = new NetworkCredential(ConfigurationManager.AppSettings["smtpUserName"] ?? "",
                ConfigurationManager.AppSettings["smtpPassword"] ?? "")
        };

        /// <summary>
        /// Email sender
        /// </summary>
        public static class EmailService
        {
            /// <summary>
            /// Sends an email
            /// </summary>
            /// <param name="emailAccount">Email account to use</param>
            /// <param name="subject">Subject</param>
            /// <param name="body">Body</param>
            /// <param name="fromAddress">From address</param>
            /// <param name="fromName">From display name</param>
            /// <param name="toAddress">To address</param>
            /// <param name="toName">To display name</param>
            /// <param name="replyTo">ReplyTo address</param>
            /// <param name="replyToName">ReplyTo display name</param>
            /// <param name="bcc">BCC addresses list</param>
            /// <param name="cc">CC addresses list</param>
            /// <param name="attachmentFilePath">Attachment file path</param>
            /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
            public static void SendEmail(string subject, string body,
                string fromAddress, string fromName, string toAddress, string toName,
                string replyTo = null, string replyToName = null,
                IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
                HttpPostedFileBase attachments = null)
            /*string attachmentFilePath = null, string attachmentFileName = null*/
            {
                try
                {
                    var message = new MailMessage();
                    //from, to, reply to
                    message.From = new MailAddress(fromAddress, fromName);
                    message.To.Add(new MailAddress(toAddress, toName));
                    if (!String.IsNullOrEmpty(replyTo))
                    {
                        message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
                    }

                    //BCC
                    if (bcc != null)
                    {
                        foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                        {
                            message.Bcc.Add(address.Trim());
                        }
                    }

                    //CC
                    if (cc != null)
                    {
                        foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                        {
                            message.CC.Add(address.Trim());
                        }
                    }

                    //content
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;
                    byte[] file = new byte[0];

                    //create  the file attachment for this e-mail message
                    if (attachments != null)
                    {
                        using (var stream = attachments.InputStream)
                        {
                            using (var reader = new BinaryReader(stream))
                            {
                                file = reader.ReadBytes((int)stream.Length);
                            }
                        }
                        MemoryStream ms = new MemoryStream(file);
                        message.Attachments.Add(new Attachment(ms, attachments.FileName, attachments.ContentType));
                    }

                    //send email
                    _smtp.Send(message);
                }
                catch (Exception e)
                {
                    _logger.Error(string.Format("EmailExtension.SendEmail, error sending email, exception: {0}", e));
                }

            }

            public static void SendEmail(string subject, string body,
                string fromAddress, string fromName, string toAddress, string toName,
                IEnumerable<string> cc = null,
                string[] attachments = null)
            {

                var message = new MailMessage();
                //from, to, reply to
                message.From = new MailAddress(fromAddress, fromName);
                message.To.Add(new MailAddress(toAddress, toName));

                //CC
                if (cc != null)
                {
                    foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                    {
                        message.CC.Add(address.Trim());
                    }
                }

                //content
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                if (attachments != null && attachments.Length > 0)
                {
                    foreach (var s in attachments)
                    {
                        if (s!= null && !string.IsNullOrEmpty(s.Trim()))
                        {
                            System.Net.Mail.Attachment attachment;
                            attachment = new System.Net.Mail.Attachment(s);
                            message.Attachments.Add(attachment);
                        }
                    }

                }

                _smtp.Send(message);
            }

            /// <summary>
            /// Sends an email
            /// </summary>
            /// <param name="emailAccount">Email account to use</param>
            /// <param name="subject">Subject</param>
            /// <param name="body">Body</param>
            /// <param name="toAddress">To address</param>
            /// <param name="toName">To display name</param>
            public static void SendEmail(string subject, string body, string toAddress, string fromAddress, string toName="")
            {
                var message = new MailMessage();
                //from, to, reply to
                message.From = new MailAddress(fromAddress);
                message.To.Add(new MailAddress(toAddress, toName));

                //content
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                //send email
                _smtp.Send(message);

            }

            public static void SendSysEmail(string subject, string body,
                string toAddress, string toName)
            {
                var emailAccount = ConfigurationManager.AppSettings["systemMail"];
                if (String.IsNullOrEmpty(emailAccount))
                    return;

                var message = new MailMessage();
                //from, to, reply to
                message.From = new MailAddress(emailAccount);
                message.To.Add(new MailAddress(toAddress, toName));

                //content
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                //send email
                _smtp.Send(message);
            }

        }
    }
}