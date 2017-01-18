using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class CurrencyController : Controller
    {
        CurrencyModel currencyModel = new CurrencyModel();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            currencyModel.AddOrUpdateCurrencies();
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public ActionResult Get()
        {
            var list = currencyModel.GetAll();
            return View(list);
        }
    }
}