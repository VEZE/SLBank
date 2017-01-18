using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class LoginController : Controller
    {
        private MainEntities _db = new MainEntities();
        private LoginModel _loginModel = new LoginModel();
        private RegistrationModel _registrationModel = new RegistrationModel(); //

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Users model)
        {
            string result = _loginModel.LogIn(model.Email, model.Password);
            if (result == "Success")
            {
                var serializedUser = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                var ticket = new FormsAuthenticationTicket(1, model.Email, DateTime.Now, DateTime.Now.AddHours(3), false, serializedUser);
                var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var isSsl = Request.IsSecureConnection;
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                {
                    HttpOnly = true, 
                    Secure = isSsl
                };
                Response.Cookies.Set(cookie);
                if (_loginModel.IsFirstLogin(model.Email))
                {
                    
                    
                    return RedirectToAction("ShowSessionKeys", "Login");
                }
                    
                /*if (Session["ShowKeys"] != null && Session["ShowKeys"].ToString() == "true")
                {
                    //Session["ShowKeys"] = null;
                    //Session["AllowKeys"] = "1";
                    return RedirectToAction("ShowSessionKeys", "Login");
                }*/
                return RedirectToAction("Index", "Dashboard");
            }
            if (result == "Locked")
            {
                Session["Response"] = "Ваш аккаунт заблокирован";
                return RedirectToAction("Index", "Login");
            }
                
            if (result == "Invalid")
            {
                Session["Response"] = "Неверный логин или пароль";
                if (_loginModel.FailCountInc(model.Email) == "Locked")
                    return RedirectToAction("Logout", "Login");
                return RedirectToAction("Index", "Login");
            }
            
            return RedirectToAction("Error", "Error");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Response.Cookies[FormsAuthentication.FormsCookieName].Value = string.Empty;
                return RedirectToAction("Index", "Login");
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult ShowSessionKeys()
        {
            if (_loginModel.IsFirstLogin(User.Identity.Name))
            {
                _loginModel.SetFirstLogin(User.Identity.Name, false);
                var list = _registrationModel.GetKeyList(User.Identity.Name);
                _loginModel.SendEmail(User.Identity.Name, list);
                foreach (var item in list)
                {
                    Session["k1"] = item.Key1;
                    Session["k2"] = item.Key2;
                    Session["k3"] = item.Key3;
                    Session["k4"] = item.Key4;
                    Session["k5"] = item.Key5;
                    Session["k6"] = item.Key6;
                    Session["k7"] = item.Key7;
                    Session["k8"] = item.Key8;
                    Session["k9"] = item.Key9;
                    Session["k10"] = item.Key10;
                }
                _registrationModel.EncryptKeys(User.Identity.Name);
                Session["Response"] = "Ключи отправлены Вам на почту " + User.Identity.Name;
                return View();
            }
            return RedirectToAction("Page404", "Error");
                
                
            
        }
    }
}