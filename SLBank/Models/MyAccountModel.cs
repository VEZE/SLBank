using System;
using System.Linq;
using Helpers;

namespace InternetBankLemonade.Models
{
    public class MyAccountModel
    {
        private MainEntities _db = new MainEntities();
        private Crypt _crypt = new Crypt();
        public Users GetUser(string email)
        {
            var query = from u in _db.Users where u.Email == email select u;
            return query.FirstOrDefault<Users>();
        }

        public string GetUserIdFromEmail(string email)
        {
            var query = (from u in _db.Users where u.Email == email select u).FirstOrDefault();
            if (query != null)
                return query.Id;
            return String.Empty;
        }


        public string UpdateUserInformation(string email, string password, string newPassword, string newEmail, bool isPass = false)
        {
            string id = GetUserIdFromEmail(email);
            var currentUser = _db.Users.Where(u => u.Id == id).FirstOrDefault();
            if (currentUser == null)
                return "UserNA";
            if (_crypt.IsMatch(password, currentUser.PasswordHash))
            {
                try
                {
                    if (isPass)
                    {
                        if (!String.IsNullOrEmpty(newPassword))
                            if (password == newPassword)
                                return "ExPass";
                            currentUser.PasswordHash = _crypt.CryptPassword(newPassword);
                    }
                    
                    if (!String.IsNullOrEmpty(newEmail))
                    {
                        currentUser.Email = newEmail;
                        var auto = (from au in _db.AutoPay where au.Email == email select au).FirstOrDefault();
                        if (auto != null)
                            auto.Email = newEmail;
                    }
                    _db.SaveChanges();
                }
                catch
                {
                    return "Error";
                }
                return "Success";
            }
            return "PassError";
        }
    }
}