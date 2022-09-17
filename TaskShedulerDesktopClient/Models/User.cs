using System;
using System.ComponentModel.DataAnnotations;
using TaskShedulerDesktopClient.Core;

namespace TaskShedulerDesktopClient.Models {
    public class User : ObservableObject, IUser {
        private string userName;
        private string userEmail;
        private string userPassword;

        [Required(ErrorMessage = "Відсутній id")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Недопустиме значення id")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Відсутній логін")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Некоректна довжина логіну")]
        [RegularExpression(@"\A[А-ЯA-ZЇІЄҐЬа-яa-zїієєґь._0-9 ]{3,50}\z", ErrorMessage = "Недопустимі символи")]
        public string UserName { get { return userName; } set { userName = value; OnPropertyChanged(); } }
        
        [Required(ErrorMessage = "Відсутній адрес електронної пошти")]
        [EmailAddress(ErrorMessage = "Не відповідає електронній пошті")]
        [StringLength(50, ErrorMessage = "Некоректна довжина електронної пошти")]
        public string UserEmail { get { return userEmail; } set { userEmail = value; OnPropertyChanged(); } }

        [Required(ErrorMessage = "Відсутній пароль")]
        [StringLength(50, ErrorMessage = "Некоректна довжина паролю")]
        [RegularExpression(@"^(?=.{10,}$)(?=(?:.*?[A-Z]){2})(?=.*?[a-z])(?=(?:.*?[0-9]){2}).*$", ErrorMessage = "Недопустимі символи")]
        public string UserPassword { get { return userPassword; } set { userPassword = value; OnPropertyChanged(); } }

        public User() { }
        public User(User user) {
            UserId = user.UserId;
            UserName = user.UserName;
            UserEmail = user.UserEmail;
            UserPassword = user.UserPassword;
        }
    }
}
