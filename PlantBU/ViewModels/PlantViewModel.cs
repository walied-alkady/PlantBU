// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-18-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-06-2021
// ***********************************************************************
// <copyright file="EquipmentViewModel.cs" company="PlantBU">
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
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel'
    public class PlantViewModel : BaseViewModel
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.Plant'
        public Plant Plant { get { return _Plant; } set { _Plant = value; OnPropertyChanged("Plant"); OnPropertyChanged("Plant"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.Plant'
        Plant _Plant;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.ProductionLines'
        public List<ProductionLine> ProductionLines { get { return _ProductionLines; } set { _ProductionLines = value; OnPropertyChanged("ProductionLines"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.ProductionLines'
        List<ProductionLine> _ProductionLines;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.Shops'
        public List<Shop> Shops { get { return _Shops; } set { _Shops = value; OnPropertyChanged("Shops"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.Shops'
        List<Shop> _Shops;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.IsProductionsLineView'
        public bool IsProductionsLineView { get { return _IsProductionsLineView; } set { _IsProductionsLineView = value; OnPropertyChanged("IsProductionsLineView"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.IsProductionsLineView'
        bool _IsProductionsLineView;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.IsShopsView'
        public bool IsShopsView { get { return _IsShopsView; } set { _IsShopsView = value; OnPropertyChanged("IsShopsView"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.IsShopsView'
        bool _IsShopsView;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.ListViewTappedCommand'
        public Command<object> ListViewTappedCommand { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.ListViewTappedCommand'
        /// <summary>
        /// Gets the ListView holding command.
        /// </summary>
        /// <value>The ListView holding command.</value>
        public Command<object> ListViewHoldingCommand { get; private set; }
        /// <summary>
        /// Gets the search command.
        /// </summary>
        /// <value>The search command.</value>

        public PlantViewModel()
        {
            IsBusy = true;
            IsProductionsLineView = false;
            IsShopsView = false;
            if (DBManager.realm.All<Plant>().Any())
                Plant = DBManager.realm.All<Plant>().First();
            CheckLinesShops();
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            IsBusy = false;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.ListViewTappedCommandMethod(object)'
        public async void ListViewTappedCommandMethod(object obj)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.ListViewTappedCommandMethod(object)'
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is ProductionLine)
                {

                    ProductionLine eq = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as ProductionLine;
                    if (eq.IsValid)
                    {
                        await Navigation.PushAsync(new ProductionLinePage(eq));
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.ProductionLine + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Shop)
                {
                    Shop eq = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Shop;
                    if (eq.IsValid)
                    {
                        await Navigation.PushAsync(new ShopPage(eq));
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Shop + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.ListViewHoldingCommandMethod(object)'
        public async void ListViewHoldingCommandMethod(object obj)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.ListViewHoldingCommandMethod(object)'
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(ProductionLine))
                {

                    ProductionLine eq = (ProductionLine)(obj as ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {
                        string action = await page.DisplayActionSheet(eq.Code + " " + eq.Code, Properties.Resources.Cancel, null, Properties.Resources.AddShop, Properties.Resources.Edit, Properties.Resources.Details, Properties.Resources.Remove);

                        if (action == Properties.Resources.Edit)
                        {
                            await Navigation.PushAsync(new ProductionLinePage(eq, true));
                        }
                        else if (action == Properties.Resources.Details)
                        {
                            await Navigation.PushAsync(new ProductionLinePage(eq));
                        }
                        else if (action == Properties.Resources.Remove)
                        {
                            var str = eq.Code;
                            Plant.ProductionLineRemove(eq);
                            CheckLinesShops();
                            await page.DisplayAlert(Properties.Resources.Info, str + Properties.Resources.Deleted, Properties.Resources.Ok);
                        }
                        else if (action == Properties.Resources.AddShop)
                        {
                            Shop NewShop = new Shop() { ShopName = "new shop" };
                            DBManager.realm.Write(() =>
                            {
                                eq.Shops.Add(NewShop);
                            });
                            CheckLinesShops();
                            await Navigation.PushAsync(new ShopPage(NewShop));
                        }

                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.ProductionLine + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Shop))
                {
                    Shop mtr = (Shop)(obj as ItemHoldingEventArgs).ItemData;
                    if (mtr.IsValid)
                    {
                        string action = await page.DisplayActionSheet(mtr.ShopName + " " + mtr.ShopDescription, Properties.Resources.Cancel, null, Properties.Resources.Edit, Properties.Resources.Details, Properties.Resources.Remove);

                        if (action == Properties.Resources.Edit)
                        {
                            await Navigation.PushAsync(new ShopPage(mtr, true));
                        }
                        else if (action == Properties.Resources.Details)
                        {
                            await Navigation.PushAsync(new ShopPage(mtr));
                        }

                        else if (action == Properties.Resources.Remove)
                        {

                            RemoveItem(mtr);
                            CheckLinesShops();
                            await page.DisplayAlert(Properties.Resources.Info, mtr.ShopName + Properties.Resources.Deleted, Properties.Resources.Ok);
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Shop + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.CheckLinesShops()'
        public void CheckLinesShops()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantViewModel.CheckLinesShops()'
        {
            if (Plant != null && DBManager.realm.All<ProductionLine>().Any())
            {
                IsProductionsLineView = true;
                ProductionLines = DBManager.realm.All<ProductionLine>().ToList();
                if (ProductionLines != null && DBManager.realm.All<Shop>().Any())
                {
                    IsShopsView = true;
                    Shops = DBManager.realm.All<Shop>().ToList();
                }
            }
        }
    }
}
