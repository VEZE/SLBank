using System;
using System.Linq;
using System.Collections.Generic;


namespace InternetBankLemonade.Models
{
    public class CurrencyModel
    {
        MainEntities _db = new MainEntities();
        public bool AddOrUpdateCurrencies(double byn = 1, double eur = 2.2, double usd = 1.98)
        {
            CurrencyRates BYN = new CurrencyRates();
            CurrencyRates EUR = new CurrencyRates();
            CurrencyRates USD = new CurrencyRates();
            try
            {
                var BYNquery = (from c in _db.CurrencyRates where c.CurrencyCode == "BYN" select c).FirstOrDefault();
                if (BYNquery == null)
                {
                    BYN.Id = Guid.NewGuid().ToString();
                    BYN.CurrencyCode = "BYN";
                    BYN.AmountInBaseCurrency = byn;
                    BYN.CurrencySetDate = DateTime.Now;
                    _db.CurrencyRates.AddObject(BYN);
                }
                else
                {
                    BYNquery.AmountInBaseCurrency = byn;
                    BYNquery.CurrencySetDate = DateTime.Now;
                }

                var EURquery = (from c in _db.CurrencyRates where c.CurrencyCode == "EUR" select c).FirstOrDefault();
                if (EURquery == null)
                {
                    EUR.Id = Guid.NewGuid().ToString();
                    EUR.CurrencyCode = "EUR";
                    EUR.AmountInBaseCurrency = eur;
                    EUR.CurrencySetDate = DateTime.Now;
                    _db.CurrencyRates.AddObject(EUR);
                }
                else
                {
                    EURquery.AmountInBaseCurrency = eur;
                    EURquery.CurrencySetDate = DateTime.Now;
                }

                var USDquery = (from c in _db.CurrencyRates where c.CurrencyCode == "USD" select c).FirstOrDefault();
                if (USDquery == null)
                {
                    USD.Id = Guid.NewGuid().ToString();
                    USD.CurrencyCode = "USD";
                    USD.AmountInBaseCurrency = usd;
                    USD.CurrencySetDate = DateTime.Now;
                    _db.CurrencyRates.AddObject(USD);
                }
                else
                {
                    USDquery.AmountInBaseCurrency = usd;
                    USDquery.CurrencySetDate = DateTime.Now;
                }
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public double Exchange(double moneyAmount, string fromCurrency, string toCurrency)
        {
            try
            {
                var fromResult = (from c in _db.CurrencyRates where c.CurrencyCode == fromCurrency select c).FirstOrDefault().AmountInBaseCurrency;
                var toResult = (from c in _db.CurrencyRates where c.CurrencyCode == toCurrency select c).FirstOrDefault().AmountInBaseCurrency;
                double rate = toResult/fromResult;
                double result = moneyAmount*rate;
                return Math.Round(result, 2);
            }
            catch
            {
                return 0;
            }
        }

        public List<CurrencyRates> GetAll()
        {
            List<CurrencyRates> result = new List<CurrencyRates>();
            try
            {
                var query = (from c in _db.CurrencyRates select c).ToList();
                return query;
            }
            catch
            {
                return result;
            }
        }

    }
}