using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;

namespace InternetBankLemonade.Models
{
    public class OrderModel
    {
        MainEntities _db = new MainEntities();
        private const int RECORDS_PER_PAGE = 10;
        
        public bool AddTransactionRecord(string ownerId, string serviceId, string accountId,
            DateTime orderDate, DateTime transactionDate, string sign, string currencyId, double moneyAmount, string status, string number = "---")
        {
            
            Orders order = new Orders();
            order.Id = Guid.NewGuid().ToString();
            order.OwnerId = ownerId;
            order.ServiceId = serviceId;
            order.AccountId = accountId;
            order.OrderDate = orderDate;
            order.CurrencyId = currencyId;
            order.TransactionDate = transactionDate;
            order.Sign = sign;
            order.MoneyAmount = moneyAmount;
            order.Status = status;
            order.Number = number;
            _db.Orders.AddObject(order);
            try
            {
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<Orders> GetOrders(string email, DateTime fromDate, DateTime toDate, string cardNumber1, int pageNumber = 1)
        {
            List<Orders> result = new List<Orders>();
            toDate = toDate.AddHours(23).AddMinutes(59).AddSeconds(59);
            List<Orders> orders;
            try
            {
                var ownerId = (from u in _db.Users where u.Email == email select u).FirstOrDefault().Id;
                if (cardNumber1 == "Все карты" || cardNumber1 == null)
                {
                    orders = (from o in _db.Orders
                                  where o.OwnerId == ownerId && o.TransactionDate >= fromDate &&
                                      o.TransactionDate <= toDate
                                  select o).ToList();
                }
                else
                {
                    var accId = (from a in _db.Account where a.CardNumber == cardNumber1 select a).FirstOrDefault().Id;
                    orders = (from o in _db.Orders
                                  where o.OwnerId == ownerId && o.TransactionDate >= fromDate &&
                                      o.TransactionDate <= toDate && o.AccountId == accId
                                  select o).ToList();
                }
                if (orders.Count != 0)
                {
                    foreach (var item in orders)
                    {
                        var serviceName =
                            (from s in _db.Services where s.Id == item.ServiceId select s).FirstOrDefault().ServiceName;
                        var account =
                            (from a in _db.Account where a.Id == item.AccountId select a).FirstOrDefault();
                        var accountNumber = account.AccountNumber;
                        var cardNumber = account.CardNumber;
                        var currencyName =
                            (from c in _db.CurrencyRates where c.Id == item.CurrencyId select c).FirstOrDefault()
                                .CurrencyCode;
                        
                        result.Add(new Orders()
                        {
                            Id = item.Id,OwnerId = item.OwnerId,ServiceId = item.ServiceId,AccountId = item.AccountId,OrderDate = item.OrderDate,
                            TransactionDate = item.TransactionDate,CurrencyId = item.CurrencyId,
                            MoneyAmount = item.MoneyAmount,Status = item.Status,AccountNumber = cardNumber,
                            ServiceName = serviceName,CurrencyName = currencyName, Sign = item.Sign, Number = item.Number
                        });

                    }
                    result.Sort((p, q) => q.TransactionDate.CompareTo(p.TransactionDate));
                    return result.ToPagedList(pageNumber, RECORDS_PER_PAGE);
                }
            }
            catch
            {
                return result.ToPagedList(pageNumber, RECORDS_PER_PAGE);
            }
            return result.ToPagedList(pageNumber, RECORDS_PER_PAGE);
        }
    }
}