using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskShedulerDesktopClient.Core;
using TaskShedulerDesktopClient.Data;
using TaskShedulerDesktopClient.Data.Errors;
using TaskShedulerDesktopClient.Models;
using TaskShedulerDesktopClient.View.Windows;

namespace TaskShedulerDesktopClient.ViewModel {
    class UserViewModel : ObservableObject {
        #region *** Properties and values ***
        public Window window;
        private Visibility alertVisible;
        public Visibility AlertVisible { get { return alertVisible; } set { alertVisible = value; OnPropertyChanged(); } }
        private bool showLoading;
        public bool ShowLoading { get { return showLoading; } set { showLoading = value; OnPropertyChanged(); } }
        private bool buttonEnable;
        public bool ButtonEnable { get { return buttonEnable; } set { buttonEnable = value; OnPropertyChanged(); } }
        public ErrorInfo errorInfo { get; set; } = new ErrorInfo();
        private string errorOldPassword;
        public string ErrorOldPassword { get { return errorOldPassword; } set { errorOldPassword = value; OnPropertyChanged(); } }
        private string errorNewPassword;
        public string ErrorNewPassword { get { return errorNewPassword; } set { errorNewPassword = value; OnPropertyChanged(); } }
        private string errorConfirmNewPassword;
        public string ErrorConfirmNewPassword { get { return errorConfirmNewPassword; } set { errorConfirmNewPassword = value; OnPropertyChanged(); } }
        private string errorTooltip;
        public string ErrorTooltip { get { return errorTooltip; } set { errorTooltip = value; OnPropertyChanged(); } }
        private int numberFalseAssignments;

        public int NumberFalseAssignments { get { return numberFalseAssignments; } set { numberFalseAssignments = value; OnPropertyChanged(); } }
        private int numberTrueAssignments;
        public int NumberTrueAssignments { get { return numberTrueAssignments; } set { numberTrueAssignments = value; OnPropertyChanged(); } }
        private int numberNullAssignments;
        public int NumberNullAssignments { get { return numberNullAssignments; } set { numberNullAssignments = value; OnPropertyChanged(); } }
        public TaskShedulerContext taskShedulerContext { get; private set; }

        private string userName;
        public string UserName { get { return userName; } set { userName = value; OnPropertyChanged(); } }
        private string userEmail;
        public string UserEmail { get { return userEmail; } set { userEmail = value; OnPropertyChanged(); } }
        private string oldPassword;
        public string OldPassword { get { return oldPassword; } set { oldPassword = value; OnPropertyChanged(); } }
        private string newPassword;
        public string NewPassword { get { return newPassword; } set { newPassword = value; OnPropertyChanged(); } }
        private string confirmNewPassword;
        public string ConfirmNewPassword { get { return confirmNewPassword; } set { confirmNewPassword = value; OnPropertyChanged(); } }

        public int assignmentsCount;
        public int AssignmentsCount { get { return assignmentsCount; } set { assignmentsCount = value; OnPropertyChanged(); } }
        public int percentFalseAssignments = 0;
        public int PercentFalseAssignments { get { return percentFalseAssignments; } set { percentFalseAssignments = value; OnPropertyChanged(); } }
        public int percentTrueAssignments = 0;
        public int PercentTrueAssignments { get { return percentTrueAssignments; } set { percentTrueAssignments = value; OnPropertyChanged(); } }
        public int percentNullAssignments = 0;
        public int PercentNullAssignments { get { return percentNullAssignments; } set { percentNullAssignments = value; OnPropertyChanged(); } }
        #endregion

