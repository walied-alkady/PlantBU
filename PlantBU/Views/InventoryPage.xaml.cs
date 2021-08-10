// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-01-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="InventoryPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    /// <summary>
    /// Class InventoryPage.
    /// Implements the <see cref="Xamarin.Forms.ContentPage" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryPage : ContentPage
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoryPage.InventoryPage(Inventory, bool)'
        public InventoryPage(Inventory inventory, bool IsEnabled = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoryPage.InventoryPage(Inventory, bool)'
        {
            InitializeComponent();
            try
            {
                CircularProgressBar.IsVisible = false;
                (BindingContext as InventoryViewModel).Navigation = Navigation;
                (BindingContext as InventoryViewModel).page = this;
                (BindingContext as InventoryViewModel).Inventory = inventory;
                (BindingContext as InventoryViewModel).AddProducts(0, 10);

                if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                    (BindingContext as InventoryViewModel).IsEnabled = true;
                else
                    (BindingContext as InventoryViewModel).IsEnabled = false;
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoryPage.OnAppearing()'
        protected override async void OnAppearing()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoryPage.OnAppearing()'
        {
            base.OnAppearing();
            try
            {
                IsBusy = true;
                DeviceDisplay.KeepScreenOn = true;
                if (!DBManager.IsInventoryLoaded && !DBManager.realm.IsInTransaction)
                {

                    string action = await DisplayActionSheet(Properties.Resources.InventoryLoadMessage, Properties.Resources.Cancel, null, Properties.Resources.Ok, Properties.Resources.No);
                    if (action == Properties.Resources.Ok)
                    {
                        CircularProgressBar.IsVisible = true;
                        listView.IsVisible = false;
                        searchBar.IsVisible = false;
                        IProgress<int> progressIndicator = new Progress<int>(ReportProgress);
                       await DBManager.DBLoadInventory(progressIndicator, (BindingContext as InventoryViewModel).Inventory.Code);
                        listView.IsVisible = true;
                        searchBar.IsVisible = true;
                        CircularProgressBar.IsVisible = false;

                    }
                    else
                    {
                        DeviceDisplay.KeepScreenOn = false;
                        IsBusy = false;
                        await Navigation.PopAsync();
                    }
                }
                DeviceDisplay.KeepScreenOn = false;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
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

            var spare = obj as Spare;
            if (
                    (spare.Code != null ? spare.Code.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (spare.Description1 != null ? spare.Description1.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (spare.Description2 != null ? spare.Description2.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (spare.Brand != null ? spare.Brand.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (spare.BrandType != null ? spare.BrandType.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (spare.ExtraData != null ? spare.ExtraData.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (spare.Location != null ? spare.Location.ToLower().Contains(searchBar.Text.ToLower()) : false))
                return true;
            else
                return false;
        }
        void ReportProgress(int value)
        {
            //Update the UI to reflect the progress value that is passed back.
            CustomContentProgressBarLabel.Text = string.Format("{0:00} %", value);
            CircularProgressBar.Progress = value;

        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                Inventory spr = (BindingContext as InventoryViewModel).Inventory;
                if (sender is ToolbarItem)
                {
                    switch ((sender as ToolbarItem).StyleId)
                    {
                        case "Add":
                            Spare newsp = new Spare()
                            {
                                Code = "000AA00AA00",
                                Description1 = "New Spare"
                            };
                            spr.SpareAdd(newsp);
                            await Navigation.PushAsync(new SparePage(newsp, true));
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


