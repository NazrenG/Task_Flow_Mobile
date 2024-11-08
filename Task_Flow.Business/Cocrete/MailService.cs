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

        public void SendEmail(string recieverMail, string name, string phone, string text)
        {
            string sender = _userName;
            string senderPassword = _password;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(sender);
            message.Subject = $"New Feedback!";
            message.To.Add(new MailAddress(recieverMail));
            var str = new StringBuilder();
            message.Body = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Email Notification</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            padding: 20px;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            border: 1px solid #ddd;
            border-radius: 8px;
            background-color: #f9f9f9;
        }
        h1 {
            color: #333;
        }
        .details {
            margin: 20px 0;
        }
        .details p {
            margin: 5px 0;
        }
        .footer {
            margin-top: 20px;
            font-size: 0.9em;
            color: #555;
        }
    </style>
" + $"</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <h1>Message Received</h1>\r\n        <p>Dear Admin,</p>\r\n        <p>You have received a new message from a user. Below are the details:</p>\r\n        \r\n        <div class=\"details\">\r\n            <p><strong>Name:</strong> {name}</p>\r\n            <p><strong>Phone:</strong> {phone}</p>\r\n            <p><strong>Message:</strong></p>\r\n            <p>{text}</p>\r\n        </div>\r\n\r\n        <div class=\"footer\">\r\n            <p>Thank you,</p>\r\n            <p>Your Website Team</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";
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

