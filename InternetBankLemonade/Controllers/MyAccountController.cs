using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class MyAccountController : Controller
    {
        MyAccountModel _myAccountModel = new MyAccountModel();
        AuthModel _auth = new AuthModel();

        [HttpGet]
        public ActionResult Index()
        {
            return View(_myAccountModel.GetUser(User.Identity.Name));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pass(Users model)
        {
                if (model.EmailNew == model.ConfirmEmail && model.PasswordNew == model.ConfirmPassword)
                {
                    string result = _myAccountModel.UpdateUserInformation(User.Identity.Name, model.Password, model.PasswordNew, model.EmailNew, true);
                    if (result == "UserNA")
                    {
                        Session["Response"] = "Пользователь не найден";
                        return RedirectToAction("Index", "MyAccount");
                    }
                    if (result == "PassError")
                    {
                        Session["Response"] = "Текущий пароль неверен";
                        return RedirectToAction("Index", "MyAccount");
                    }
                    if (result == "Success")
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                    if (result == "Error")
                    {
                        return RedirectToAction("Error", "Error");
                    }
                    if (result == "ExPass")
                    {
                        Session["Response"] = "Вы ввели действующий пароль";
                        return RedirectToAction("Index", "MyAccount");
                    }

                }
                Session["Response"] = "Пароли/ящики не совпадают";
                return RedirectToAction("Index", "MyAccount");
            
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Email(Users model)
        {
                if (model.EmailNew == model.ConfirmEmail && model.PasswordNew == model.ConfirmPassword)
                {
                    string result = _myAccountModel.UpdateUserInformation(User.Identity.Name, model.Password, model.PasswordNew, model.EmailNew);
                    if (result == "UserNA")
                    {
                        Session["Response"] = "Пользователь не найден";
                        return RedirectToAction("Index", "MyAccount");
                    }
                    if (result == "PassError")
                    {
                        Session["Response"] = "Текущий пароль неверен";
                        return RedirectToAction("Index", "MyAccount");
                    }
                    if (result == "Success")
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                    if (result == "Error")
                    {
                        return RedirectToAction("Error", "Error");
                    }

                }
                Session["Response"] = "Пароли/ящики не совпадают";
                return RedirectToAction("Index", "MyAccount");
            
        }
    }
}