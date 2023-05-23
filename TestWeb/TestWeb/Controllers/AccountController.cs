using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TestWeb.Data;
using TestWeb.Models;
using System.Diagnostics;

namespace TestWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly LoginContext _context;

        public AccountController(LoginContext context)
        {
            _context = context;
        }

        public IActionResult Sucess(string info)
        {
            return View(info);
        }

        public IActionResult Fail(string info)
        {
            return View(info);
        }

        [HttpPost]
        public IActionResult Login(Login model)
        {	//  IsValid : 모든값 입력 받았는지 검증 ID, 비밀번호 필수로 적어야 한다.
            // [Required] 어노테이션을 기준으로 판단한다. 
            if (ModelState.IsValid)
            {
                /* using문을 사용하여 DB 오픈 커넥션 후 자동으로 닫게 해준다. */
                //using (var db = new AspnetNoteDbContext())
                //{
                //    db.Users.Add(model); // 메모리상에 올라감
                //    db.SaveChanges(); // 메모리 -> DB에 저장
                //}
                //return RedirectToAction("Index", "Home");
                //string strConn = @"Data Source=192.168.1.3,1433; Initial Catalog=USER; User ID=symation; Password=tlapdltus!@#; TrustServerCertificate=true";
                //string query = "select USER_ID, USER_PWD from dbo.LOGIN WHERE USER_ID = @id AND USER_PWD = @pwd";                


                //using (SqlConnection conn = new SqlConnection(strConn))
                //{
                //    conn.Open();
                //    var cmd = new SqlCommand(query , conn);
                //    cmd.Parameters.Add(new SqlParameter("@id", model.USER_ID));
                //    cmd.Parameters.Add(new SqlParameter("@pwd", model.USER_PWD));

                //    SqlDataReader rdr = cmd.ExecuteReader();
                //    if (rdr.HasRows)
                //    {
                //        rdr.Close();
                //        return RedirectToAction("Sucess", "Account");
                //    }
                //    else
                //    {
                //        rdr.Close();
                //        return RedirectToAction("Fail", "Account");
                //    }                    
                //}


                //var user = db.Users.FirstOrDefault(x => x.USER_ID.Equals(model.USER_ID) && x.USER_PWD.Equals(model.USER_PWD));
                //User user = _context.Users
                //    .FirstOrDefault(x => x.USER_ID.Equals(model.USER_ID) && x.USER_PWD.Equals(model.USER_PWD));
                try 
                {
                    Login? user = _context.Logins
                    .FirstOrDefault(x => x.USER_ID.Equals(model.USER_ID) && x.USER_PWD.Equals(model.USER_PWD));

                    if (user != null)
                    {
                        return RedirectToAction("Sucess", "Account");
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
            return RedirectToAction("Login", "Home");
        }

        //[HttpPost]
        //public IActionResult Register(Register model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //if (model.USER_PWD != model.USER_PWD_CHECK)
        //        //{
        //        //    return RedirectToAction("Register", "Home");
        //        //}
        //        Register user = new Register();

        //        user.USER_ID = model.USER_ID;
        //        user.USER_PWD = model.USER_PWD;
        //        user.NAME = model.NAME;
        //        user.EMAIL_ADRESS = model.EMAIL_ADRESS;

        //        try
        //        {
        //            _context.Registers.Add(user);
        //            _context.SaveChanges();
        //            return RedirectToAction("Sucess", "Account");
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex.Message);
        //            return RedirectToAction("Register", "Home");
        //        }
        //    }
        //    ViewData["ErrMsg"] = "에러입니다";
        //    return RedirectToAction("Register", "Home");
        //}
    }
}
