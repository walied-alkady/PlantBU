using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlantBU.DataModel;
using PlantBU.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentsPage'
    public partial class OtherComponentsPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentsPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentsPage.OtherComponentsPage()'
        public OtherComponentsPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentsPage.OtherComponentsPage()'
        {
            InitializeComponent();
            (BindingContext as OtherComponentsViewModel).page = this;
            (BindingContext as OtherComponentsViewModel).Navigation = Navigation;
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

            var OtherComponent = obj as OtherComponent;
            double value; bool IsDouble = double.TryParse(searchBar.Text, out value);

            if (
                    (OtherComponent.Code != null ? OtherComponent.Code.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (OtherComponent.Description != null ? OtherComponent.Description.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (OtherComponent.Brand != null ? OtherComponent.Brand.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (OtherComponent.BrandType != null ? OtherComponent.BrandType.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (OtherComponent.ExtraData != null ? OtherComponent.ExtraData.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 )
                return true;
            else
                return false;
        }
    }
}