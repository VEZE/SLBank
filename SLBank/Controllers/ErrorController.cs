using System.Web.Mvc;

namespace InternetBankLemonade.Controllers
{
    public class ErrorController : Controller
    {

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Page404()
        {
            return View();
        }
    }
}