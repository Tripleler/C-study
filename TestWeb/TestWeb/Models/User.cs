using System.ComponentModel.DataAnnotations;

namespace TestWeb.Models
{
    public class User
    {
        /// <summary>
        /// 사용자번호
        /// </summary>
        [Key] // PK 설정
        public int UserNo { get; set; }

        /// <summary>
        /// 사용자 ID
        /// </summary>
        [Required(ErrorMessage = "사용자 아이디를 입력하세요.")] // Not Null 설정
        public string USER_ID { get; set; }

        /// <summary>
        /// 사용자 비밀번호
        /// </summary>
        [Required(ErrorMessage = "사용자 비밀번호를 입력하세요.")] // Not Null 설정
        public string USER_PWD { get; set; }

    }
}
