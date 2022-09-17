using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TaskShedulerDesktopClient.Core;
using TaskShedulerDesktopClient.Data;
using TaskShedulerDesktopClient.Data.Errors;
using TaskShedulerDesktopClient.View.Pages;
using TaskShedulerDesktopClient.View.Windows;

namespace TaskShedulerDesktopClient.ViewModel {
    class MainViewModel : ObservableObject {
        #region *** Properties and values ***
        private object _currentView;
        public object CurrentView {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }
        private int numberFalseAssignments;
        public int NumberFalseAssignments {
            get { return numberFalseAssignments; }
            set { numberFalseAssignments = value; OnPropertyChanged(); }
        }
        private int numberTrueAssignments;
        public int NumberTrueAssignments {
            get { return numberTrueAssignments; }
            set { numberTrueAssignments = value; OnPropertyChanged(); }
        }
        private int numberNullAssignments;
        public int NumberNullAssignments {
            get { return numberNullAssignments; }
            set { numberNullAssignments = value; OnPropertyChanged(); }
        }

        public ErrorInfo errorInfo { get; set; }
        public ApplicationCommands applicationCommands { get; set; } 
        public Window window { get; set; }
        public TaskShedulerContext taskShedulerContext { get; set; }

        private UserControl tasksView = new TasksView();
        private UserControl userView = new UserView();
        #endregion

        #region *** Commands ***
        private RelayCommand exitAccount;
        public RelayCommand ExitAccount {
            get {
                return exitAccount ??
                  (exitAccount = new RelayCommand(o => {
                      Window window = o as Window;
                      APIFunction._token = "";
                      taskShedulerContext.ClearData();
                      new AuthorizationWindow().Show();
                      window.Close();
                  }));
            }
        }
        private RelayCommand showOffice;
        public RelayCommand ShowOffice {
            get {
                return showOffice ??
                  (showOffice = new RelayCommand(o => {
                      CurrentView = userView;
                  }));
            }
        }
        private RelayCommand showTasksView;
        public RelayCommand ShowTasksView {
            get {
                return showTasksView ??
                  (showTasksView = new RelayCommand(o => {
                      CurrentView = tasksView;
                  }));
            }
        }
        #endregion

        public MainViewModel() {
            applicationCommands = new ApplicationCommands();
            errorInfo = new ErrorInfo();
            taskShedulerContext = new TaskShedulerContext();
            CurrentView = tasksView;
            Statistics();
            taskShedulerContext.Assignments.CollectionChanged += Assignments_CollectionChanged;
        }

        #region *** Methods ***
        private void Assignments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            Statistics();
        }
        private void Statistics() {
            NumberFalseAssignments = taskShedulerContext.Assignments.Count(e => e?.AssignmentState == false);
            NumberTrueAssignments = taskShedulerContext.Assignments.Count(e => e?.AssignmentState == true);
            NumberNullAssignments = taskShedulerContext.Assignments.Count(e => e?.AssignmentState == null);
        }
        #endregion
    }
}