using System.Collections.Generic;
using System.Linq;

namespace InternetBankLemonade.Models
{
    public class DashboardModel
    {
        MainEntities _db = new MainEntities();

        public List<Account> GetCards(string email)
        {
            List<Account> list = new List<Account>();
            try
            {
                var UsersQuery = from u in _db.Users where u.Email == email select u;
                var response = UsersQuery.FirstOrDefault();
                var AccountQuery = from a in _db.Account where a.OwnerId == response.Id select a;
                var usdId = (from c in _db.CurrencyRates where c.CurrencyCode == "USD" select c).FirstOrDefault().Id;
                var eurId = (from c in _db.CurrencyRates where c.CurrencyCode == "EUR" select c).FirstOrDefault().Id;
                var bynId = (from c in _db.CurrencyRates where c.CurrencyCode == "BYN" select c).FirstOrDefault().Id;
                foreach (var item in AccountQuery.ToList())
                {
                    if (item.CurrencyId == usdId)
                        item.CurrencyCode = "USD";
                    if (item.CurrencyId == eurId)
                        item.CurrencyCode = "EUR";
                    if (item.CurrencyId == bynId)
                        item.CurrencyCode = "BYN";
                    list.Add(item);
                }
                return list;
            }
            catch
            {
                return list;
            }
        }
    }
}