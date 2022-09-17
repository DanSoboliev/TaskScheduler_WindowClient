using System.Windows.Controls;
using TaskShedulerDesktopClient.ViewModel;

namespace TaskShedulerDesktopClient.View.Pages {
    /// <summary>
    /// Логика взаимодействия для UserView.xaml
    /// </summary>
    public partial class UserView : UserControl {
        public UserView() {
            InitializeComponent();
            UserViewModel dc = new UserViewModel();
            this.DataContext = dc;
        }
    }
}