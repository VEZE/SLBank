using System;
using System.Linq;

namespace InternetBankLemonade.Models
{
    public class TransactionModel
    {
        MainEntities _db = new MainEntities();
        CurrencyModel _currencyModel = new CurrencyModel();
        OrderModel _orderModel = new OrderModel();
        private DateTime _orderDate = DateTime.Now;
        private Helpers.Crypt _crypt = new Helpers.Crypt();
        public string TransferToAccount(string email, string fromCardNumber, string toCardNumber, double moneyAmount)
        {
            try
            {
                moneyAmount = Math.Abs(moneyAmount);
                var ownerResult = (from u in _db.Users where u.Email == email select u).FirstOrDefault().Id;
                var fromAccount = (from a in _db.Account
                                     where a.CardNumber == fromCardNumber && a.OwnerId == ownerResult
                                    select a).FirstOrDefault();
                var toAccount = (from a in _db.Account
                                  where a.CardNumber == toCardNumber && a.OwnerId == ownerResult
                                  select a).FirstOrDefault();
                var fromCurrency =
                    (from c in _db.CurrencyRates where c.Id == fromAccount.CurrencyId select c).FirstOrDefault();

                var fromCurrencyCode = fromCurrency.CurrencyCode;
                var toCurrency =
                    (from c in _db.CurrencyRates where c.Id == toAccount.CurrencyId select c).FirstOrDefault();
                var toCurrencyCode = toCurrency.CurrencyCode;
                var serviceId =
                    (from s in _db.Services where s.ServiceNumber == toAccount.AccountNumber select s).FirstOrDefault()
                        .Id;
                var serviceId2 = (from s in _db.Services where s.ServiceNumber == fromAccount.AccountNumber select s).FirstOrDefault()
                        .Id;
                double excangedMoneyAmount = _currencyModel.Exchange(moneyAmount, fromCurrencyCode, toCurrencyCode);
                if (excangedMoneyAmount != 0)
                {
                    if (fromAccount.MoneyAmount >= excangedMoneyAmount)
                    {
                        fromAccount.MoneyAmount -= excangedMoneyAmount;
                        toAccount.MoneyAmount += moneyAmount;
                        _db.SaveChanges();
                        
                        _orderModel.AddTransactionRecord(ownerResult, serviceId, fromAccount.Id, _orderDate,
                            DateTime.Now, "-", fromCurrency.Id, excangedMoneyAmount, "Успешно");
                        _orderModel.AddTransactionRecord(ownerResult, serviceId2, toAccount.Id, _orderDate, DateTime.Now, "+" , toCurrency.Id, moneyAmount,
                            "Успешно");
                        return "Success";
                    }
                    return "NoMoney";
                }
            }
            catch
            {
                return "Error";
            }
            return "Error";
        }


        public string CheckCards(string email ,string fromCardNumber, string toCardNumber)
        {
            try
            {
                var ownerQuery = from u in _db.Users where u.Email == email select u;
                var ownerResult = ownerQuery.FirstOrDefault();
                if (ownerResult == null)
                    return "";
                string ownerId = ownerResult.Id;
                var fromCardQuery = from a in _db.Account
                                    where a.CardNumber == fromCardNumber && a.OwnerId == ownerId
                                    select a;
                var toCardQuery = from a in _db.Account
                                  where a.CardNumber == toCardNumber && a.OwnerId == ownerId
                                  select a;
                var fromCardResult = fromCardQuery.FirstOrDefault();
                var toCardResult = toCardQuery.FirstOrDefault();
                if (fromCardResult == null || toCardResult == null)
                    return "";

                if (fromCardResult.IsLocked || toCardResult.IsLocked)
                    return "";
                if (fromCardResult.ExpDate < DateTime.Now || toCardResult.ExpDate < DateTime.Now)
                    return "";
                var codeQuery = from c in _db.CurrencyRates where c.Id == toCardResult.CurrencyId select c;
                var codeResult = codeQuery.FirstOrDefault();
                if (codeResult == null)
                    return "";
                return codeResult.CurrencyCode;
            }
            catch
            {
                return "";
            }
        }

        public string GetSessionKeyNumber(string email)
        {
            _orderDate = DateTime.Now;
            Random rnd = new Random();
            try
            {
                var ownerQuery = from u in _db.Users where u.Email == email select u;
                var scopeId = ownerQuery.FirstOrDefault().SessionKeysScopeId;
                var sessionKeys = (from s in _db.SessionKeys where s.Id == scopeId select s).FirstOrDefault();
                if (sessionKeys == null)
                {
                    return "";
                }
                if (sessionKeys.LastKeyName == null)
                {
                    sessionKeys.LastKeyName = ("Key" + rnd.Next(1, 11));
                    _db.SaveChanges();
                    return sessionKeys.LastKeyName.Substring(3);
                }
                    return sessionKeys.LastKeyName.Substring(3);
            }
            catch
            {
                return "";
            }
        }

        public string CheckSessionKey(string email, string input, string strKeyNumber)
        {
            try
            {
                int keyNumber = Convert.ToInt32(strKeyNumber);
                var ownerQuery = from u in _db.Users where u.Email == email select u;
                var scopeId = ownerQuery.FirstOrDefault().SessionKeysScopeId;
                var sessionKeys = (from s in _db.SessionKeys where s.Id == scopeId select s).FirstOrDefault();
                if (sessionKeys == null)
                {
                    return "";
                }
                switch (keyNumber)
                {
                    case 1:
                        if (_crypt.IsMatch(input, sessionKeys.Key1))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }
                        return "InvalidKey";
                   case 2:
                        if (_crypt.IsMatch(input, sessionKeys.Key2))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }
                        return "InvalidKey";
                        
                        
                    case 3:
                        if (_crypt.IsMatch(input, sessionKeys.Key3))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }

                        return "InvalidKey";
                        
                        
                    case 4:
                        if (_crypt.IsMatch(input, sessionKeys.Key4))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }

                        return "InvalidKey";
                        
                    case 5:
                        if (_crypt.IsMatch(input, sessionKeys.Key5))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }

                        return "InvalidKey";
                        
                    case 6:
                        if (_crypt.IsMatch(input, sessionKeys.Key6))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }

                        return "InvalidKey";
                        
                    case 7:
                        if (_crypt.IsMatch(input, sessionKeys.Key7))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }

                        return "InvalidKey";
                        
                    case 8:
                        if (_crypt.IsMatch(input, sessionKeys.Key8))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }


                        return "InvalidKey";
                        
                    case 9:
                        if (_crypt.IsMatch(input, sessionKeys.Key9))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }

                        return "InvalidKey";
                        
                    case 10:
                        if (_crypt.IsMatch(input, sessionKeys.Key10))
                        {
                            sessionKeys.LastKeyName = null;
                            _db.SaveChanges();
                            return "Success";
                        }

                        return "InvalidKey";
                        ;
                    default:
                        return "";
                }
            }
            catch
            {
                return "";
            }
        }
    }
}