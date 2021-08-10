using PlantBU.DataModel;
using PlantBU.Utilities;
using PlantBU.ViewModels;
using Syncfusion.DataSource;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'LogsPage'
    public partial class LogsPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'LogsPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'LogsPage.LogsPage()'
        public LogsPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'LogsPage.LogsPage()'
        {
            InitializeComponent();
            try
            {
                (BindingContext as LogsViewModel).Navigation = Navigation;
                (BindingContext as LogsViewModel).page = this;
                (BindingContext as LogsViewModel).RefreshItems();
                listView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
                {
                    PropertyName = "DateLog",
                    KeySelector = (object obj1) =>
                    {
                        var item = (obj1 as Log);
                        string monthName = new DateTime(item.DateLog.Year, item.DateLog.Month, item.DateLog.Day).ToString("MMM", CultureInfo.InvariantCulture);
                        return "  " + item.DateLog.Year + "-" + monthName;
                    },
                    Comparer = new CustomGroupComparerYearMonth()
                });
                Preferences.Set("NewLog", 0);
                Preferences.Set("NewLogsCount", 0);

            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (listView.DataSource != null)
            {
                this.listView.DataSource.Filter = FilterItems;
                this.listView.DataSource.RefreshFilter();
            }
        }
        private bool FilterItems(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;
            var contacts = obj as Log;
            var emp = DBManager.realm.All<Employee>().Where(x => x.CompanyCode == contacts.AssigneeCompanyCode).FirstOrDefault();

            if (
                    (contacts.ItemCode != null ? contacts.ItemCode.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.ItemDescription != null ? contacts.ItemDescription.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Repair != null ? contacts.Repair.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Repairdetails != null ? contacts.Repairdetails.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Notes != null ? contacts.Notes.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (emp != null ? emp.FirstName.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (emp != null ? emp.LastName.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.DateLog != null ? contacts.DateLog.Month.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.DateLog != null ? contacts.DateLog.Day.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.DateLog != null ? contacts.DateLog.Year.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false))
                return true;
            else
                return false;
        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is ToolbarItem)
                {
                   
                        switch ((sender as ToolbarItem).StyleId)
                        {

                            case "Add":
                                Log newlog = new Log()
                                {
                                    DateLog = DateTime.Now.Date
                                };
                                Plant pl = DBManager.realm.All<Plant>().First();
                                pl.LogAdd(newlog);
                                (BindingContext as LogsViewModel).RefreshItems();
                                await Navigation.PushAsync(new LogPage(newlog, true));
                                break;

                        }
                   

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

    }
}