        #region *** Commands ***
        private RelayCommand deleteUser;
        public RelayCommand DeleteUser {
            get {
                return deleteUser ??
                  (deleteUser = new RelayCommand(async o => {
                      ShowLoading = true;
                      buttonEnable = false;
                      ClearErrorMessages();

                      if (MessageBox.Show($"Видалити користувача {taskShedulerContext.User.UserName}?", "Підтвердження видалення", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) {
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      try {
                          await APIFunction.DeleteUserAsync();
                      }
                      catch (ServerException ex) {
                          ErrorInfo.SetServerErrors(errorInfo, ex.ResponseMessage);
                          AlertVisible = Visibility.Visible;
                      }

                      taskShedulerContext.ClearData();
                      new AuthorizationWindow().Show();
                      window.Close();
                      buttonEnable = true;
                      ShowLoading = false;
                  }, o => buttonEnable));
            }
        }
        private RelayCommand updateLogin;
        public RelayCommand UpdateLogin {
            get {
                return updateLogin ??
                  (updateLogin = new RelayCommand(async o => {
                      ShowLoading = true;
                      buttonEnable = false;
                      ClearErrorMessages();

                      User user = new User(taskShedulerContext.User);
                      user.UserName = UserName;

                      bool continuation = await Implementation(user);
                      if (!continuation) return;

                      taskShedulerContext.User.UserName = UserName;

                      buttonEnable = true;
                      ShowLoading = false;
                  }, o => buttonEnable));
            }
        }
        private RelayCommand updateEmail;
        public RelayCommand UpdateEmail {
            get {
                return updateEmail ??
                  (updateEmail = new RelayCommand(async o => {
                      ShowLoading = true;
                      buttonEnable = false;
                      ClearErrorMessages();

                      User user = new User(taskShedulerContext.User);
                      user.UserEmail = UserEmail;

                      bool continuation = await Implementation(user);
                      if (!continuation) return;

                      taskShedulerContext.User.UserEmail = UserEmail;

                      buttonEnable = true;
                      ShowLoading = false;
                  }, o => buttonEnable));
            }
        }
        private RelayCommand updatePassword;
        public RelayCommand UpdatePassword {
            get {
                return updatePassword ??
                  (updatePassword = new RelayCommand(async o => {
                      ShowLoading = true;
                      buttonEnable = false;
                      ClearErrorMessages();

                      User user = new User(taskShedulerContext.User);
                      if (user.UserPassword != OldPassword) {
                          ErrorOldPassword = "Не правильний старий пароль";
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      user.UserPassword = NewPassword;

                      if (NewPassword != ConfirmNewPassword) {
                          ErrorConfirmNewPassword = "Паролі не співпадають";
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      bool continuation = await Implementation(user);
                      if (!continuation) return;

                      taskShedulerContext.User.UserPassword = NewPassword;

                      buttonEnable = true;
                      ShowLoading = false;
                  }, o => buttonEnable));
            }
        }
        #endregion

        public UserViewModel() {
            this.window = Application.Current.Windows.OfType<Window>().Where(w => w.Title == "MainWindow").FirstOrDefault();
            taskShedulerContext = new TaskShedulerContext();
            taskShedulerContext.Assignments.CollectionChanged += Assignments_CollectionChanged;
            Statistics();
            AlertVisible = Visibility.Hidden;
            ShowLoading = false;
            ButtonEnable = true;
        }

        #region *** Methods ***
        private void Assignments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            Statistics();
        }
        private void Statistics() {
            NumberFalseAssignments = taskShedulerContext.Assignments.Count(e => e?.AssignmentState == false);
            NumberTrueAssignments = taskShedulerContext.Assignments.Count(e => e?.AssignmentState == true);
            NumberNullAssignments = taskShedulerContext.Assignments.Count(e => e?.AssignmentState == null);

            AssignmentsCount = taskShedulerContext.Assignments.Count;
            if (AssignmentsCount == 0) return;

            CalculationPercents();
        }
        private void CalculationPercents() {
            int[] percents = new int[3];
            percents[0] = (int)((double)NumberFalseAssignments / AssignmentsCount * 100);
            percents[1] = (int)((double)NumberTrueAssignments / AssignmentsCount * 100);
            percents[2] = (int)((double)NumberNullAssignments / AssignmentsCount * 100);
            int sum = percents[0] + percents[1] + percents[2];
            int i = 0;
            while (sum < 100 && sum != 100) {
                if (i > 2) i = 0;
                if (percents[i] > 0) percents[i] = percents[i] + 1;
                sum = percents[0] + percents[1] + percents[2];
            }
            PercentFalseAssignments = percents[0];
            PercentTrueAssignments = percents[1];
            PercentNullAssignments = percents[2];
        }

        private async Task<bool> Implementation(User user) {
            var context = new ValidationContext(user);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(user, context, results, true)) {
                ErrorInfo.SetValidationErrors(errorInfo, results);
                buttonEnable = true;
                ShowLoading = false;
                return false;
            }

            try {
                user.RSA_Encrypt_IUser();
            }
            catch (Exception) {
                errorInfo.ServerError = "Помилка передачі даних. Повторіть, будь ласка, спробу";
                AlertVisible = Visibility.Visible;
                buttonEnable = true;
                ShowLoading = false;
                return false;
            }

            try {
                await APIFunction.UpdateUserAsync(user);
            }
            catch (ServerException ex) {
                ErrorInfo.SetServerErrors(errorInfo, ex.ResponseMessage);
                AlertVisible = Visibility.Visible;
                buttonEnable = true;
                ShowLoading = false;
                return false;
            }
            return true;
        }

        private void ClearErrorMessages() {
            ErrorOldPassword = "";
            ErrorNewPassword = "";
            ErrorConfirmNewPassword = "";
            AlertVisible = Visibility.Hidden;
            errorInfo.ClearErrors(errorInfo);
        }
        #endregion
    }
}