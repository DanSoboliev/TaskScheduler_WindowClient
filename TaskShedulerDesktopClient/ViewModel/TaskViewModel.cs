using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using TaskShedulerDesktopClient.Core;
using TaskShedulerDesktopClient.Data;
using TaskShedulerDesktopClient.Data.Errors;
using TaskShedulerDesktopClient.Models;
using TaskShedulerDesktopClient.View.Pages;

namespace TaskShedulerDesktopClient.ViewModel {
    class TaskViewModel : ObservableObject {
        #region *** Properties and values ***
        private List<int> deleteList = new List<int>();
        public ErrorInfo errorInfo { get; set; } = new ErrorInfo();

        private string assignmentName;
        public string AssignmentName { get { return assignmentName; } set { assignmentName = value; OnPropertyChanged(); } }
        private string assignmentDescription;
        public string AssignmentDescription { get { return assignmentDescription; } set { assignmentDescription = value; OnPropertyChanged(); } }
        private DateTime assignmentDate;
        public DateTime AssignmentDate { get { return assignmentDate; } set { assignmentDate = value; OnPropertyChanged(); } }
        private DateTime assignmentTime;
        public DateTime AssignmentTime { get { return assignmentTime; } set { assignmentTime = value; OnPropertyChanged(); } }
        private bool? assignmentState;
        public bool? AssignmentState { get { return assignmentState; } set { assignmentState = value; OnPropertyChanged(); } }

        private Assignment selectedAssignment;
        public Assignment SelectedAssignment { get { return selectedAssignment; } set { selectedAssignment = value; OnPropertyChanged(); if (value != null) SelectedAssignment_PropertyChanged(); else NewCreateTaskView(); } }
        private object _currentView;
        public object CurrentView { get { return _currentView; } set { _currentView = value; OnPropertyChanged(); } }
        private IEnumerable<Assignment> collectionAssignments;
        public IEnumerable<Assignment> CollectionAssignments { get { return collectionAssignments; } set { collectionAssignments = value; OnPropertyChanged(); } }
        public TaskShedulerContext taskShedulerContext { get; private set; }
        public AssignmentsFiltrSort assignmentsFiltrSort { get; private set; }

        private string searchName;
        public string SearchName { get { return searchName; } set { searchName = value; OnPropertyChanged(); assignmentsFiltrSort.searchName = searchName; CollectionAssignments = assignmentsFiltrSort.ApplyFilters(taskShedulerContext.Assignments); } }
        private DateTime dateFrom;
        public DateTime DateFrom { get { return dateFrom; } set { dateFrom = value; OnPropertyChanged(); assignmentsFiltrSort.dateFrom = dateFrom; } }
        private DateTime dateTo;
        public DateTime DateTo { get { return dateTo; } set { dateTo = value; OnPropertyChanged(); assignmentsFiltrSort.dateTo = dateTo; } }
        private bool sortDirection;
        public bool SortDirection { get { return sortDirection; } set { sortDirection = value; OnPropertyChanged(); assignmentsFiltrSort.sortDirection = sortDirection; CollectionAssignments = assignmentsFiltrSort.ApplyFilters(taskShedulerContext.Assignments); } }
        private bool showLoading;
        public bool ShowLoading { get { return showLoading; } set { showLoading = value; OnPropertyChanged(); } }
        private bool buttonEnable = true;
        #endregion

