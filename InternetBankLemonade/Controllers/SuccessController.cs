using System.Web.Mvc;

namespace InternetBankLemonade.Controllers
{
    public class SuccessController : Controller
    {

        [AllowAnonymous]
        public ActionResult Success()
        {
            return View();
        }
    }
}