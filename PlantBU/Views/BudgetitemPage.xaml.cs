using PlantBU.DataModel;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.SfDataGrid.XForms.Renderers;
using Syncfusion.XForms.ComboBox;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetitemPage'
    public partial class BudgetitemPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetitemPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetitemPage.Budgetitem'
        public Budgetitem Budgetitem { get { return _Budgetitem; } set { _Budgetitem = value; OnPropertyChanged("Budgetitem"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetitemPage.Budgetitem'
        Budgetitem _Budgetitem;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetitemPage.BudgetitemPage(Budgetitem, bool)'
        public BudgetitemPage(Budgetitem emp, bool IsEnabled = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetitemPage.BudgetitemPage(Budgetitem, bool)'
        {
            InitializeComponent();
            try
            {
                Budgetitem = emp;
                TitleEditor.Text = Budgetitem.Title; TitleEditor.IsEnabled = IsEnabled;
                DescriptionEditor.Text = Budgetitem.Description; DescriptionEditor.IsEnabled = IsEnabled;
                ServiceTypecomboBox.SelectedValue = Budgetitem.ItemType; ServiceTypecomboBox.IsEnabled = IsEnabled;
                PartCodeAutoComplete.Text = Budgetitem.PartCode; PartCodeAutoComplete.IsEnabled = IsEnabled;
                ValueEditor.Text = Budgetitem.Value.ToString(); ValueEditor.IsEnabled = IsEnabled;
                datefromPicker.Date = Budgetitem.DateBudgetItem.Date; datefromPicker.IsEnabled = IsEnabled;

            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private void Editor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    if (sender is SfComboBox)
                    {
                        switch ((sender as SfComboBox).StyleId)
                        {
                            case "ServiceTypecomboBox":
                                Budgetitem.PartCode = (sender as SfComboBox).Text;
                                break;
                        }
                    }
                    else if (sender is SfAutoComplete)
                    {
                        switch ((sender as SfAutoComplete).StyleId)
                        {
                            case "PartCodeAutoComplete":
                                Budgetitem.PartCode = (sender as SfAutoComplete).Text;
                                break;
                        }
                    }
                    else if (sender is Editor)
                    {
                        switch ((sender as Editor).StyleId)
                        {
                            case "TitleEditor":
                                Budgetitem.Title = (sender as Editor).Text;
                                break;
                            case "DescriptionEditor":
                                Budgetitem.Description = (sender as Editor).Text;
                                break;

                            case "ValueEditor":
                                Budgetitem.Value = double.Parse((sender as Editor).Text);
                                break;
                        }
                    }
                    else if (sender is SfDatePicker)
                        switch ((sender as SfDatePicker).StyleId)
                        {
                            case "datefromPicker":
                                //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                                Budgetitem.DateBudgetItem = (sender as SfDatePicker).Date;
                                break;

                        }
                });
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
    }
}