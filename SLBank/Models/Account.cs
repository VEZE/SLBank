using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetBankLemonade.Models
{
    public partial class Account
    {
        public string CurrencyCode { get; set; }
        public string FromCard { get; set; }
        public string ToCard { get; set; }
        public string SessionKey { get; set; }
        public string CardId { get; set; }
    }
}