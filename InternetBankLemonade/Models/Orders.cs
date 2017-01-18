using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetBankLemonade.Models
{
    public partial class Orders
    {
        public DateTime FirstDate { get; set; }
        public DateTime SecondDate { get; set; }
        public string ConfirmPassword { get; set; }
        public string ServiceName { get; set; }
        public string CurrencyName { get; set; }
        public string AccountNumber { get; set; }
        public string CardNumber { get; set; }
    }
}