using System.Windows;
using TaskShedulerDesktopClient.ViewModel;

namespace TaskShedulerDesktopClient.View.Windows {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            MainViewModel dc = new MainViewModel();
            this.DataContext = dc;
            dc.window = this;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.DragMove();
        }
    }
}
