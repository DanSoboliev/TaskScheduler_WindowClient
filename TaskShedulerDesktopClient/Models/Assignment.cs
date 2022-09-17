using System;
using System.ComponentModel.DataAnnotations;
using TaskShedulerDesktopClient.Core;

namespace TaskShedulerDesktopClient.Models {
    public class Assignment : ObservableObject {
        private string assignmentName;
        private string assignmentDescription;
        private DateTime assignmentTime;
        private bool? assignmentState;

        [Required(ErrorMessage = "Відсутній id")]
        [Range(0, int.MaxValue, ErrorMessage = "Недопустиме значення id")]
        public int AssignmentId { get; set; }

        [Required(ErrorMessage = "Відсутня назва")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Некоректна довжина назви")]
        [RegularExpression(@"\A[А-ЯA-ZЇІЄҐЬа-яa-zїієєґь._0-9 ]{1,50}\z", ErrorMessage = "Недопустимі символи")]
        public string AssignmentName { get { return assignmentName; } set { assignmentName = value; OnPropertyChanged(); } }

        [StringLength(250, ErrorMessage = "Некоректна довжина опису")]
        public string AssignmentDescription { get { return assignmentDescription; } set { assignmentDescription = value; OnPropertyChanged(); } }

        [Required(ErrorMessage = "Відсутній час")]
        public DateTime AssignmentTime { get { return assignmentTime; } set { assignmentTime = value; OnPropertyChanged(); } }

        public bool? AssignmentState { get { return assignmentState; } set { assignmentState = value; OnPropertyChanged(); } }

        [Required(ErrorMessage = "Відсутній користувач")]
        [Range(0, int.MaxValue, ErrorMessage = "Недопустиме значення id")]
        public int UserId { get; set; }

        public Assignment() { }
        public Assignment(string AssignmentName, string AssignmentDescription, DateTime AssignmentTime) {
            AssignmentId = 0;
            this.AssignmentName = AssignmentName;
            this.AssignmentDescription = AssignmentDescription;
            this.AssignmentTime = AssignmentTime;
            if (AssignmentTime > DateTime.Now) AssignmentState = null;
            else AssignmentState = false;
            UserId = 0;
        }
    }
}
