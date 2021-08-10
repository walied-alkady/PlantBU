using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetPage'
    public partial class BudgetPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetPage.BudgetPage()'
        public BudgetPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetPage.BudgetPage()'
        {
            InitializeComponent();
            (BindingContext as BudgetViewModel).Navigation = Navigation;
            (BindingContext as BudgetViewModel).page = this;
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

            var contacts = obj as Budgetitem;
            if (
                (contacts.PartCode != null ? contacts.PartCode.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Description != null ? contacts.Description.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.ItemType != null ? contacts.ItemType.Contains(searchBar.Text.ToLower()) : false)
                  || (contacts.Title != null ? contacts.Title.Contains(searchBar.Text.ToLower()) : false)
#pragma warning disable CS0472 // The result of the expression is always 'true' since a value of type 'double' is never equal to 'null' of type 'double?'
                   || (contacts.Value != null ? contacts.Value == double.Parse(searchBar.Text) : false)
#pragma warning restore CS0472 // The result of the expression is always 'true' since a value of type 'double' is never equal to 'null' of type 'double?'
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
                            Budgetitem budgetitem = new Budgetitem()
                            {
                                Title = "New Budget Item",
                            };
                            Plant pl;
                            if (DBManager.realm.All<Plant>().Any())
                            {
                                pl = DBManager.realm.All<Plant>().First();
                                pl.BudgetAdd(budgetitem);
                            }
                            else
                                throw new Exception(Properties.Resources.Plant + Properties.Resources.NotFound);
                            await Navigation.PushAsync(new BudgetitemPage(budgetitem, true));

                            break;
                        case "BudgetExpense":
                                await Navigation.PushAsync(new BudgetToExpensePage());
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