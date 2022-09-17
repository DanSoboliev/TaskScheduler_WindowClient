using System.Windows.Controls;
using TaskShedulerDesktopClient.ViewModel;

namespace TaskShedulerDesktopClient.View.Pages {
    /// <summary>
    /// Логика взаимодействия для TasksView.xaml
    /// </summary>
    public partial class TasksView : UserControl {
        public TasksView() {
            InitializeComponent();
            TaskViewModel dc = new TaskViewModel();
            this.DataContext = dc;
        }
    }
}