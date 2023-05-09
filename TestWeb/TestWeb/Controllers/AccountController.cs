using Microsoft.AspNetCore.Mvc;
using TestWeb.Models;

namespace TestWeb.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User model)
        {	//  IsValid : 모든값 입력 받았는지 검증 ID, 비밀번호 필수로 적어야 한다.
            // [Required] 어노테이션을 기준으로 판단한다. 
            if (ModelState.IsValid)
            {
                /* using문을 사용하여 DB 오픈 커넥션 후 자동으로 닫게 해준다. */
                using (var db = new AspnetNoteDbContext())
                {
                    db.Users.Add(model); // 메모리상에 올라감
                    db.SaveChanges(); // 메모리 -> DB에 저장
                }
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
