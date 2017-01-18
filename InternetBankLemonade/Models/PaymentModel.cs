using System;
using System.Collections.Generic;
using System.Linq;

namespace InternetBankLemonade.Models
{
    public class PaymentModel
    {
        const string EXCLUDING = "Перевод на карту";
        const string EXCLUDING_USD = "Зачисление на карту(USD)";
        const string EXCLUDING_BYN = "Зачисление на карту(BYN)";
        const string EXCLUDING_EUR = "Зачисление на карту(EUR)";
        MainEntities _db = new MainEntities();
        CurrencyModel _currencyModel = new CurrencyModel();
        OrderModel _orderModel = new OrderModel();
        private DateTime _orderDate = DateTime.Now;
        public List<Services> GetAllPaymentServices()
        {
            List<Services> result = new List<Services>();
            try
            {
                var query = (from s in _db.Services where s.ServiceName != EXCLUDING
                             && s.ServiceName != EXCLUDING_BYN && s.ServiceName != EXCLUDING_EUR && s.ServiceName != EXCLUDING_USD select s).ToList();
                return query;
            }
            catch
            {
                return result;
            }
        }

        public string CheckPayment(string email, string serviceId)
        {
            try
            {
                var query = (from s in _db.Services where s.Id == serviceId select s).ToList();
                if (query.Count() == 1)
                {
                    return query[0].ServiceNumber;
                }
                return "Error";
            }
            catch
            {
                return "Error";
            }
        }

        public string Pay(string email, string cardNumber, string number, string serviceId, double moneyAmount)
        {
            try
            {
                moneyAmount = Math.Abs(moneyAmount);
                var ownerId = (from u in _db.Users where u.Email == email select u).FirstOrDefault().Id;
                var account = (from a in _db.Account where a.CardNumber == cardNumber select a).FirstOrDefault();
                var service = (from s in _db.Services where s.Id == serviceId select s).FirstOrDefault();
                var currency = (from c in _db.CurrencyRates where c.Id == account.CurrencyId select c).FirstOrDefault();
                double exchangedMoney = _currencyModel.Exchange(moneyAmount, currency.CurrencyCode, "BYN");
                if (account.MoneyAmount < exchangedMoney)
                    return "NoMoney";
                account.MoneyAmount -= exchangedMoney;
                _orderModel.AddTransactionRecord(ownerId, serviceId, account.Id, DateTime.Now, _orderDate, "-",
                    currency.Id, exchangedMoney, "Успешно", number);
                _db.SaveChanges();
                return "Success";
            }
            catch
            {
                return "Error";
                
            }
        }
    }
}