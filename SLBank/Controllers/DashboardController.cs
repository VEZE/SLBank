using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class DashboardController : Controller
    {
        DashboardModel _model = new DashboardModel();

        [HttpGet]
        public ActionResult Index()
        {
           return View(_model.GetCards(User.Identity.Name));
        }
    }
}