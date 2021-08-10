using PlantBU.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantBU.ViewModels
{
    class EmployeeSchedulePerformanceViewModel : BaseViewModel
    {
        public List<Schedule> Schedules { get { return _Schedules; } set { _Schedules = value; OnPropertyChanged("Schedules"); } }
        List<Schedule> _Schedules;
        public List<Employee> employees { get { return _employees; } set { _employees = value; OnPropertyChanged("employees"); } }
        List<Employee> _employees;
        public string Employee { get { return _employee; } set { _employee = value; OnPropertyChanged("employee"); } }
        string _employee;
        public List<EmployeeSchedulePerformanceChartDataUnit> ChartData { get { return _ChartData; } set { _ChartData = value; OnPropertyChanged("ChartData"); } }
        List<EmployeeSchedulePerformanceChartDataUnit> _ChartData;
        public string TasksTotal
        {
            get
            {
                if (Schedules != null && !string.IsNullOrEmpty(Employee))
                    return (from emp in Schedules where emp.AssigneeCompanyCode == Employee  select emp).Count().ToString();
                else
                    return "";
            }
        }
        public string TasksTotalDone
        {
            get
            {
                if (Schedules != null && Employee != null)
                    return (from emp in Schedules 
                            where emp.AssigneeCompanyCode == Employee && 
                            emp.StatusSchedule == true
                            select emp).Count().ToString();
                else
                    return "";
            }
        }
        public EmployeeSchedulePerformanceViewModel()
        {
            employees = DBManager.realm.All<Employee>().ToList();
        }
        internal void PopulateData()
        {
            IsBusy = true;
            Schedules = GetItems<Schedule>().ToList();
            ChartData = new List<EmployeeSchedulePerformanceChartDataUnit>();
            if (Schedules != null)
                foreach (var em in employees)
                {
                    var xd = Schedules.Count(x => x.AssigneeCompanyCode == em.CompanyCode);
                    var xd1 = Schedules.Count(x => x.AssigneeCompanyCode == em.CompanyCode && x.StatusSchedule == true);

                    var newdata = new EmployeeSchedulePerformanceChartDataUnit(em.FirstName + " " + em.LastName, Schedules.Count(x => x.AssigneeCompanyCode == em.CompanyCode),
                        Schedules.Count(x => x.AssigneeCompanyCode == em.CompanyCode && x.StatusSchedule == true));
                    ChartData.Add(newdata);
                 }
/*                ChartData = new List<EmployeeSchedulePerformanceChartDataUnit>()
                {
                    new EmployeeSchedulePerformanceChartDataUnit ("Jan" ,(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Feb",(from emp in Schedules where emp.SetDate.Month==2 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName  select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Mar", (from emp in Schedules where emp.SetDate.Month==3 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName  select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Apr", (from emp in Schedules where emp.SetDate.Month==4 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName  select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "May", (from emp in Schedules where emp.SetDate.Month==5 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Jun", (from emp in Schedules where emp.SetDate.Month==6 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName   select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Jul", (from emp in Schedules where emp.SetDate.Month==7 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Aug", (from emp in Schedules where emp.SetDate.Month==8 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Sep", (from emp in Schedules where emp.SetDate.Month==9 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName  select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Oct", (from emp in Schedules where emp.SetDate.Month==10 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Nov", (from emp in Schedules where emp.SetDate.Month==11 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName  select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count()),
                    new EmployeeSchedulePerformanceChartDataUnit ( "Dec", (from emp in Schedules where emp.SetDate.Month==12 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName  select emp).Count(),(from emp in Schedules where emp.SetDate.Month==1 && emp.AssigneeFirstName == Employee.FirstName && emp.AssigneeLastName == Employee.LastName && emp.StatusSchedule == true select emp).Count())
                };
*/            IsBusy = false;
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit'
    public class EmployeeSchedulePerformanceChartDataUnit
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit.EmployeeSchedulePerformanceChartDataUnit(string, int, int)'
        public EmployeeSchedulePerformanceChartDataUnit(string employee, int tasksno, int tasksnoDone)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit.EmployeeSchedulePerformanceChartDataUnit(string, int, int)'
        {
            Employee = employee; TasksNo = tasksno; TasksNoDone = tasksnoDone;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit.Employee'
        public string Employee { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit.Employee'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit.TasksNo'
        public int TasksNo { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit.TasksNo'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit.TasksNoDone'
        public int TasksNoDone { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformanceChartDataUnit.TasksNoDone'
    }
}
