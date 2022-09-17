using System.Windows;
using System.Windows.Input;
using TaskShedulerDesktopClient.ViewModel;

namespace TaskShedulerDesktopClient.View.Windows {
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window {
        public AuthorizationWindow() {
            InitializeComponent();
            AuthorizationViewModel dc = new AuthorizationViewModel();
            this.DataContext = dc;
            //if (dc.CloseAction == null) dc.CloseAction = new Action(this.Close);
            dc.window = this;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }
    }
}