        #region *** Commands ***
        private RelayCommand addAssignment;
        public RelayCommand AddAssignment {
            get {
                return addAssignment ??
                  (addAssignment = new RelayCommand(o => {
                      SelectedAssignment = null;
                      errorInfo.ClearErrors(errorInfo);
                      NewCreateTaskView();
                  }));
            }
        }
        private RelayCommand deleteAssignments;
        public RelayCommand DeleteAssignments {
            get {
                return deleteAssignments ??
                  (deleteAssignments = new RelayCommand(async o => {
                      ShowLoading = true;
                      foreach (int id in deleteList) {
                          try {
                              await APIFunction.DeleteAssignmentAsync(id);
                          }
                          catch (ServerException) {
                              continue;
                          }
                          try {
                              taskShedulerContext.Assignments.Remove((from a in taskShedulerContext.Assignments where a.AssignmentId == id select a).First());
                          }
                          catch (Exception) {
                              continue;
                          }
                      }
                      deleteList.Clear();
                      ShowLoading = false;
                  }));
            }
        }
        private RelayCommand checkForDelete;
        public RelayCommand CheckForDelete {
            get {
                return checkForDelete ??
                  (checkForDelete = new RelayCommand(o => {
                      int assignmentId = Convert.ToInt32(o);
                      if (deleteList.Contains(assignmentId)) deleteList.Remove(assignmentId);
                      else deleteList.Add(assignmentId);
                  }));
            }
        }
        private RelayCommand checkStateInList;
        public RelayCommand CheckStateInList {
            get {
                return checkStateInList ??
                  (checkStateInList = new RelayCommand(async o => {
                      int assignmentId = Convert.ToInt32(o);
                      int index = taskShedulerContext.Assignments.IndexOf((from a in taskShedulerContext.Assignments where a.AssignmentId == assignmentId select a).First());
                      Assignment assignment = taskShedulerContext.Assignments[index];
                      bool? rollback = taskShedulerContext.Assignments[index].AssignmentState;
                      if (taskShedulerContext.Assignments[index].AssignmentState != true) {
                          if (assignment.AssignmentTime > DateTime.Now) assignment.AssignmentState = null;
                          else assignment.AssignmentState = false;
                      }
                      taskShedulerContext.Assignments[index] = assignment;
                      try {
                          await APIFunction.UpdateAssignmentAsync(taskShedulerContext.Assignments[index]);
                      }
                      catch (ServerException) {
                          assignment.AssignmentState = rollback;
                          taskShedulerContext.Assignments[index] = assignment;
                      }
                      if (selectedAssignment?.AssignmentId == assignment.AssignmentId) AssignmentState = assignment.AssignmentState;
                  }));
            }
        }

