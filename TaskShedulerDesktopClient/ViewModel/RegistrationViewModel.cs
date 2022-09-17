using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using TaskShedulerDesktopClient.Core;
using TaskShedulerDesktopClient.Data;
using TaskShedulerDesktopClient.Data.Errors;
using TaskShedulerDesktopClient.Models;
using TaskShedulerDesktopClient.View.Windows;

namespace TaskShedulerDesktopClient.ViewModel {
    class RegistrationViewModel : ObservableObject {
        #region *** Properties and values ***
        private bool buttonEnable = true;
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool AgreeTermsOfUse { get; set; } = false;
        private bool showLoading;
        public bool ShowLoading { get { return showLoading; } set { showLoading = value; OnPropertyChanged(); } }
        public Window window { get; set; }
        public ApplicationCommands applicationCommands { get; set; } = new ApplicationCommands();
        public ErrorInfo errorInfo { get; set; } = new ErrorInfo();
        #endregion

        #region *** Commands ***
        private RelayCommand returnToAuthorizationView;
        public RelayCommand ReturnToAuthorizationView {
            get {
                return returnToAuthorizationView ??
                  (returnToAuthorizationView = new RelayCommand(o => {
                      Window window = o as Window;
                      AuthorizationWindow authorizationWindow = new AuthorizationWindow();
                      authorizationWindow.Show();
                      window.Close();
                  }));
            }
        }

        private RelayCommand registration;
        public RelayCommand Registration {
            get {
                return registration ??
                  (registration = new RelayCommand(async o => {
                      ShowLoading = true;
                      buttonEnable = false;
                      RegisterModel registerModel = new RegisterModel() { UserName = UserName, UserEmail = Email, UserPassword = Password };

                      var context = new ValidationContext(registerModel);
                      var results = new List<ValidationResult>();
                      if (!Validator.TryValidateObject(registerModel, context, results, true)) {
                          ErrorInfo.SetValidationErrors(errorInfo, results);
                          if (Password != ConfirmPassword) errorInfo.ConfirmPassword = "Паролі не співпадають";
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }
                      if (Password != ConfirmPassword) {
                          errorInfo.ConfirmPassword = "Паролі не співпадають";
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }
                      if (AgreeTermsOfUse != true) {
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      try {
                          registerModel.RSA_Encrypt_IUser();
                      }
                      catch (Exception) {
                          errorInfo.ServerError = "Помилка передачі даних. Повторіть, будь ласка, спробу";
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      try {
                          await APIFunction.CreateUserAsync(registerModel);
                      }
                      catch(ServerException ex) {
                          ErrorInfo.SetServerErrors(errorInfo, ex.ResponseMessage);
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      LoginModel loginModel = new LoginModel() { UserName = registerModel.UserName, UserPassword = registerModel.UserPassword };

                      User user;
                      ObservableCollection<Assignment> assignments;

                      try {
                          await APIFunction.GetSaveTokenAsync(loginModel);
                          user = await APIFunction.GetUserAsync();
                          assignments = await APIFunction.GetAssignmentsAsync();
                      }
                      catch (ServerException) {
                          APIFunction._token = "";
                          MessageBox.Show("Користувач успішно зареєстрований");
                          new AuthorizationWindow().Show();
                          window.Close();
                          return;
                      }

                      TaskShedulerContext taskShedulerContext = new TaskShedulerContext(user, assignments);
                      new MainWindow().Show();
                      window.Close();
                  }, o => buttonEnable));
            }
        }
        #endregion

        public RegistrationViewModel() { }
    }
}