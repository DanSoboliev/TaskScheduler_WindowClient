using System.ComponentModel.DataAnnotations;

namespace TaskShedulerDesktopClient.Models {
    public class RegisterModel : IUser {
        [Required(ErrorMessage = "Відсутній логін")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Некоректна довжина логіну")]
        [RegularExpression(@"\A[А-ЯA-ZЇІЄҐЬа-яa-zїієєґь._0-9 ]{3,50}\z", ErrorMessage = "Недопустимі символи")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Відсутній адрес електронної пошти")]
        [EmailAddress]
        [StringLength(50, ErrorMessage = "Некоректна довжина електронної пошти")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Відсутній пароль")]
        [StringLength(50, ErrorMessage = "Некоректна довжина паролю")]
        [RegularExpression(@"^(?=.{10,}$)(?=(?:.*?[A-Z]){2})(?=.*?[a-z])(?=(?:.*?[0-9]){2}).*$", ErrorMessage = "Недопустимі символи")]
        public string UserPassword { get; set; }

        public string ConfirmPassword { get; set; }

        public byte[] UserImage { get; set; }

        public RegisterModel() { }
    }
}
