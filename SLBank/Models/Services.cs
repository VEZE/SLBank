using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetBankLemonade.Models
{
    public partial class Services
    {
        public string FromCard { get; set; }
        public string SessionKey { get; set; }
        public string Number { get; set; }
        public double MoneyAmount { get; set; }
        public bool AutoPayCheckbox { get; set; }

    }
}