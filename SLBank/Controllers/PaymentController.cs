using System.Linq;
using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class PaymentController : Controller
    {

        private PaymentModel _paymentModel = new PaymentModel();
        private Services _services = new Services();
        private TransactionModel _transactionModel = new TransactionModel();
        private DashboardModel _dashboardModel = new DashboardModel();
        private AutoPayModel _autoPayModel = new AutoPayModel();
        AuthModel _auth = new AuthModel();

        [HttpGet]
        public ActionResult Index()
        {
            if (!_auth.IsAdmin(User.Identity.Name))
            {
                var result = _paymentModel.GetAllPaymentServices();
                var cards = _dashboardModel.GetCards(User.Identity.Name);
                if (result.Count() != 0 && cards.Count() != 0)
                {
                    Session["Cards"] = cards;
                    Session["CheckPayment"] = "true";
                    return View(result);
                }
                return RedirectToAction("Error", "Error");
            }
            return RedirectToAction("Page404", "Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckPayment(Services model)
        {
            if (Session["CheckPayment"] != null && Session["CheckPayment"].ToString() == "true")
            {
                Session["CheckPayment"] = null;
                if (model.Id != "")
                {
                    string result = _paymentModel.CheckPayment(User.Identity.Name, model.Id);
                    if (result == "Error" || result == "")
                        return RedirectToAction("Error", "Error");
                    Session["ServiceId"] = model.Id;
                    Session["CheckPayment"] = "Send";
                    Session["KeyNumber"] = _transactionModel.GetSessionKeyNumber(User.Identity.Name);
                    Session["FromCard"] = model.FromCard;
                    Session["Number"] = model.Number;
                    return View();
                }
            }
            return RedirectToAction("Error", "Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Send(Services model)
        {
            if (Session["CheckPayment"] != null && Session["CheckPayment"].ToString() == "Send")
            {
                Session["CheckPayment"] = null;
               string result = _transactionModel.CheckSessionKey(User.Identity.Name, model.SessionKey, Session["KeyNumber"].ToString());
                if (result == "Success")
                {
                    string payResult = _paymentModel.Pay(User.Identity.Name, Session["FromCard"].ToString(),
                        Session["Number"].ToString(),
                        Session["ServiceId"].ToString(), model.MoneyAmount);
                    if (payResult == "Success")
                    {
                        if (model.AutoPayCheckbox)
                            _autoPayModel.Add(User.Identity.Name, Session["FromCard"].ToString(),
                        Session["Number"].ToString(),
                        Session["ServiceId"].ToString(), model.MoneyAmount);
                        return RedirectToAction("Success", "Success");
                    }
                    if (payResult == "NoMoney")
                    {
                        Session["Response"] = "Недостаточно средств";
                        return RedirectToAction("Index", "Payment");
                    }
                        
                    return RedirectToAction("Error", "Error");
                }
                if (result == "InvalidKey")
                {
                    Session["Response"] = "Неверный ключ";
                    return RedirectToAction("Index", "Payment");
                }
            }
            return RedirectToAction("Error", "Error");
        }
    }
}