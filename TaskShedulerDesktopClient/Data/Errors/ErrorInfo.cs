using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using TaskShedulerDesktopClient.Core;

namespace TaskShedulerDesktopClient.Data.Errors {
    public class ErrorInfo : ObservableObject {
        private string userName;
        public string UserName { get { return userName; } set { userName = value;  OnPropertyChanged(); } }
        private string userEmail;
        public string UserEmail { get { return userEmail; } set { userEmail = value; OnPropertyChanged(); } }
        private string userPassword;
        public string UserPassword { get { return userPassword; } set { userPassword = value; OnPropertyChanged(); } }
        private string confirmPassword;
        public string ConfirmPassword { get { return confirmPassword; } set { confirmPassword = value; OnPropertyChanged(); } }

        private string assignmentName;
        public string AssignmentName { get { return assignmentName; } set { assignmentName = value; OnPropertyChanged(); } }
        private string assignmentDescription;
        public string AssignmentDescription { get { return assignmentDescription; } set { assignmentDescription = value; OnPropertyChanged(); } }
        private string assignmentTime;
        public string AssignmentTime { get { return assignmentTime; } set { assignmentTime = value; OnPropertyChanged(); } }
        private string assignmentState;
        public string AssignmentState { get { return assignmentState; } set { assignmentState = value; OnPropertyChanged(); } }

        private string serverError;
        public string ServerError { get { return serverError; } set { serverError = value; OnPropertyChanged(); } }

        public static void SetValidationErrors(ErrorInfo errorInfo, List<ValidationResult> validationResults) {
            Type myType = typeof(ErrorInfo);
            PropertyInfo[] props = myType.GetProperties();
            ClearErrors(errorInfo, props);
            foreach (ValidationResult error in validationResults) {
                foreach (PropertyInfo prop in props) {
                    //try
                    string errorProp = Enumerable.ToArray(error.MemberNames)[0];
                    if (errorProp == prop.Name) {
                        string mes = "";
                        try {
                            mes = prop.GetValue(errorInfo).ToString();
                        }
                        catch{ }
                        mes += error.ErrorMessage;
                        mes += " ";
                        prop.SetValue(errorInfo, mes);
                    }
                }
            }
        }
        private static void ClearErrors(ErrorInfo errorInfo, PropertyInfo[] props) {
            foreach (PropertyInfo prop in props) {
                prop.SetValue(errorInfo, "");
            }
        }
        public void ClearErrors(ErrorInfo errorInfo) {
            Type myType = typeof(ErrorInfo);
            PropertyInfo[] props = myType.GetProperties();
            ClearErrors(errorInfo, props);
        }

        public static void SetServerErrors(ErrorInfo errorInfo, HttpResponseMessage responseMessage) {
            Type myType = typeof(ErrorInfo);
            PropertyInfo[] props = myType.GetProperties();
            ClearErrors(errorInfo, props);

            int code = (int)responseMessage.StatusCode;

            if (code == 400) {
                string result = responseMessage.Content.ReadAsStringAsync().Result;
                Dictionary<string, string> dictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                try {
                    errorInfo.ServerError = dictionaryResult["error"];
                }catch(Exception) {
                    errorInfo.ServerError = "Сталася помилка";
                }
            }
            else {
                errorInfo.ServerError = "Помилка серверу. Повторіть, будь ласка, спробу";
            }
        }

    }
}
