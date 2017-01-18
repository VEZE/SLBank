using System;
using System.Linq;
using Helpers;
using System.Net;
using System.Net.Mail;
using SendGrid;
using System.Collections.Generic;

namespace InternetBankLemonade.Models
{
    public class LoginModel
    {
        private MainEntities _db = new MainEntities();

        public string LogIn(string email, string password)
        {
            Crypt crypt = new Crypt();
            try
            {
                var query = from users in _db.Users where users.Email == email select users;
                var user = query.FirstOrDefault<Users>();
                if (user.IsLocked)
                    return "Locked";
                if (crypt.IsMatch(password, user.PasswordHash))   //TODO: User can be null
                {
                    user.FailCount = 0;
                    _db.SaveChanges();
                    return "Success";
                }
                    
            }
            catch
            {
                return "Invalid";
            }

            return "Invalid";
        }

        public string FailCountInc(string email)
        {
            try
            {
                var user = (from users in _db.Users where users.Email == email select users).FirstOrDefault();
                user.FailCount += 1;
                if (user.FailCount >= 3)
                {
                    user.IsLocked = true;
                    _db.SaveChanges();
                    return "Locked";
                }
                _db.SaveChanges();
                
                return "";
            }
            catch
            {
                return "";
            }
        }

        public bool IsFirstLogin(string email)
        {
            try
            {
                var result = (from u in _db.Users where u.Email == email select u).FirstOrDefault().IsFirstLogin;
                return result;
            }
            catch
            {
                return false;
            }
        }

        public void SetFirstLogin(string email, bool login)
        {
            try
            {
                var result = (from u in _db.Users where u.Email == email select u).FirstOrDefault();
                result.IsFirstLogin = login;
                _db.SaveChanges();
            }
            catch
            {
                
                
            }
            
        }

        public void SendEmail(string email, List<SessionKeys> keys)
        {
            // Create network credentials to access your SendGrid account
            var username = System.Environment.GetEnvironmentVariable("SENDGRID_USERNAME");
            var pswd = System.Environment.GetEnvironmentVariable("SENDGRID_PASSWORD");

            var credentials = new NetworkCredential(username, pswd);
            // Create an Web transport for sending email.
            var transportWeb = new Web(credentials);
            // Create the email object first, then add the properties.
            var myMessage = new SendGridMessage();

            // Add the message properties.
            myMessage.From = new MailAddress("keys@lemonadeproject.co.uk");

            // Add multiple addresses to the To field.
            var recipient = email;

            myMessage.AddTo(recipient);

            myMessage.Subject = "Ваши сессионные ключи";

            //Add the HTML and Text bodies
            myMessage.Html = "Ваши сессионные ключи, запишите их в надежное место! <br/> Ключ 1:" + keys[0].Key1
                + "<br/> Ключ 2:" + keys[0].Key2
                + "<br/> Ключ 3:" + keys[0].Key3
                + "<br/> Ключ 4:" + keys[0].Key4
                + "<br/> Ключ 5:" + keys[0].Key5
                + "<br/> Ключ 6:" + keys[0].Key6
                + "<br/> Ключ 7:" + keys[0].Key7
                + "<br/> Ключ 8:" + keys[0].Key8
                + "<br/> Ключ 9:" + keys[0].Key9
                + "<br/> Ключ 10:" + keys[0].Key10;
            transportWeb.DeliverAsync(myMessage);
            
        }
    }
}