using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Syncfusion.DataSource.Extensions;
using PlantBU.DataModel;
using PlantBU.ViewModels;

namespace PlantBU.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportsPage'
	public partial class SafetyReportsPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportsPage'
	{
        GroupResult expandedGroup;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportsPage.SafetyReportsPage()'
        public SafetyReportsPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportsPage.SafetyReportsPage()'
		{
			InitializeComponent ();
            (BindingContext as SafetyReportsViewModel).page = this;
            (BindingContext as SafetyReportsViewModel).Navigation = Navigation;
        }
		private async void ToolbarItem_Clicked(object sender, EventArgs e)
		{
            try
            {
#pragma warning disable CS0219 // The variable 'action' is assigned but its value is never used
                string action = "";
#pragma warning restore CS0219 // The variable 'action' is assigned but its value is never used
                if (sender is ToolbarItem)
                {
                   
                    switch ((sender as ToolbarItem).StyleId)
                    {
                        case "Add":
                            SafetyReport newReport = null;
                            Plant pl = DBManager.realm.All<Plant>().First();
                            DBManager.realm.Write(() =>
                            {
                                if (pl.Safety == null)
                                    pl.Safety = new Safety();
                                    if (DBManager.CurrentUser.Title == "Technician")
                                    newReport = new SafetyReport()
                                    {
                                        IssueDate = DateTimeOffset.Now.LocalDateTime,
                                        IssueOnCompany = "Titan",
                                        IssueOnCompanyName = "Titan",
                                        ReporterDepartment = "Electrical",
                                        ReporterName = DBManager.CurrentUser.FirstName + " " + DBManager.CurrentUser.LastName,
                                        ReportDetailsType = "Unsafe_Condition",
                                        ReportDetailsRisk = "Low",
                                        DueDate = DateTime.Now.AddDays(30).ToLocalTime(),
                                        Status = "OPEN",

                                    };
                                    else
                                    newReport = new SafetyReport()
                                    {
                                        IssueDate = DateTimeOffset.Now.LocalDateTime,
                                        ReporterName = DBManager.CurrentUser.FirstName + " " + DBManager.CurrentUser.LastName,
                                        ReportDetailsType = "Unsafe_Condition",
                                        ReportDetailsRisk = "Low",
                                        DueDate = DateTime.Now.AddDays(30).ToLocalTime(),
                                        Status = "OPEN",

                                    };
                                pl.Safety.SafetyReports.Add(newReport);
                            });

                            await Navigation.PushAsync(new SafetyReportPage(newReport, true,true));

                            break;
                        case "SafetyReportsAna":
                            await Navigation.PushAsync(new SafetyReportsAnalysisPage());
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
		private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
		{ }
        private void listView_GroupExpanding(object sender, GroupExpandCollapseChangingEventArgs e)
        {
            if (e.Groups.Count > 0)
            {
                var group = e.Groups[0];
                if (expandedGroup == null || group.Key != expandedGroup.Key)
                {
                    foreach (var otherGroup in listView.DataSource.Groups)
                    {
                        if (group.Key != otherGroup.Key)
                        {
                            listView.CollapseGroup(otherGroup);
                        }
                    }
                    expandedGroup = group;
                    listView.ExpandGroup(expandedGroup);
                }
            }
        }
        private void listView_Loaded(object sender, ListViewLoadedEventArgs e)
        {
            listView.CollapseAll();
        }
    }
}