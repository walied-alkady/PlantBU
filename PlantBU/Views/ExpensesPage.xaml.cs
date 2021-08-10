using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensesPage'
    public partial class ExpensesPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensesPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ExpensesPage.ExpensesPage()'
        public ExpensesPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ExpensesPage.ExpensesPage()'
        {
            InitializeComponent();
            (BindingContext as ExpensesViewModel).Navigation = Navigation;
            (BindingContext as ExpensesViewModel).page = this;


        }
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (listView.DataSource != null)
            {
                this.listView.DataSource.Filter = FilterEquipments;
                this.listView.DataSource.RefreshFilter();
            }
        }
        private bool FilterEquipments(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var contacts = obj as ExpensItem;
            if (
                    (contacts.PartCode != null ? contacts.PartCode.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Description != null ? contacts.Description.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.ItemType != null ? contacts.ItemType.Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Title != null ? contacts.Title.Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Value == double.Parse(searchBar.Text))
                 )
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
                            ExpensItem newEq = new ExpensItem()
                            {
                                Title = "New Expsense",
                                Description = "New Expsense details"
                            };
                            DBManager.AddItem(newEq);
                            await Navigation.PushAsync(new ExpensePage(newEq, true));

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