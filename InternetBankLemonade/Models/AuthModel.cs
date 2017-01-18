using System.Linq;

namespace InternetBankLemonade.Models
{
    public class AuthModel
    {
        private MainEntities _db = new MainEntities();

        public bool IsAdmin(string email)
        {
            try
            {
                var result = (from u in _db.Users where u.Email == email select u).FirstOrDefault();
                if (result != null)
                {
                    if (result.IsAdmin)
                        return true;
                    return false;
                }

            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}