        private RelayCommand createAssignment;
        public RelayCommand CreateAssignment {
            get {
                return createAssignment ??
                  (createAssignment = new RelayCommand(async o => {
                      ShowLoading = true;
                      Assignment assignment = new Assignment(AssignmentName, AssignmentDescription, new DateTime(AssignmentDate.Year, AssignmentDate.Month, AssignmentDate.Day, AssignmentTime.Hour, AssignmentTime.Minute, AssignmentTime.Second));
                      var context = new ValidationContext(assignment);
                      var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                      if (!Validator.TryValidateObject(assignment, context, results, true)) {
                          ErrorInfo.SetValidationErrors(errorInfo, results);
                          ShowLoading = false;
                          return;
                      }
                      int assignmentId = 0;
                      try {
                          assignmentId = await APIFunction.CreateAssignmentAsync(assignment);
                      }
                      catch (ServerException ex) {
                          ErrorInfo.SetServerErrors(errorInfo, ex.ResponseMessage);
                          ShowLoading = false;
                          return;
                      }
                      assignment.AssignmentId = assignmentId;
                      assignment.UserId = taskShedulerContext.User.UserId;
                      taskShedulerContext.Assignments.Add(assignment);
                      errorInfo.ClearErrors(errorInfo);
                      ShowLoading = false;
                  }));
            }
        }
        private RelayCommand cleanCreateAssignment;
        public RelayCommand CleanCreateAssignment {
            get {
                return cleanCreateAssignment ??
                  (cleanCreateAssignment = new RelayCommand(o => {
                      CleanProperties();
                      errorInfo.ClearErrors(errorInfo);
                  }));
            }
        }
        private RelayCommand editAssignment;
        public RelayCommand EditAssignment {
            get {
                return editAssignment ??
                  (editAssignment = new RelayCommand(async o => {
                      ShowLoading = true;
                      DateTime dateTime = new DateTime(AssignmentDate.Year, AssignmentDate.Month, AssignmentDate.Day, AssignmentTime.Hour, AssignmentTime.Minute, AssignmentTime.Second);
                      if (AssignmentState != true) {
                          if (dateTime > DateTime.Now) AssignmentState = null;
                          else AssignmentState = false;
                      }
                      Assignment assignment = new Assignment(AssignmentName, AssignmentDescription, dateTime) { AssignmentId = SelectedAssignment.AssignmentId, UserId = SelectedAssignment.UserId, AssignmentState = AssignmentState };
                      var context = new ValidationContext(assignment);
                      var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                      if (!Validator.TryValidateObject(assignment, context, results, true)) {
                          ErrorInfo.SetValidationErrors(errorInfo, results);
                          ShowLoading = false;
                          return;
                      }

                      try {
                          await APIFunction.UpdateAssignmentAsync(assignment);
                      }
                      catch (ServerException ex) {
                          ErrorInfo.SetServerErrors(errorInfo, ex.ResponseMessage);
                          ShowLoading = false;
                          return;
                      }
                      int index = taskShedulerContext.Assignments.IndexOf((from a in taskShedulerContext.Assignments where a.AssignmentId == SelectedAssignment.AssignmentId select a).First());
                      taskShedulerContext.Assignments[index] = assignment;
                      SelectedAssignment = assignment;
                      errorInfo.ClearErrors(errorInfo);
                      ShowLoading = false;
                  }));
            }
        }
        private RelayCommand rollbacklEditAssignment;
        public RelayCommand RollbacklEditAssignment {
            get {
                return rollbacklEditAssignment ??
                  (rollbacklEditAssignment = new RelayCommand(o => {
                      errorInfo.ClearErrors(errorInfo);
                      AssignmentName = SelectedAssignment.AssignmentName;
                      AssignmentDescription = SelectedAssignment.AssignmentDescription;
                      AssignmentDate = SelectedAssignment.AssignmentTime;
                      AssignmentTime = SelectedAssignment.AssignmentTime;
                      AssignmentState = SelectedAssignment.AssignmentState;
                  }));
            }
        }
        private RelayCommand deleteAssignment;
        public RelayCommand DeleteAssignment {
            get {
                return deleteAssignment ??
                  (deleteAssignment = new RelayCommand(async o => {
                      ShowLoading = true;
                      try {
                          await APIFunction.DeleteAssignmentAsync(SelectedAssignment.AssignmentId);
                      }
                      catch (ServerException ex) {
                          ErrorInfo.SetServerErrors(errorInfo, ex.ResponseMessage);
                          ShowLoading = false;
                          return;
                      }
                      taskShedulerContext.Assignments.Remove((from a in taskShedulerContext.Assignments where a.AssignmentId == SelectedAssignment.AssignmentId select a).First());
                      ShowLoading = false;
                  }));
            }
        }
        private RelayCommand copyAssignment;
        public RelayCommand CopyAssignment {
            get {
                return copyAssignment ??
                  (copyAssignment = new RelayCommand(o => {
                      errorInfo.ClearErrors(errorInfo);

                      AssignmentName = SelectedAssignment.AssignmentName;
                      AssignmentDescription = SelectedAssignment.AssignmentDescription;
                      AssignmentDate = SelectedAssignment.AssignmentTime;
                      AssignmentTime = SelectedAssignment.AssignmentTime;

                      UserControl createTaskView = new CreateTaskView();
                      createTaskView.DataContext = this;
                      CurrentView = createTaskView;
                  }));
            }
        }
        private RelayCommand checkStateInForm;
        public RelayCommand CheckStateInForm {
            get {
                return checkStateInForm ??
                  (checkStateInForm = new RelayCommand(o => {
                      DateTime dateTime = new DateTime(AssignmentDate.Year, AssignmentDate.Month, AssignmentDate.Day, AssignmentTime.Hour, AssignmentTime.Minute, AssignmentTime.Second);
                      if (AssignmentState != true) {
                          if (dateTime > DateTime.Now) AssignmentState = null;
                          else AssignmentState = false;
                      }
                  }));
            }
        }

        Func<Assignment, object>[] sortVariation = new Func<Assignment, object>[] { e => e.AssignmentName, e => e.AssignmentTime, e => e.AssignmentState };
        private RelayCommand checkSort;
        public RelayCommand CheckSort {
            get {
                return checkSort ??
                  (checkSort = new RelayCommand(o => {
                      int i = Convert.ToInt32(o);
                      try {
                          assignmentsFiltrSort.sortFunc = sortVariation[i];
                      }
                      catch (Exception) { return; }
                      CollectionAssignments = assignmentsFiltrSort.ApplyFilters(taskShedulerContext.Assignments);
                  }));
            }
        }

