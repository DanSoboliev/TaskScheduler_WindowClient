using System.ComponentModel.DataAnnotations;

namespace TaskShedulerDesktopClient.Models {
    public class LoginModel : IUser {
        [Required(ErrorMessage = "Відсутній логін")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Некоректна довжина логіну")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Відсутній пароль")]
        [StringLength(50, ErrorMessage = "Некоректна довжина паролю")]
        public string UserPassword { get; set; }

        public LoginModel() { }
    }
}
