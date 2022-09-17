using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using TaskShedulerDesktopClient.Data.Errors;
using TaskShedulerDesktopClient.Models;

namespace TaskShedulerDesktopClient.Data {
    public class APIFunction {
        public static string _token { get; set; }
        public static string _API_ConnectionString { get { return "https://taskschedulerapi20220608191912.azurewebsites.net"; } }

        public static async Task<string> GetAPIKeyAsync() {
            using (var client = new HttpClient()) {
                var response = await client.GetAsync(_API_ConnectionString + "/api/User/GetPublicKey");
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
                return await response.Content.ReadAsStringAsync();
            }
        }
        private static async Task<string> GetTokenAsync(LoginModel model) {
            using (var client = new HttpClient()) {
                HttpResponseMessage response = await client.PostAsJsonAsync(_API_ConnectionString + "/api/Auth/Auth", model);
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
                string result = await response.Content.ReadAsStringAsync();
                Dictionary<string, string> dictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                return dictionaryResult["token"];
            }
        }
        public static async Task GetSaveTokenAsync(LoginModel model) {
            _token = await GetTokenAsync(model);
        }

        #region *** CRUD User ***
        public static async Task<User> GetUserAsync() {
            using (var client = CreateClientWithToken(_token)) {
                client.DefaultRequestHeaders.Add("userPublicKey", СryptographyData._publicKey);
                var response = await client.GetAsync(_API_ConnectionString + "/api/User/GetUser");
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
                var result = await response.Content.ReadAsAsync<User>();
                return result;
            }
        }
        public static async Task CreateUserAsync(RegisterModel model) {
            using (var client = new HttpClient()) {
                var response = await client.PostAsJsonAsync(_API_ConnectionString + "/api/User/CreateUser", model);
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
            }
        }
        public static async Task UpdateUserAsync(User user) {
            using (var client = CreateClientWithToken(_token)) {
                var response = await client.PutAsJsonAsync(_API_ConnectionString + "/api/User/UpdateUser", user);
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
            }
        }
        public static async Task DeleteUserAsync() {
            using (var client = CreateClientWithToken(_token)) {
                var response = await client.DeleteAsync(_API_ConnectionString + "/api/User/DeleteUser");
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
            }
        }
        #endregion

        #region *** CRUD Assignment ***
        public static async Task<ObservableCollection<Assignment>> GetAssignmentsAsync() {
            using (var client = CreateClientWithToken(_token)) {
                var response = await client.GetAsync(_API_ConnectionString + "/api/Assignment/GetAllAssignments");
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
                var result = await response.Content.ReadAsAsync<ObservableCollection<Assignment>>();
                return result;
            }
        }
        public static async Task<int> CreateAssignmentAsync(Assignment assignment) {
            using (var client = CreateClientWithToken(_token)) {
                var response = await client.PostAsJsonAsync(_API_ConnectionString + "/api/Assignment/CreateAssignment", assignment);
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
                var result = await response.Content.ReadAsAsync<int>();
                return result;
            }
        }
        public static async Task UpdateAssignmentAsync(Assignment assignment) {
            using (var client = CreateClientWithToken(_token)) {
                var response = await client.PutAsJsonAsync(_API_ConnectionString + "/api/Assignment/UpdateAssignment", assignment);
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
            }
        }
        public static async Task DeleteAssignmentAsync(int assignmentId) {
            using (var client = CreateClientWithToken(_token)) {
                var response = await client.DeleteAsync(_API_ConnectionString + $"/api/Assignment/DeleteAssignment/{assignmentId}");
                if (!response.IsSuccessStatusCode) throw new ServerException(response);
            }
        }
        #endregion

        private static HttpClient CreateClientWithToken(string token) {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.
                    AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
