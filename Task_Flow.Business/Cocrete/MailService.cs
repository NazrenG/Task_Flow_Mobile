using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Task_Flow.Business.Cocrete
{
   public class MailService
    {

        private readonly string _host;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _password;

        public MailService(IConfiguration configuration)
        {
              _host = configuration["SmtpSettings:Host"];
        _port = int.Parse(configuration["SmtpSettings:Port"]);
        _userName = configuration["SmtpSettings:Username"];
        _password = configuration["SmtpSettings:Password"];
        }

        public void SendEmail(string recieverMail, string text)
        {
            string sender = _userName;
            string senderPassword = _password;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(sender);
            message.Subject = $"New Feedback!";
            message.To.Add(new MailAddress(recieverMail));
            var str = new StringBuilder();
            message.Body = text;
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient(_host)
            {
                Port = _port,
                Credentials = new NetworkCredential(sender, senderPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);


        }


        public int sendVerifyMail(string recieverMail)
        {
            string sender = _userName;
            string senderPassword = _password;

            Random randNum = new Random();
            int temp = randNum.Next(1000, 9999);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(sender);
            message.Subject = $"Verification Code {temp}";
            message.To.Add(new MailAddress(recieverMail));
            message.Body = @"<link href=""https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css"" rel=""stylesheet"" />
                        <section class=""mail-seccess section"">
                        	<div class=""container"">
                        		<div class=""row"">
                        			<div class=""col-lg-6 offset-lg-3 col-12"">
                        				<!-- Error Inner -->
                        				<div class=""success-inner"">
                        					<h1><i class=""fa fa-envelope""></i><span>Verify Your Mail!</span></h1>
                        					<p> Do not share your code with anyone! Copy it and paste to the progarm so your mail can be verified.</p>
                        					<a href=""#"" class=""btn btn-primary btn-lg"">Go Home</a>
                        				</div>
                        				<!--/ End Error Inner -->
                        			</div>
                        		</div>
                        	</div>
                        </section>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient(_host)
            {
                Port = _port,
                Credentials = new NetworkCredential(sender, senderPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);

            return temp;

        }

        public bool mailVerifier(string recieverMail)
        {
            int funcReturn = sendVerifyMail(recieverMail);
            Console.WriteLine("Enter verification number we sent to you mail: ");
            int userInput = Convert.ToInt32(Console.ReadLine());
            if (userInput == funcReturn)
                return true;
            return false;
        }


    }
}

