using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows;
using TaskShedulerDesktopClient.Core;
using TaskShedulerDesktopClient.Data;
using TaskShedulerDesktopClient.Data.Errors;
using TaskShedulerDesktopClient.Models;
using TaskShedulerDesktopClient.View.Windows;

namespace TaskShedulerDesktopClient.ViewModel {
    class AuthorizationViewModel : ObservableObject {
        #region *** Properties and values ***
        private bool buttonEnable = true;
        public Window window { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public ErrorInfo errorInfo { get; set; } = new ErrorInfo();
        public ApplicationCommands applicationCommands { get; set; } = new ApplicationCommands();
        private bool showLoading;
        public bool ShowLoading { get { return showLoading; } set { showLoading = value; OnPropertyChanged(); } }
        #endregion

        #region *** Commands ***
        private RelayCommand loginCommand;
        public RelayCommand LoginCommand {
            get {
                return loginCommand ??
                  (loginCommand = new RelayCommand(async o => {
                      ShowLoading = true;
                      buttonEnable = false;
                      LoginModel loginModel = new LoginModel() { UserName = Login, UserPassword = Password };

                      var context = new ValidationContext(loginModel);
                      var results = new List<ValidationResult>();
                      if (!Validator.TryValidateObject(loginModel, context, results, true)) {
                          ErrorInfo.SetValidationErrors(errorInfo, results);
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      try {
                          loginModel.RSA_Encrypt_IUser();
                      }
                      catch (Exception) {
                          errorInfo.ServerError = "Помилка передачі даних. Повторіть, будь ласка, спробу";
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      User user;
                      ObservableCollection<Assignment> assignments;

                      try {
                          await APIFunction.GetSaveTokenAsync(loginModel);
                          user = await APIFunction.GetUserAsync();
                          assignments = await APIFunction.GetAssignmentsAsync();  
                      }
                      catch (ServerException ex) {
                          ErrorInfo.SetServerErrors(errorInfo, ex.ResponseMessage);
                          APIFunction._token = "";
                          buttonEnable = true;
                          ShowLoading = false;
                          return;
                      }

                      try {
                          user.RSA_Decrypt_IUser();
                      }
                      catch (Exception) { }

                      bool? rollback;
                      Task task = Task.Run(async () => {
                          foreach (Assignment assignment in assignments) {
                              if (assignment.AssignmentState == true) continue;
                              rollback = assignment.AssignmentState;
                              if (assignment.AssignmentTime > DateTime.Now) assignment.AssignmentState = null;
                              else assignment.AssignmentState = false;
                              try {
                                  await APIFunction.UpdateAssignmentAsync(assignment);
                              }
                              catch (ServerException) {
                                  assignment.AssignmentState = rollback;
                              }
                          }
                      });
                      await task;

                      TaskShedulerContext taskShedulerContext = new TaskShedulerContext(user, assignments);

                      new MainWindow().Show();
                      window.Close();
                  }, o => buttonEnable));
            }
        }

        private RelayCommand registrationViewCommand;
        public RelayCommand RegistrationViewCommand {
            get {
                return registrationViewCommand ??
                  (registrationViewCommand = new RelayCommand(o => {
                      Window window = o as Window;
                      new RegistrationWindow().Show();
                      window.Close();
                  }));
            }
        }
        #endregion

        public AuthorizationViewModel() {
            СryptographyData.RSA_KeysGenerate_Save();
            SaveAPIKey();
        }

        #region *** Methods ***
        private async void SaveAPIKey() {
            while (true) {
                try { 
                    СryptographyData._API_PublicKey = await APIFunction.GetAPIKeyAsync();
                    break;
                }
                catch (ServerException) {
                    continue;
                }
                catch (Exception) {
                    continue;
                }
            }
        }
        #endregion
    }
}