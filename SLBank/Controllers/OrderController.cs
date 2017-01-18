using System;
using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class OrderController : Controller
    {
        Orders _orders = new Orders();
        OrderModel _orderModel = new OrderModel();
        AuthModel _auth = new AuthModel();
        DashboardModel _dashboardModel = new DashboardModel();

        [HttpGet]
        public ActionResult Index()
        {
            if (!_auth.IsAdmin(User.Identity.Name))
                return View(_dashboardModel.GetCards(User.Identity.Name));
            return RedirectToAction("Page404", "Error");
        }

        [HttpGet]
        public ActionResult Get(Orders model, int? page)
        {
            if (!_auth.IsAdmin(User.Identity.Name))
            {
                int pageNumber = (page ?? 1);
                if (model.FirstDate != Convert.ToDateTime("01.01.0001 0:00:00"))
                {
                    Session["StartDate"] = model.FirstDate;
                    Session["EndDate"] = model.SecondDate;
                }
                if (Session["StartDate"] != null && Session["EndDate"] != null)
                {
                    var result = _orderModel.GetOrders(User.Identity.Name, Convert.ToDateTime(Session["StartDate"]), Convert.ToDateTime(Session["EndDate"]), model.CardNumber, pageNumber);
                    return View(result);
                }
                else
                {
                    var result = _orderModel.GetOrders(User.Identity.Name, model.FirstDate, model.SecondDate, model.CardNumber, pageNumber);
                    return View(result);
                }
            }
            return RedirectToAction("Page404", "Error");
            
        }
    }
}