using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Helpers;

namespace InternetBankLemonade.Models
{
    public class RegistrationModel
    {
        private MainEntities _db = new MainEntities();
        private Crypt _crypt = new Crypt();
        public string[] GenerateSessionKeys()
        {
            string[] result = new string[10];
            Random rnd = new Random();
            for (int i = 0; i < 10; i++ )
            {
                result[i] = rnd.Next(10000000, 99999999).ToString();
                Thread.Sleep(rnd.Next(10, 150));
            }
            return result;
        }

        public void EncryptKeys(string email)
        {
            try
            {
                var sessionkeyScopeId = (from u in _db.Users where u.Email == email select u).FirstOrDefault().SessionKeysScopeId;
                var list = (from se in _db.SessionKeys where se.Id == sessionkeyScopeId select se).FirstOrDefault();
                list.Key1 = _crypt.CryptPassword(list.Key1);
                list.Key2 = _crypt.CryptPassword(list.Key2);
                list.Key3 = _crypt.CryptPassword(list.Key3);
                list.Key4 = _crypt.CryptPassword(list.Key4);
                list.Key5 = _crypt.CryptPassword(list.Key5);
                list.Key6 = _crypt.CryptPassword(list.Key6);
                list.Key7 = _crypt.CryptPassword(list.Key7);
                list.Key8 = _crypt.CryptPassword(list.Key8);
                list.Key9 = _crypt.CryptPassword(list.Key9);
                list.Key10 = _crypt.CryptPassword(list.Key10);
                _db.SaveChanges();
            }
            catch
            {

            }
        }

        public string RegisterUser(string passportNumber ,string name, string surname, string lastName, string password,
            string email, string telephone)
        {
            string passwordHash = _crypt.CryptPassword(password);
            if (passwordHash == null)
                return "Error";
            //check for collision
            var queryPassportId = from p in _db.Passports where p.PassportNumber == passportNumber select p;
            var passportResult = queryPassportId.FirstOrDefault<Passports>();
            if (passportResult == null)
            {
                return "PassNull";
            }
            if (passportResult.IsActive)
            {
                return "IsActive";
            }
            var queryExist = from u in _db.Users where u.Email == email ||
                                 u.Telephone == telephone  select u;
            if (queryExist.FirstOrDefault<Users>() != null)
            {
                return "Exist";
            }
            SessionKeys sessionKeys = new SessionKeys();
            string[] keys = GenerateSessionKeys();
            sessionKeys.Id = Guid.NewGuid().ToString();
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
            try
            {
                _db.SessionKeys.AddObject(sessionKeys);
                _db.SaveChanges();
            }
            catch
            {
                return "Error";
            }
            Users user = new Users();
            user.Id = Guid.NewGuid().ToString();
            user.Name = name;
            user.Surname = surname;
            user.LastName = lastName;
            user.PasswordHash = passwordHash;
            user.Email = email;
            user.Telephone = telephone;
            user.SessionKeysScopeId = sessionKeys.Id;
            user.PassportId = passportResult.Id;
            user.IsFirstLogin = true;
            try
            {
                _db.Users.AddObject(user);
                _db.SaveChanges();

            }
            catch
            {
                _db.SessionKeys.DeleteObject(new SessionKeys() {Id = sessionKeys.Id});
                return "Error";
            }
            if (CreateAccounts(user.Id))
            {
                try
                {
                    var updatedPassport = _db.Passports.Where(c => c.Id == passportResult.Id).FirstOrDefault();
                    if (updatedPassport != null)
                    {
                        updatedPassport.IsActive = true;
                        _db.SaveChanges();
                    }
                }
                catch
                {
                   return "Error";
                }
                return "Success";
            }
            return "Error";
        }


        public List<SessionKeys> GetKeyList(string email)
        {
            List<SessionKeys> result = new List<SessionKeys>();
            try
            {
                var sessionScopeId =
                (from u in _db.Users where u.Email == email select u).FirstOrDefault().SessionKeysScopeId;
                result = (from s in _db.SessionKeys where s.Id == sessionScopeId select s).ToList();
                return result;
            }
            catch
            {
                return result;
            }
        }

        

