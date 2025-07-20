//using Castle.Core.Smtp;
using System.Net;
using System.Net.Mail;
using Serilog;
using Pinnacle.Entities;
using System.Runtime.CompilerServices;
using Pinnacle.Helpers;



namespace Pinnacle.Helpers
{

    public class SendMail
    {
        CommonLogic CL = new CommonLogic();
        ConfigKeyInfo config = new ConfigKeyInfo();

        public SendMail()
        {
            config = CL.getConfigValues();
        }
        public bool SendEmail(string Toemail, string CC, string subject, string htmlMessage, string BCC = "")
        {
            try
            {
                //var AbsolutePath = _config.GetValue<string>("CustomConfigs:AbsolutePath");
                SmtpClient client = new SmtpClient();
                client.Host = config.SMTP_HOST;
                client.Port = int.Parse(config.SMTP_PORT);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(config.SMTP_USERNAME, config.SMTP_PASSWORD);
                //client.Send("Admin@HitechHealthSolutions.com", Toemail, subject, htmlMessage);
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(config.SMTP_USERNAME);
                mailMessage.To.Add(Toemail);
                if (CC != null && CC != "")
                    mailMessage.To.Add(CC);
                if (BCC != null && BCC != "")
                    mailMessage.Bcc.Add(BCC);
                // mailMessage.CC.Add("suresh.k@lionorbit.com");
                mailMessage.Body = htmlMessage;
                mailMessage.Subject = subject;
                client.Send(mailMessage);
                return true;

                client.EnableSsl = true;
                // client.IsBodyHtml = true;
                client.Credentials = new NetworkCredential(config.SMTP_USERNAME, config.SMTP_PASSWORD);
                client.Send("Pinnaclenotification@gmail.com", Toemail, subject, htmlMessage);

            }
            catch (Exception ex)
            {
                Log.Information("Failed to send email at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return false;
            }
        }

        public bool SendEmailWithAttachment(string Toemail, string CC, string subject, string htmlMessage, string attachmentPath, string BCC = "")
        {
            try
            {
                //var AbsolutePath = _config.GetValue<string>("CustomConfigs:AbsolutePath");
                SmtpClient client = new SmtpClient();
                client.Host = config.SMTP_HOST;
                client.Port = int.Parse(config.SMTP_PORT);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(config.SMTP_USERNAME, config.SMTP_PASSWORD);
                //client.Send("Admin@HitechHealthSolutions.com", Toemail, subject, htmlMessage);
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(config.SMTP_USERNAME);
                mailMessage.To.Add(Toemail);
                if (CC != null && CC != "")
                    mailMessage.To.Add(CC);
                if (BCC != null && BCC != "")
                    mailMessage.Bcc.Add(BCC);
                // mailMessage.CC.Add("suresh.k@lionorbit.com");
                mailMessage.Body = htmlMessage;
                mailMessage.Subject = subject;

                Attachment attachment = new Attachment(attachmentPath);
                mailMessage.Attachments.Add(attachment);

                client.Send(mailMessage);
                return true;

                client.EnableSsl = true;
                // client.IsBodyHtml = true;
                client.Credentials = new NetworkCredential(config.SMTP_USERNAME, config.SMTP_PASSWORD);
                client.Send("Pinnaclenotification@gmail.com", Toemail, subject, htmlMessage);

            }
            catch (Exception ex)
            {
                Log.Information("Failed to send email at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return false;
            }
        }
        public bool SendEmailWithTemplate(string Toemail, string CC, string subject, string htmlMessage, string BCC = "")

        {
            try
            {

                SmtpClient client = new SmtpClient();
                client.Host = config.SMTP_HOST;
                client.Port = int.Parse(config.SMTP_PORT);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(config.SMTP_USERNAME, config.SMTP_PASSWORD);

                string startupPath = System.IO.Directory.GetCurrentDirectory();
                string FilePath = startupPath + "/EmailTemplates/MailTemplate.html";

                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();

                MailText = MailText.Replace("[MailContent]", htmlMessage);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(config.SMTP_USERNAME);
                mailMessage.To.Add(Toemail);
                if (CC != null && CC != "")
                    mailMessage.To.Add(CC);
                if (BCC != null && BCC != "")
                    mailMessage.Bcc.Add(BCC);             
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = MailText;
                mailMessage.Subject = subject;
                client.Send(mailMessage);
                return true;

                client.EnableSsl = true;
                // client.IsBodyHtml = true;
                client.Credentials = new NetworkCredential(config.SMTP_USERNAME, config.SMTP_PASSWORD);
                client.Send("Pinnaclenotification@gmail.com", Toemail, subject, htmlMessage);

            }
            catch (Exception ex)
            {
                Log.Information("Failed to send email at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return false;
            }
        }

        public bool SendEmailWithTemplateAndAttachment(string Toemail, string CC, string subject, string htmlMessage, string BCC, string attachmentPath)

        {
            try
            {
                //var AbsolutePath = _config.GetValue<string>("CustomConfigs:AbsolutePath");
                SmtpClient client = new SmtpClient();
                client.Host = config.SMTP_HOST;
                client.Port = int.Parse(config.SMTP_PORT);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(config.SMTP_USERNAME, config.SMTP_PASSWORD);
                //client.Send("Admin@HitechHealthSolutions.com", Toemail, subject, htmlMessage);


                //Fetching Email Body Text from EmailTemplate File. 

                // string FilePath = "D:/Gopal/AROHS/AROHSapi/AROHS/EmailTemplates/MailTemplate.html";
                string startupPath = System.IO.Directory.GetCurrentDirectory();
                string FilePath = startupPath + "/EmailTemplates/MailTemplate.html";

                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();

                MailText = MailText.Replace("[MailContent]", htmlMessage);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(config.SMTP_USERNAME);
                mailMessage.To.Add(Toemail);
                if (CC != null && CC != "")
                    mailMessage.To.Add(CC);
                if (BCC != null && BCC != "")
                    mailMessage.Bcc.Add(BCC);
                // mailMessage.CC.Add("suresh.k@lionorbit.com");
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = MailText;
                mailMessage.Subject = subject;

                //string attachmentPath = Path.GetFullPath("Uploads/User\\" + "image_User1002.png").Replace("~\\", ""); ;
                Attachment attachment = new Attachment(attachmentPath);
                mailMessage.Attachments.Add(attachment);


                client.Send(mailMessage);
                return true;

                client.EnableSsl = true;
                // client.IsBodyHtml = true;
                client.Credentials = new NetworkCredential(config.SMTP_USERNAME, config.SMTP_PASSWORD);
                client.Send("Pinnaclenotification@gmail.com", Toemail, subject, htmlMessage);

            }
            catch (Exception ex)
            {
                Log.Information("Failed to send email at " + DateTime.Now.ToString() + " message " + (ex.Message));
                return false;
            }
        }
    }
}
