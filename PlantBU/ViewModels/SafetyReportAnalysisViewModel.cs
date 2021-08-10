using PlantBU.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PlantBU.ViewModels
{
    class SafetyReportAnalysisViewModel : BaseViewModel
    {
        public List<SafetyReport> SafetyReports { get { return _SafetyReports; } set { _SafetyReports = value; OnPropertyChanged("SafetyReports"); } }
        List<SafetyReport> _SafetyReports;
        public List<Employee> employees { get { return _employees; } set { _employees = value; OnPropertyChanged("employees"); } }
        List<Employee> _employees;
        public Employee Employee { get { return _employee; } set { _employee = value; OnPropertyChanged("Employee"); } }
        Employee _employee;
        public List<SafetyReportAnalysisChartDataUnit> ChartData { get { return _ChartData; } set { _ChartData = value; OnPropertyChanged("ChartData"); } }
        List<SafetyReportAnalysisChartDataUnit> _ChartData;
        public SafetyReportAnalysisViewModel()
        {
            PopulateData();
        }
        internal void PopulateData()
        {
            IsBusy = true;
            employees = DBManager.realm.All<Employee>().ToList();
            SafetyReports = GetItems<SafetyReport>().ToList();
            if (SafetyReports != null && Employee != null)
                ChartData = new List<SafetyReportAnalysisChartDataUnit>()
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
                };
            IsBusy = false;
        }
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisChartDataUnit'
    public class SafetyReportAnalysisChartDataUnit
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisChartDataUnit'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisChartDataUnit.SafetyReportAnalysisChartDataUnit(string, int)'
        public SafetyReportAnalysisChartDataUnit(string month, int reportCount)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisChartDataUnit.SafetyReportAnalysisChartDataUnit(string, int)'
        {
            Month = month; ReportsCount = reportCount;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisChartDataUnit.Month'
        public string Month { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisChartDataUnit.Month'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisChartDataUnit.ReportsCount'
        public int ReportsCount { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisChartDataUnit.ReportsCount'
    }
}
