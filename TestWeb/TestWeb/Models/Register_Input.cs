using System.ComponentModel.DataAnnotations;

namespace TestWeb.Models
{
    public class Register_Input
    {
        [Key]
        public string? USER_ID { get; set; }

        public string? USER_PWD { get; set; }

        public string? EMAIL_ADRESS { get; set; }

        public string? NAME { get; set; }
    }
}
