using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class AutoPayController : Controller
    {
        AutoPayModel _autoPayModel = new AutoPayModel();
        AuthModel _auth = new AuthModel();

        [AllowAnonymous]
        [HttpGet]
        public ActionResult PayAll()
        {
            _autoPayModel.PayAll();
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public ActionResult GetAll(int? page)
        {
            if (!_auth.IsAdmin(User.Identity.Name))
            {
                int pageNumber = (page ?? 1);
                return View(_autoPayModel.GetAll(User.Identity.Name,pageNumber));
            }
            return RedirectToAction("Page404", "Error");
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (_autoPayModel.Delete(id))
            {
                Session["Success"] = "Удалено";
                return RedirectToAction("GetAll", "AutoPay");
            }
            Session["Error"] = "Ошибка";
            return RedirectToAction("GetAll", "AutoPay");
        }
    }
}