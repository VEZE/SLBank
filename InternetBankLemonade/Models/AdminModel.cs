using System;
using System.Collections.Generic;
using System.Linq;

namespace InternetBankLemonade.Models
{
    
    public class AdminModel
    {
        MainEntities _db = new MainEntities();
        OrderModel _orderModel = new OrderModel();
        RegistrationModel _registrationModel = new RegistrationModel();
        LoginModel _loginModel = new LoginModel();

        public bool AddPassportNumber(string passportNumber)
        {
            if (String.IsNullOrEmpty(passportNumber))
                return false;
            try
            {
                var result = (from pa in _db.Passports where pa.PassportNumber == passportNumber select pa).FirstOrDefault();
                if (result != null)
                    return false;
            }
            catch
            {
                return false;
            }
            Passports passports = new Passports();
            passports.Id = Guid.NewGuid().ToString();
            passports.PassportNumber = passportNumber;
            passports.IsActive = false;
            try
            {
                _db.Passports.AddObject(passports);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Account> GetCardsByPassport(string passport)
        {
            List<Account> list = new List<Account>();
            try
            {
                var userId = GetUserIdByPassportNumber(passport);
                var acc = (from a in _db.Account where a.OwnerId == userId select a).ToList();
                return acc;
            }
            catch
            {

                return list;
            }

        }

        public bool AddToAccount(string cardId, double moneyAmount, string ownerId, string currencyId)
        {
            try
            {
                string accId = GetAccountIdByCardId(cardId);
                var account = (from a in _db.Account where a.Id == accId select a).FirstOrDefault();
                account.MoneyAmount += moneyAmount;

                _db.SaveChanges();
                if (_orderModel.AddTransactionRecord(ownerId, GetServiceIdByCurrencyId(currencyId),
                    accId, DateTime.Now, DateTime.Now, "+", currencyId, moneyAmount, "Успешно"))
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetUserIdByPassportNumber(string passportNumber)
        {
            try
            {
                var passportId = (from p in _db.Passports where p.PassportNumber == passportNumber select p).FirstOrDefault().Id;
                var userId = (from u in _db.Users where u.PassportId == passportId select u).FirstOrDefault().Id;
                return userId;
            }
            catch
            {
                return "";
            }
        }

        public string GetCurrencyIdByCardId(string cardId)
        {
            try
            {
                var currencyId = (from a in _db.Account where a.Id == cardId select a).FirstOrDefault().CurrencyId;
                return currencyId;
            }
            catch
            {
                return "";
            }
        }

        public string GetAccountIdByCardId(string cardId)
        {
            try
            {
                var accountId = (from a in _db.Account where a.Id == cardId select a).FirstOrDefault().Id;
                return accountId;
            }
            catch
            {
                return "";
            }
        }

        public string GetServiceIdByCurrencyId(string currnecyId)
        {
            try
            {
                var serviceCode =
                    (from c in _db.CurrencyRates where c.Id == currnecyId select c).FirstOrDefault().CurrencyCode;
                string serviceName = "Зачисление на карту(" + serviceCode.Trim() + ")";
                var serviceId = (from s in _db.Services where s.ServiceCurrencyId == currnecyId && s.ServiceName == serviceName select s).FirstOrDefault().Id;
                return serviceId;
            }
            catch
            {
                return "";
            }
        }

        public bool UpdateKeys(string passport)
        {
            try
            {
                var passportId =
                    (from p in _db.Passports where p.PassportNumber == passport select p).FirstOrDefault().Id;
                var user = (from u in _db.Users where u.PassportId == passportId select u).FirstOrDefault();
                var sessionKeysScopeId = user.SessionKeysScopeId;
                var sessionKeys =
                    (from se in _db.SessionKeys where se.Id == sessionKeysScopeId select se).FirstOrDefault();

                string[] keys = _registrationModel.GenerateSessionKeys();
                
                sessionKeys.Key1 = keys[0];
                sessionKeys.Key2 = keys[1];
                sessionKeys.Key3 = keys[2];
                sessionKeys.Key4 = keys[3];
                sessionKeys.Key5 = keys[4];
                sessionKeys.Key6 = keys[5];
                sessionKeys.Key7 = keys[6];
                sessionKeys.Key8 = keys[7];
                sessionKeys.Key9 = keys[8];
                sessionKeys.Key10 = keys[9];
                
                _loginModel.SetFirstLogin(user.Email ,true);
                _db.SaveChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }

        public bool IsPassportOnAdmin(string passport)
        {
            try
            {
                var passportId = (from p in _db.Passports where p.PassportNumber == passport select p).FirstOrDefault().Id;
                var isAdmin = (from u in _db.Users where u.PassportId == passportId select u).FirstOrDefault().IsAdmin;
                return isAdmin;
            }
            catch
            {
                return false;

            }
            
        }
    }
}