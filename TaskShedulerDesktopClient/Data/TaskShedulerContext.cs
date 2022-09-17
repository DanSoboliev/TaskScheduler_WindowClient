using System.Collections.ObjectModel;
using TaskShedulerDesktopClient.Core;
using TaskShedulerDesktopClient.Models;

namespace TaskShedulerDesktopClient.Data {
    public class TaskShedulerContext : ObservableObject {
        private static ObservableCollection<Assignment> _Assignments;
        public ObservableCollection<Assignment> Assignments {
            get { return _Assignments; }
            set {
                _Assignments = value;
                OnPropertyChanged();
            }
        }
        private static User _User;
        public User User {
            get { return _User; }
            private set {
                _User = value;
                OnPropertyChanged();
            }
        }
        public TaskShedulerContext() { }
        public TaskShedulerContext(User user, ObservableCollection<Assignment> assignments) {
            _User = user;
            _Assignments = assignments;
        }
        public void ClearData() {
            _Assignments = null;
            _User = null;
        }
    }
}