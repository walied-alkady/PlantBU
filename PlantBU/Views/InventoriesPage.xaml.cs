using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoriesPage'
    public partial class InventoriesPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoriesPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoriesPage.InventoriesPage()'
        public InventoriesPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoriesPage.InventoriesPage()'
        {
            InitializeComponent();
            (BindingContext as InventoriesViewModel).Navigation = Navigation;
            (BindingContext as InventoriesViewModel).page = this;
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
                            Plant pl;
                            if (DBManager.realm.All<Plant>().Any())
                            {
                                pl = DBManager.realm.All<Plant>().First();
                                pl.InventoryAdd("Inv", "");
                            }
                            else
                                throw new Exception(Properties.Resources.Plant + Properties.Resources.NotFound);
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