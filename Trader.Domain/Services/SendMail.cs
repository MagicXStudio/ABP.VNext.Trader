using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.IO;

namespace Trader.Domain.Services
{
    public static class SendMail
    {
        public static void Send()
        {
            string path = @"F:\Project\LintSenseManagementSystem\LintSense.Inspection.Dashboard\src\Download\Excel\验布报告汇总08-04-11-45-53.xlsx";
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("Joey Tribbiani", "report@lintsense.com"));//
            message.To.Add(new MailboxAddress("AngkorW", "404926414@qq.com"));
            message.Subject = "你好吗?";

            MimePart attachment = new MimePart("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                Content = new MimeContent(File.OpenRead(path), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(path)
            };
            TextPart body = new TextPart("plain")
            {
                Text = @"Hey Angkor"
            };

            // 现在创建multipart / mixed容器来保存消息文本和图像附件
            Multipart multipart = new Multipart("mixed")
            {
                body, attachment
            };
            // 现在将multipart / mixed设置为消息正文 
            message.Body = multipart;
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.Timeout = 60 * 1000;
                    client.Connect("smtp.mxhichina.com", 465, SecureSocketOptions.Auto);//smtp.mxhichina.com
                    client.Authenticate("report@lintsense.com", "Lonely520980");

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
