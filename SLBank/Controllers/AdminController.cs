using System;
using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class AdminController : Controller
    {
        private MainEntities _db = new MainEntities();
        private AdminModel _adminModel = new AdminModel();
        private AuthModel _auth = new AuthModel();
        private CurrencyModel _currencyModel = new CurrencyModel();



        
        [HttpGet]
        public ActionResult Passport()
        {
            if (_auth.IsAdmin(User.Identity.Name))
                return View(_db.Passports);
            return RedirectToAction("Page404", "Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Passport(Passports model)
        {
            if (!String.IsNullOrEmpty(model.PassportNumber) && _adminModel.AddPassportNumber(model.PassportNumber))
            {
                Session["Success"] = "1";
                return RedirectToAction("Passport", "Admin");
            }
            Session["Error"] = "1";
            return RedirectToAction("Passport", "Admin");
        }

        [HttpGet]
        public ActionResult Currency()
        {
            if (_auth.IsAdmin(User.Identity.Name))
                return View(_currencyModel.GetAll());
            return RedirectToAction("Page404", "Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Currency(CurrencyRates model)
        {
            if (_currencyModel.AddOrUpdateCurrencies(model.BYN, model.EUR, model.USD))
            {
                Session["Success"] = "Обновлено";
                return RedirectToAction("Currency", "Admin");
            }
            Session["Error"] = "Ошибка";
            return RedirectToAction("Currency", "Admin");
        }

        [HttpGet]
        public ActionResult Search()
        {
            if (_auth.IsAdmin(User.Identity.Name))
                return View();
            return RedirectToAction("Page404", "Error");
        }


        [ValidateAntiForgeryToken]
        public ActionResult Cards(Passports model)
        {
            if (_auth.IsAdmin(User.Identity.Name))
            {
                if (_adminModel.IsPassportOnAdmin(model.PassportNumber))
                {
                    Session["Error"] = "Паспорт не найден";
                    return RedirectToAction("Search", "Admin");
                }
                Session["UserId"] = _adminModel.GetUserIdByPassportNumber(model.PassportNumber);
                var result = _adminModel.GetCardsByPassport(model.PassportNumber);
                if (result.Count < 1)
                {
                    Session["Error"] = "Пользователь не найден";
                    return RedirectToAction("Search", "Admin");
                }
                return View(result);
            }
                
            return RedirectToAction("Page404", "Error");
        }

        public ActionResult Add(Account model)
        {
            Session["CurrencyId"] = _adminModel.GetCurrencyIdByCardId(model.CardId);
            if (_adminModel.AddToAccount(model.CardId, model.MoneyAmount, Session["UserId"].ToString(), Session["CurrencyId"].ToString()))
            {
                Session["Success"] = "Зачислено";
                return RedirectToAction("Search", "Admin");
            }
            Session["Error"] = "Ошибка";
            return RedirectToAction("Search", "Admin");
        }

        [HttpGet]
        public ActionResult SearchKey()
        {
            if (!_auth.IsAdmin(User.Identity.Name))
                return RedirectToAction("Page404", "Error");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Keys(Passports model)
        {
            if (_adminModel.UpdateKeys(model.PassportNumber))
            {
                if (_adminModel.IsPassportOnAdmin(model.PassportNumber))
                {
                    Session["Error"] = "Паспорт не найден";
                    return RedirectToAction("Search", "Admin");
                }
                Session["Success"] = "Ключи обновлены";
                return RedirectToAction("SearchKey", "Admin");
            }
            Session["Error"] = "Пользователь не существует";
            return RedirectToAction("SearchKey", "Admin");
        }

        
        
    }
}