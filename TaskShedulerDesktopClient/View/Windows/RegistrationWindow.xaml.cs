using System.Windows;
using TaskShedulerDesktopClient.ViewModel;

namespace TaskShedulerDesktopClient.View.Windows {
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window {
        public RegistrationWindow() {
            InitializeComponent();
            RegistrationViewModel dc = new RegistrationViewModel();
            this.DataContext = dc;
            dc.window = this;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.DragMove();
        }
    }
}