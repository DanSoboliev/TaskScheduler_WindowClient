using System.Windows;
using TaskShedulerDesktopClient.Core;

namespace TaskShedulerDesktopClient.ViewModel {
    class ApplicationCommands {
        private RelayCommand closeWinwow;
        public RelayCommand CloseWinwow {
            get {
                return closeWinwow ??
                  (closeWinwow = new RelayCommand(o => {
                      Window window = o as Window;
                      window.Close();
                  }));
            }
        }

        private RelayCommand minimizedWinwow;
        public RelayCommand MinimizedWinwow {
            get {
                return minimizedWinwow ??
                  (minimizedWinwow = new RelayCommand(o => {
                      Window window = o as Window;
                      window.WindowState = WindowState.Minimized;
                  }));
            }
        }

        private RelayCommand goToWebsite;
        public RelayCommand GoToWebsite
        {
            get
            {
                return goToWebsite ??
                  (goToWebsite = new RelayCommand(o => {
                      var uri = "https://gigabruhtaskscheduler.azurewebsites.net";
                      var psi = new System.Diagnostics.ProcessStartInfo();
                      psi.UseShellExecute = true;
                      psi.FileName = uri;
                      System.Diagnostics.Process.Start(psi);
                  }));
            }
        }
        private RelayCommand goToTermsOfUse;
        public RelayCommand GoToTermsOfUse
        {
            get
            {
                return goToTermsOfUse ??
                  (goToTermsOfUse = new RelayCommand(o => {
                      var uri = "https://gigabruhtaskscheduler.azurewebsites.net/Home/Privacy";
                      var psi = new System.Diagnostics.ProcessStartInfo();
                      psi.UseShellExecute = true;
                      psi.FileName = uri;
                      System.Diagnostics.Process.Start(psi);
                  }));
            }
        }
    }
}