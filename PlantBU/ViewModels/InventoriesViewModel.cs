// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-01-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-07-2021
// ***********************************************************************
// <copyright file="InventoryViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.Views;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class InventoryViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    public class InventoriesViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the spares.
        /// </summary>
        /// <value>The spares.</value>
        public List<Inventory> Inventories { get { return _Inventories; } set { _Inventories = value; OnPropertyChanged("Inventories"); } }
        /// <summary>
        /// The spares
        /// </summary>
        private List<Inventory> _Inventories;


        /// <summary>
        /// Gets the ListView tapped command.
        /// </summary>
        /// <value>The ListView tapped command.</value>
        public Command<object> ListViewTappedCommand { get; private set; }
        /// <summary>
        /// Gets the ListView holding command.
        /// </summary>
        /// <value>The ListView holding command.</value>
        public Command<object> ListViewHoldingCommand { get; private set; }

        /// <summary>
        /// Gets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public ICommand SearchCommand => new Command<string>((string query) =>
            {
                Inventories = GetItems<Inventory>(query);
            });
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryViewModel"/> class.
        /// </summary>
#pragma warning disable CS0169 // The field 'InventoriesViewModel.index' is never used
        int index;
#pragma warning restore CS0169 // The field 'InventoriesViewModel.index' is never used
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoriesViewModel.InventoriesViewModel()'
        public InventoriesViewModel()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoriesViewModel.InventoriesViewModel()'
        {
            IsBusy = true;
            if (DBManager.IsInventoryLoaded)
            {
                ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
                ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            }
            IsBusy = false;
        }

        /// <summary>
        /// ListViews the tapped command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                IsBusy = true;
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Spare)
                {

                    Inventory inv = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Inventory;
                    if (inv.IsValid)
                    {
                        await Navigation.PushAsync(new InventoryPage(inv));
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Spare + Properties.Resources.NotAvailable , Properties.Resources.Ok);
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        /// <summary>
        /// ListViews the holding command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Inventory)
                    && DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                {

                    Inventory spr = (Inventory)(obj as ItemHoldingEventArgs).ItemData;
                    if (spr.IsValid)
                    {
                        string action = await page.DisplayActionSheet(spr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);

                        if (action == Properties.Resources.Edit)
                        {
                            await Navigation.PushAsync(new InventoryPage(spr, true));
                        }
                        else if (action == Properties.Resources.Details)
                        {
                            await Navigation.PushAsync(new InventoryPage(spr));
                        }
                        else if (action == Properties.Resources.Remove)
                        {
                            var pls = DBManager.realm.All<Plant>();
                            if (pls.Any())
                            {
                                pls.First().InventoryDelete(spr);
                                await page.DisplayAlert(Properties.Resources.Info, spr.Code + Properties.Resources.Deleted, Properties.Resources.Ok);
                            }
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Inventory + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        /// <summary>
        /// Refreshes the items.
        /// </summary>
        public void RefreshItems()
        {
            IsBusy = true;
            if (Inventories != null)
                Inventories.Clear();
            Inventories = GetItems<Inventory>();
            IsBusy = false;
        }

    }
}
