using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class TransactionController : Controller
    {
        MainEntities _db = new MainEntities();
        DashboardModel dashboardModel = new DashboardModel();
        TransactionModel transactionModel = new TransactionModel();
        LoginModel _loginModel = new LoginModel();
        AuthModel _auth = new AuthModel();

        [HttpGet]
        public ActionResult Index()
        {
            if (_auth.IsAdmin(User.Identity.Name))
                return RedirectToAction("Page404", "Error");
            Session["TransactionStep"] = "First";
            return View(dashboardModel.GetCards(User.Identity.Name));
        }


        [ValidateAntiForgeryToken]
        public ActionResult Continue(Account model)
        {
            if (_auth.IsAdmin(User.Identity.Name))
                return RedirectToAction("Page404", "Error");
            if (Session["TransactionStep"] != null && Session["TransactionStep"].ToString() == "First")
            {
                if (model.FromCard == model.ToCard)
                {
                    Session["Response"] = "Карта назначения должна отличаться от карты списания";
                    return RedirectToAction("Index", "Transaction");
                }
                var currencyCode = transactionModel.CheckCards(User.Identity.Name, model.FromCard, model.ToCard);
                if (currencyCode == "")
                {
                    Session["Response"] = "Карточки не верны";
                    return RedirectToAction("Index", "Transaction");
                }
                    
                Session["FromCard"] = model.FromCard;
                Session["ToCard"] = model.ToCard;
                Session["CurrencyCode"] = currencyCode;
                Session["SessionKeyNumber"] = transactionModel.GetSessionKeyNumber(User.Identity.Name);
                if (Session["SessionKeyNumber"] != null && Session["SessionKeyNumber"].ToString() == "")
                {
                    Session["Response"] = "Ошибка сервера";
                    return RedirectToAction("Index", "Transaction");
                }
                Session["TransactionStep"] = "Second";
                return View();
            }
            Session["TransactionStep"] = null;
            return RedirectToAction("Error", "Error"); // TODO: Redirect to error
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EnterAmount(Account model)
        {
            if (Session["TransactionStep"] != null && Session["TransactionStep"].ToString() == "Second")
            {
                var result = transactionModel.CheckSessionKey(User.Identity.Name, model.SessionKey, Session["SessionKeyNumber"].ToString());
                if (result == "Success")
                {
                    string transResult = transactionModel.TransferToAccount(User.Identity.Name,
                        Session["FromCard"].ToString(),
                        Session["ToCard"].ToString(), model.MoneyAmount);
                    if (transResult == "Success")
                    {
                        Session["TransactionStep"] = null;
                        Session["FromCard"] = null;
                        Session["ToCard"] = null;
                        Session["CurrencyCode"] = null;
                        Session["SessionKeyNumber"] = null;
                        return RedirectToAction("Success", "Success");
                    }
                    if (transResult == "NoMoney")
                    {
                        Session["Response"] = "Недостаточно средств";
                        return RedirectToAction("Index", "Transaction");
                    }
                }
                else if (result == "InvalidKey")
                {
                    
                    if (_loginModel.FailCountInc(User.Identity.Name) == "Locked")
                    {
                        Session["Response"] = "Ваш аккаунт заблокирован";
                        return RedirectToAction("Logout", "Login");
                    }
                    Session["Response"] = "Неправильный ключ";
                    return RedirectToAction("Index", "Transaction");

                }
            }
            Session["TransactionStep"] = null;
            return RedirectToAction("Error", "Error");  //TODO: redirect to error
        }
    }
}