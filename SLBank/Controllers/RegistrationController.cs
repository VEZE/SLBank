using System.Web.Mvc;
using InternetBankLemonade.Models;

namespace InternetBankLemonade.Controllers
{
    public class RegistrationController : Controller
    {
        MainEntities _db = new MainEntities();
        RegistrationModel _registrationModel = new RegistrationModel();

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(_db.Users);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Users model)
        {
            if (model.ConfirmEmail == model.Email && model.Password == model.ConfirmPassword) //TODO: Move it to frontend!!
            {
                Session["Name"] = model.Name;
                Session["Surname"] = model.Surname;
                Session["LastName"] = model.LastName;

                string result = _registrationModel.RegisterUser(model.PassportNumber, model.Name, model.Surname,
                    model.LastName,
                    model.Password,
                    model.Email, model.Telephone);
                if (result == "PassNull")
                {
                    Session["Email"] = model.Email;
                    Session["Tel"] = model.Telephone;
                    Session["Response"] = "Ваш паспорт не зарегистрирован";
                    return RedirectToAction("Index", "Registration");
                }
                if (result == "IsActive")
                {
                    Session["Email"] = model.Email;
                    Session["Tel"] = model.Telephone;
                    Session["Response"] = "Пользователь с данным паспортом уже зарегистрирован";
                    return RedirectToAction("Index", "Registration");
                }
                if (result == "Exist")
                {
                    Session["Passport"] = model.PassportNumber;
                    Session["Response"] = "Пользователь с данным Email/Телефоном уже зарегистрирован";
                    return RedirectToAction("Index", "Registration");
                }
                if (result == "Success")
                {
                    Session["ShowKeys"] = "true";
                    return RedirectToAction("Index", "Login"); //TODO: Need redirect to login if success and redirect if error
                }
                if (result == "Error")
                {
                    Session["Response"] = "Ошибка сервера";
                    return RedirectToAction("Index", "Registration");
                }
                
            }
            return RedirectToAction("Error", "Error");  //TODO: redirect to error page
        }
    }
}