using System;
using System.Linq;
using System.Collections.Generic;
using PagedList;

namespace InternetBankLemonade.Models
{
    public class AutoPayModel
    {
        MainEntities _db = new MainEntities();
        AutoPay autoPay = new AutoPay();
        PaymentModel _paymentModel = new PaymentModel();
        private const int RECORDS_PER_PAGE = 10;

        public void Add(string email, string fromCard, string number, string serviceId, double moneyAmount)
        {
            try
            {
                autoPay.Id = Guid.NewGuid().ToString();
                autoPay.Email = email;
                autoPay.CardNumber = fromCard;
                autoPay.Number = number;
                autoPay.ServiceId = serviceId;
                autoPay.MoneyAmount = moneyAmount;
                autoPay.CreationDate = DateTime.Now;
                _db.AutoPay.AddObject(autoPay);
                _db.SaveChanges();
            }
            catch
            {
                
            }
        }

        public void PayAll()
        {
            try
            {
                var list = (from au in _db.AutoPay select au).ToList();
                foreach (var item in list)
                {
                    if (item.CreationDate.Date.AddDays(30) < DateTime.Now.Date)
                    {
                        string result = _paymentModel.Pay(item.Email, item.CardNumber, item.Number, item.ServiceId, item.MoneyAmount);
                        if (result == "Success")
                            UpdateDateAfterPay(item.Email, item.CardNumber, item.Number, item.ServiceId, item.MoneyAmount);
                    }
                        
                }
                
            }
            catch
            {

            }
        }

        public void UpdateDateAfterPay(string email, string cardNumber, string number, string serviceId, double moneyAmount)
        {
            try
            {
                var result = (from au in _db.AutoPay where au.Email == email && au.CardNumber == cardNumber && au.Number == number && au.ServiceId == serviceId && au.MoneyAmount == moneyAmount select au).FirstOrDefault();
                result.CreationDate = DateTime.Now;
                _db.SaveChanges();
            }
            catch
            {

            }
        }

        public IEnumerable<AutoPay> GetAll(string email, int pageNumber)
        {
            List<AutoPay> list = new List<AutoPay>();
            try
            {
                var result = (from au in _db.AutoPay where au.Email == email select au).ToList();
                foreach(var item in result)
                {
                    var serviceName = (from s in _db.Services where s.Id == item.ServiceId select s).FirstOrDefault().ServiceName;
                    list.Add(new AutoPay()
                    {
                        Id = item.Id,
                        Email = item.Email,
                        CardNumber = item.CardNumber,
                        Number = item.Number,
                        ServiceId = serviceName,
                        MoneyAmount = item.MoneyAmount,
                        CreationDate = item.CreationDate

                    });
                    
                }
                list.Sort((p, q) => q.CreationDate.CompareTo(p.CreationDate));
                return list.ToPagedList(pageNumber, RECORDS_PER_PAGE);
            }
            catch
            {
                return list.ToPagedList(pageNumber, RECORDS_PER_PAGE);
            }
            
        }

        public bool Delete(string id)
        {
            try
            {
                var result = (from au in _db.AutoPay where au.Id == id select au).FirstOrDefault();
                _db.AutoPay.DeleteObject(result);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        
    }
}