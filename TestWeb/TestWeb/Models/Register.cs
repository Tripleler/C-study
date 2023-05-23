using System.ComponentModel.DataAnnotations;

namespace TestWeb.Models
{
    public class Register
    {
        [Key]
        [Required(ErrorMessage ="아이디를 입력하세요")]
        public string? USER_ID { get; set; }

        [Required(ErrorMessage ="비밀번호를 입력하세요")]
        public string? USER_PWD { get; set; }

        [Required(ErrorMessage = "비밀번호 확인란을 입력하세요")]
        [Compare("USER_PWD", ErrorMessage ="입력하신 비밀번호와 비밀번호 확인란이 서로 다릅니다.")]
        public string? USER_PWD_CHECK { get; set; }

        [Required(ErrorMessage ="이메일 주소를 입력하세요")]
        [RegularExpression(@"^[\d\w._-]+@[\d\w._-]+\.[\w]+$", ErrorMessage = "이메일 형식을 사용하세요")]
        public string? EMAIL_ADRESS { get; set; }

        [Required(ErrorMessage ="이름을 입력하세요")]
        public string? NAME { get; set; }
    }
}
