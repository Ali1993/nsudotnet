using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RSS
{
    class Program
    {
       

        static void Main(string[] args)
        {
            string strURL = "http://winbeta.tumblr.com/rss";

            var reader = new SimpleFeedReader.FeedReader();
            var items = reader.RetrieveFeed(strURL);

            string message = null;

            foreach (var item in items)
            {
                message += " Title: " + item.Title + "Address: " + item.Uri;
                Console.WriteLine(item.Title);
                Console.WriteLine(item.Uri);
            }


            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("ТВОЙ ЭМЭИЛ");
                mail.To.Add("ЭМЭИЛ ПОЛУЧАТЕЛЯ");
                mail.Subject = "SSL";
                mail.Body = message;
                mail.IsBodyHtml = true;
                // Can set to false, if you are sending pure text.
                bool enableSSL = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587)) //РАБОТАЕТ ДЛЯ ПОЧТЫ GMAIL
                {
                    smtp.Credentials = new NetworkCredential("ТВОЙ ЭМЭИЛ", "ТВОЙ ПАРОЛЬ");
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }


            Console.ReadLine();
        }
    }
}
