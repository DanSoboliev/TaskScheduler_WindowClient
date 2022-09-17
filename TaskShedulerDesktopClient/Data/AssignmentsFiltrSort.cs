using System;
using System.Collections.Generic;
using System.Linq;
using TaskShedulerDesktopClient.Models;

namespace TaskShedulerDesktopClient.Data {
    class AssignmentsFiltrSort {
        public AssignmentsFiltrSort() {
            InitializeFilters();
            IniFilterCommandsInfo();
        }

        delegate void Filter(ref IEnumerable<Assignment> collection);

        Filter emptyFilter = delegate { };

        enum FilterId { ByState, ByName, ByDate };

        Filter[] filters = new Filter[Enum.GetValues(typeof(FilterId)).Length];

        private void InitializeFilters() {
            for (int i = 0; i < filters.Length; i++) {
                filters[i] = emptyFilter;
            }
        }

        class FilterCommandInfo {
            public FilterId filterId;
            public Filter filter;

            public FilterCommandInfo(FilterId filterId, Filter filter) {
                this.filterId = filterId;
                this.filter = filter;
            }
        }

        FilterCommandInfo[] filterCommandsInfo;
        private void IniFilterCommandsInfo() {
            filterCommandsInfo = new FilterCommandInfo[] {
                new FilterCommandInfo(FilterId.ByName, FiltrNameContains),
                new FilterCommandInfo(FilterId.ByState, FiltrState),
                new FilterCommandInfo(FilterId.ByDate, FiltrDateFrom),
                new FilterCommandInfo(FilterId.ByDate, FiltrDateTo),
            };
        }

        public void SelectFilterCommand(int number) {
            var commandInfo = filterCommandsInfo[number];
            var delegates = filters[(int)commandInfo.filterId].GetInvocationList();
            if (delegates.Contains(commandInfo.filter)) filters[(int)commandInfo.filterId] -= commandInfo.filter;
            else filters[(int)commandInfo.filterId] += commandInfo.filter;
        }

        public IEnumerable<Assignment> ApplyFilters(IEnumerable<Assignment> initialCollection) {
            var selectedInstances = initialCollection;
            foreach (Filter filter in filters) {
                filter(ref selectedInstances);
            }
            if(sortDirection) SortingOrderByDescending(ref selectedInstances, sortFunc);
            else SortingOrderBy(ref selectedInstances, sortFunc);
            return selectedInstances;
        }
        
        public Func<Assignment, object> sortFunc = e => e.AssignmentId;
        public bool sortDirection = false;
        private void SortingOrderBy<P>(ref IEnumerable<Assignment> selectedInstances, Func<Assignment, P> sort) {
            selectedInstances = selectedInstances.OrderBy(sort);
        }
        private void SortingOrderByDescending<P>(ref IEnumerable<Assignment> selectedInstances, Func<Assignment, P> sort) {
            selectedInstances = selectedInstances.OrderByDescending(sort);
        }

        public string searchName = "";
        private void FiltrNameContains(ref IEnumerable<Assignment> collection) {
            collection = from e in collection
                         where e.AssignmentName.Contains(searchName)
                         select e;
        }
        public DateTime dateFrom;
        private void FiltrDateFrom(ref IEnumerable<Assignment> collection) {
            collection = from e in collection
                         where e.AssignmentTime >= dateFrom
                         select e;
        }
        public DateTime dateTo;
        private void FiltrDateTo(ref IEnumerable<Assignment> collection) {
            collection = from e in collection
                         where e.AssignmentTime <= dateTo
                         select e;
        }
        public List<bool?> states = new List<bool?>();
        private void FiltrState(ref IEnumerable<Assignment> collection) {
            IEnumerable<Assignment> temporary = new List<Assignment>();
            foreach (bool? state in states) {
                temporary = temporary.Concat(from e in collection
                                             where e.AssignmentState == state
                                             select e);
            }
            collection = temporary;
        }
    }
}