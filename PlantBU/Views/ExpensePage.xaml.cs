using PlantBU.DataModel;
using PlantBU.ViewModels;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.SfDataGrid.XForms.Renderers;
using Syncfusion.XForms.ComboBox;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensePage'
    public partial class ExpensePage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensePage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensePage.ExpensItem'
        public ExpensItem ExpensItem { get { return _ExpensItem; } set { _ExpensItem = value; OnPropertyChanged("ExpensItem"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensePage.ExpensItem'
        ExpensItem _ExpensItem;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensePage.ExpensePage(ExpensItem, bool)'
        public ExpensePage(ExpensItem emp, bool IsEnabled = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensePage.ExpensePage(ExpensItem, bool)'
        {
            InitializeComponent();
            try
            {
                ExpensItem = emp;
                TitleEditor.Text = ExpensItem.Title; TitleEditor.IsEnabled = IsEnabled;
                DescriptionEditor.Text = ExpensItem.Description; DescriptionEditor.IsEnabled = IsEnabled;
                ServiceTypecomboBox.SelectedItem = ExpensItem.ItemType; ServiceTypecomboBox.IsEnabled = IsEnabled;
                PartCodeAutoComplete.Text = ExpensItem.PartCode; PartCodeAutoComplete.IsEnabled = IsEnabled;
                ValueEditor.Text = ExpensItem.Value.ToString(); ValueEditor.IsEnabled = IsEnabled;
                datefromPicker.Date = ExpensItem.DateExpense.Date; datefromPicker.IsEnabled = IsEnabled;

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
                                ExpensItem.PartCode = (sender as SfComboBox).Text;
                                break;
                        }
                    }
                    else if (sender is SfAutoComplete)
                    {
                        switch ((sender as SfAutoComplete).StyleId)
                        {
                            case "PartCodeAutoComplete":
                                ExpensItem.PartCode = (sender as SfAutoComplete).Text;
                                break;
                        }
                    }
                    else if (sender is Editor)
                    {
                        switch ((sender as Editor).StyleId)
                        {
                            case "TitleEditor":
                                ExpensItem.Title = (sender as Editor).Text;
                                break;
                            case "DescriptionEditor":
                                ExpensItem.Description = (sender as Editor).Text;
                                break;

                            case "ValueEditor":
                                ExpensItem.Value = double.Parse((sender as Editor).Text);
                                break;
                        }
                    }
                    else if (sender is SfDatePicker)
                        switch ((sender as SfDatePicker).StyleId)
                        {
                            case "datefromPicker":
                                //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                                ExpensItem.DateExpense = (sender as SfDatePicker).Date;
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