using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using TestWeb.Data;
using TestWeb.Models;

namespace TestWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public string EditUser(string id)
        {
            Register_Input? model = _context.Registers.Find(id);
            if (model == null)
                return "Sucess";
            else
            {
                return "이미 등록된 아이디입니다.";
            }
        }

        public string EditUser_temp(Register model)
        {
            var id = HttpContext.Session.GetString("User");
            if (id == null)
                return "세션 만료";
            Register_Input? user = _context.Registers.FirstOrDefault(x => x.USER_ID == id);
            if (user != null)
            {
                user.NAME = model.NAME;
                user.USER_PWD = model.USER_PWD;
                user.USER_ID = id;
                user.EMAIL_ADRESS = model.EMAIL_ADRESS;
                try
                {
                    _context.Registers.Update(user);
                    _context.SaveChanges();
                    return "Success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "error";
                }
            }
            return "error";
        }

        public IActionResult MyPage()
        {
            string? id = HttpContext.Session.GetString("User");
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Register_Input user = new Register_Input();
                user = _context.Registers.First(x => x.USER_ID.Equals(id));
                Register user2 = new Register();
                user2.USER_ID = user.USER_ID.Trim();
                user2.NAME = user.NAME.Trim();
                user2.EMAIL_ADRESS = user.EMAIL_ADRESS.Trim();
                return View(user2);
            }
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                return RedirectToAction("Index", "Board");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult LoginTest()
        {
            return View();
        }

        //public IActionResult LoginTest(Register model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //if (model.USER_PWD != model.USER_PWD_CHECK)
        //        //{
        //        //    return RedirectToAction("Register", "Home");
        //        //}
        //        Register_Input user = new Register_Input();

        //        user.USER_ID = model.USER_ID;
        //        user.USER_PWD = model.USER_PWD;
        //        user.NAME = model.NAME;
        //        user.EMAIL_ADRESS = model.EMAIL_ADRESS;

        //        try
        //        {
        //            _context.Registers.Add(user);
        //            //_context.Registers.AddRange([user.USER_ID, user.USER_PWD, user.NAME, user.EMAIL_ADRESS]);
        //            _context.SaveChanges();
        //            return RedirectToAction("Login", "Home");
        //            //return null;

        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //            return View();
        //        }
        //    }
        //    ViewData["ErrMsg"] = "입력하신 내용을 확인 후 다시 시도해주십시오";
        //    return View();
        //}

        [HttpPost]
        public IActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                //if (model.USER_PWD != model.USER_PWD_CHECK)
                //{
                //    return RedirectToAction("Register", "Home");
                //}
                Register_Input user = new Register_Input();

                user.USER_ID = model.USER_ID;
                user.USER_PWD = model.USER_PWD;
                user.NAME = model.NAME;
                user.EMAIL_ADRESS = model.EMAIL_ADRESS;

                try
                {
                    _context.Registers.Add(user);
                    //_context.Registers.AddRange([user.USER_ID, user.USER_PWD, user.NAME, user.EMAIL_ADRESS]);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "Home");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return View();
                }
            }
            ViewData["ErrMsg"] = "입력하신 내용을 확인 후 다시 시도해주십시오";
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Login? user = _context.Logins
                    .FirstOrDefault(x => x.USER_ID.Equals(model.USER_ID) && x.USER_PWD.Equals(model.USER_PWD));

                    if (user != null)
                    {
                        HttpContext.Session.SetString("User", model.USER_ID);
                        
                        return RedirectToAction("Index", "Board");
                    }
                    else
                    {
                        return RedirectToAction("Fail", "Account");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Test(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name;
            ViewData["numTimes"] = numTimes;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            using (SqlConnection conn = new SqlConnection(""))
            {

            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string Register_temp(string NAME, string USER_ID, string USER_PWD, string USER_PWD_CHECK, string EMAIL)
        {
            Register_Input user = new Register_Input();

            if (NAME.Length <1)
            {
                return "이름을 입력해 주십시오";
            }
            else if (USER_ID.Length < 1)
            {
                return "아이디를 입력해 주십시오";
            }
            else if (USER_PWD.Length < 1)
            {
                return "비밀번호를 입력해 주십시오";
            }
            else if (USER_PWD != USER_PWD_CHECK)
            {
                return "작성하신 비밀번호와 확인문구가 일치하지 않습니다.";
            }
            else if (EMAIL == null)
            {
                return "이메일을 입력해 주십시오";
            }
            else if (!Regex.IsMatch(EMAIL, @"^[a-zA-Z0-9+-_.]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
            {
                return "이메일형식에 맞게 입력해주십시오";
            }
            else
            {
                user.NAME = NAME;
                user.USER_ID = USER_ID;
                user.USER_PWD = USER_PWD;
                user.EMAIL_ADRESS = EMAIL;
                try
                {
                    _context.Registers.Add(user);
                    //_context.Registers.AddRange([user.USER_ID, user.USER_PWD, user.NAME, user.EMAIL_ADRESS]);
                    _context.SaveChanges();
                    return "Success";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "Fail";
                }
            }        
        }
    }
}