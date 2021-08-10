using PlantBU.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PlantBU.ViewModels
{
    class SafetyReportsAnalysisViewModel : BaseViewModel
    {
        public List<SafetyReport> SafetyReports { get { return _SafetyReports; } set { _SafetyReports = value; OnPropertyChanged("SafetyReports"); } }
        List<SafetyReport> _SafetyReports;
        public List<Employee> employees { get { return _employees; } set { _employees = value; OnPropertyChanged("employees"); } }
        List<Employee> _employees;
        public Employee Employee { get { return _employee; } set { _employee = value; OnPropertyChanged("Employee"); } }
        Employee _employee;
        public List<SafetyReportAnalysisChartDataUnit> ChartData { get { return _ChartData; } set { _ChartData = value; OnPropertyChanged("ChartData"); } }
        List<SafetyReportAnalysisChartDataUnit> _ChartData;
        public SafetyReportsAnalysisViewModel()
        {
            PopulateData();
        }
        internal void PopulateData()
        {
            IsBusy = true;
            employees = DBManager.realm.All<Employee>().ToList();
            SafetyReports = GetItems<SafetyReport>().ToList();
            ChartData = new List<SafetyReportAnalysisChartDataUnit>();

            if (SafetyReports != null)
                foreach (var em in employees)
                {
                    // var xd = SafetyReports.Count(x => x.ReporterName == em.FullNameAr);
                    //em.FirstName + " " + em.LastName
                    var newdata = new SafetyReportAnalysisChartDataUnit(em.FullNameAr, SafetyReports.Count(x => x.ReporterName == em.FullNameAr));
                    ChartData.Add(newdata);
                }
            /*ChartData = new List<SafetyReportAnalysisChartDataUnit>()
                {
                    new SafetyReportAnalysisChartDataUnit ("Jan" ,(from emp in SafetyReports where emp.IssueDate.Month==1 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Feb",(from emp in SafetyReports where emp.IssueDate.Month==2 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Mar", (from emp in SafetyReports where emp.IssueDate.Month==3 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Apr", (from emp in SafetyReports where emp.IssueDate.Month==4 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "May", (from emp in SafetyReports where emp.IssueDate.Month==5 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Jun", (from emp in SafetyReports where emp.IssueDate.Month==6 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Jul", (from emp in SafetyReports where emp.IssueDate.Month==7 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Aug", (from emp in SafetyReports where emp.IssueDate.Month==8 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Sep", (from emp in SafetyReports where emp.IssueDate.Month==9 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Oct", (from emp in SafetyReports where emp.IssueDate.Month==10 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Nov", (from emp in SafetyReports where emp.IssueDate.Month==11 && emp.ReporterName == Employee.FullNameAr   select emp).Count()),
                    new SafetyReportAnalysisChartDataUnit ( "Dec", (from emp in SafetyReports where emp.IssueDate.Month==12 && emp.ReporterName == Employee.FullNameAr   select emp).Count())
                };*/
            IsBusy = false;
        }
    }
}
