using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InternetBankLemonade.Models
{
    public partial class Users
    {
        public string EmailNew { get; set; }
        public string ConfirmEmail { get; set; }
        public string PasswordNew { get; set; }
        public string ConfirmPassword { get; set; }
        public string Password { get; set; }
        public string PassportNumber { get; set; }

    }
}