        public bool CreateAccounts(string ownerId)
        {
            try
            {
                Account BYNAccount = new Account();
                BYNAccount.Id = Guid.NewGuid().ToString();
                BYNAccount.CardNumber = CardNumberGenerator();
                var BYNquery = from n in _db.CurrencyRates where n.CurrencyCode == "BYN" select n;
                BYNAccount.CurrencyId = BYNquery.FirstOrDefault<CurrencyRates>().Id;
                BYNAccount.CardType = "VISA";
                BYNAccount.ExpDate = new DateTime(2018, 12, 31);
                BYNAccount.MoneyAmount = 100;
                BYNAccount.AccountNumber = Guid.NewGuid().ToString();
                BYNAccount.OwnerId = ownerId;
                BYNAccount.IsLocked = false;

                Services BYNService = new Services();
                BYNService.Id = Guid.NewGuid().ToString();
                BYNService.ServiceName = "Перевод на карту";
                BYNService.ServiceNumber = BYNAccount.AccountNumber;
                BYNService.ServiceCurrencyId = BYNAccount.CurrencyId;

                Account USDAccount = new Account();
                USDAccount.Id = Guid.NewGuid().ToString();
                USDAccount.CardNumber = CardNumberGenerator();
                var USDquery = from n in _db.CurrencyRates where n.CurrencyCode == "USD" select n;
                USDAccount.CurrencyId = USDquery.FirstOrDefault<CurrencyRates>().Id;
                USDAccount.CardType = "MasterCard";
                USDAccount.ExpDate = new DateTime(2018, 12, 31);
                USDAccount.MoneyAmount = 100;
                USDAccount.AccountNumber = Guid.NewGuid().ToString();
                USDAccount.OwnerId = ownerId;
                USDAccount.IsLocked = false;

                Services USDService = new Services();
                USDService.Id = Guid.NewGuid().ToString();
                USDService.ServiceName = "Перевод на карту";
                USDService.ServiceNumber = USDAccount.AccountNumber;
                USDService.ServiceCurrencyId = USDAccount.CurrencyId;

                Account EURAccount = new Account();
                EURAccount.Id = Guid.NewGuid().ToString();
                EURAccount.CardNumber = CardNumberGenerator();
                var EURquery = from n in _db.CurrencyRates where n.CurrencyCode == "EUR" select n;
                EURAccount.CurrencyId = EURquery.FirstOrDefault<CurrencyRates>().Id;
                EURAccount.CardType = "Maestro";
                EURAccount.ExpDate = new DateTime(2018, 12, 31);
                EURAccount.MoneyAmount = 100;
                EURAccount.AccountNumber = Guid.NewGuid().ToString();
                EURAccount.OwnerId = ownerId;
                EURAccount.IsLocked = false;

                Services EURService = new Services();
                EURService.Id = Guid.NewGuid().ToString();
                EURService.ServiceName = "Перевод на карту";
                EURService.ServiceNumber = EURAccount.AccountNumber;
                EURService.ServiceCurrencyId = EURAccount.CurrencyId;

                _db.Account.AddObject(BYNAccount);
                _db.Account.AddObject(USDAccount);
                _db.Account.AddObject(EURAccount);
                _db.Services.AddObject(BYNService);
                _db.Services.AddObject(USDService);
                _db.Services.AddObject(EURService);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string CardNumberGenerator()
        {
            Random rnd = new Random();
            string number;
            int sum = 0;
            Thread.Sleep(700);
            string first = rnd.Next(10000000, 99999999).ToString();
            string second =  rnd.Next(1000000, 9999999).ToString();
            string third;
            number = first + second;
            for (int i = 1; i < 14; i += 2 )
            {
                int element = (int) Char.GetNumericValue(number[i]) * 2;
                if (element > 9)
                {
                    sum += element - 9;
                }
                else
                {
                    sum += element;
                }
            }
            for (int i = 0; i < 15; i += 2)
            {
                int element = (int) Char.GetNumericValue(number[i]);
                sum += element;
            }
            third = (10 - (sum%10)).ToString() == "10" ? "0" : (10 - (sum%10)).ToString();
            number += third;
            string result = "";
            result += number.Substring(0, 4);
            result += "XXXXXXXXXX";
            result += number.Substring(14, 2);
            return result;
        }
    }
}