        private RelayCommand checkFiltrState;
        public RelayCommand CheckFiltrState {
            get {
                return checkFiltrState ??
                  (checkFiltrState = new RelayCommand(o => {
                      bool? state;
                      try { state = Convert.ToBoolean(o); }
                      catch (Exception) { state = null; }
                      if (assignmentsFiltrSort.states.Contains(state)) {
                          assignmentsFiltrSort.states.Remove(state);
                          if (assignmentsFiltrSort.states.Count == 0) assignmentsFiltrSort.SelectFilterCommand(1);
                      }
                      else {
                          if (assignmentsFiltrSort.states.Count == 0) assignmentsFiltrSort.SelectFilterCommand(1);
                          assignmentsFiltrSort.states.Add(state);
                      }
                      CollectionAssignments = assignmentsFiltrSort.ApplyFilters(taskShedulerContext.Assignments);
                  }));
            }
        }
        private RelayCommand checkFiltrDate;
        public RelayCommand CheckFiltrDate {
            get {
                return checkFiltrDate ??
                  (checkFiltrDate = new RelayCommand(o => {
                      int i = Convert.ToInt32(o);
                      assignmentsFiltrSort.SelectFilterCommand(i);
                      CollectionAssignments = assignmentsFiltrSort.ApplyFilters(taskShedulerContext.Assignments);
                  }));
            }
        }
        private RelayCommand reboot;
        public RelayCommand Reboot {
            get {
                return reboot ??
                  (reboot = new RelayCommand(async o => {
                      ShowLoading = true;
                      ObservableCollection<Assignment> assignments;
                      try {
                          assignments = await APIFunction.GetAssignmentsAsync();
                      }
                      catch (ServerException) {
                          assignments = taskShedulerContext.Assignments;
                      }
                      bool? rollback;
                      Task task = Task.Run(async () => {
                          foreach (Assignment assignment in assignments) {
                              if (assignment.AssignmentState == true) continue;
                              rollback = assignment.AssignmentState;
                              if (assignment.AssignmentTime > DateTime.Now) assignment.AssignmentState = null;
                              else assignment.AssignmentState = false;
                              try {
                                  await APIFunction.UpdateAssignmentAsync(assignment);
                              }
                              catch (ServerException) {
                                  assignment.AssignmentState = rollback;
                              }
                          }
                      });
                      await task;
                      taskShedulerContext.Assignments = assignments;
                      CollectionAssignments = assignmentsFiltrSort.ApplyFilters(taskShedulerContext.Assignments);
                      taskShedulerContext.Assignments.CollectionChanged += Assignments_CollectionChanged;
                      ShowLoading = false;
                  }));
            }
        }
        #endregion

        public TaskViewModel() {
            taskShedulerContext = new TaskShedulerContext();
            assignmentsFiltrSort = new AssignmentsFiltrSort();
            assignmentsFiltrSort.SelectFilterCommand(0);
            collectionAssignments = taskShedulerContext.Assignments;
            taskShedulerContext.Assignments.CollectionChanged += Assignments_CollectionChanged;
            DateFrom = DateTime.Today;
            DateTo = DateTime.Today;

            NewCreateTaskView();
        }

        #region *** Methods ***
        private void Assignments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            CollectionAssignments = assignmentsFiltrSort.ApplyFilters(taskShedulerContext.Assignments);
        }
        private void CleanProperties() {
            AssignmentName = "";
            AssignmentDescription = "";
            AssignmentTime = DateTime.Now;
            AssignmentDate = DateTime.Now;
        }
        private void SelectedAssignment_PropertyChanged() {
            errorInfo.ClearErrors(errorInfo);

            AssignmentName = SelectedAssignment.AssignmentName;
            AssignmentDescription = SelectedAssignment.AssignmentDescription;
            AssignmentDate = SelectedAssignment.AssignmentTime;
            AssignmentTime = SelectedAssignment.AssignmentTime;
            AssignmentState = SelectedAssignment.AssignmentState;

            UserControl editTaskView = new EditTaskView();
            editTaskView.DataContext = this;
            CurrentView = editTaskView;
        }
        private void NewCreateTaskView() {
            CleanProperties();
            UserControl createTaskView = new CreateTaskView();
            createTaskView.DataContext = this;
            CurrentView = createTaskView;
        }
        #endregion
    }
}