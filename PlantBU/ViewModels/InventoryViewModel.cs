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
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class InventoryViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    public class InventoryViewModel : BaseViewModel
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoryViewModel.Inventory'
        public Inventory Inventory { get { return _Inventory; } set { _Inventory = value; OnPropertyChanged("Inventory"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoryViewModel.Inventory'
        /// <summary>
        /// The spares
        /// </summary>
        private Inventory _Inventory;
        /// <summary>
        /// Gets or sets the spares.
        /// </summary>
        /// <value>The spares.</value>
        public List<Spare> Spares { get { return _Spares; } set { _Spares = value; OnPropertyChanged("Spares"); } }
        private List<Spare> _Spares;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoryViewModel.Componenets'
        public List<string> Componenets { get { return _Componenets; } set { _Componenets = value; OnPropertyChanged("Componenets"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoryViewModel.Componenets'
        /// <summary>
        /// The componenets
        /// </summary>
        private List<string> _Componenets;
        /// <summary>
        /// Gets or sets the selected component.
        /// </summary>
        /// <value>The selected component.</value>
        public string SelectedComponent { get { return _SelectedComponent; } set { _SelectedComponent = value; OnPropertyChanged("SelectedComponent"); } }
        /// <summary>
        /// The selected component
        /// </summary>
        private string _SelectedComponent;
        /// <summary>
        /// Gets the spares count.
        /// </summary>
        /// <value>The spares count.</value>
        public string SparesCount
        {
            get
            {
                if (Spares != null)
                    return "Total Spares: " + Spares.Count().ToString();
                else
                    return "";
            }
        }
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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoryViewModel.LoadMoreItemsCommand'
        public Command<object> LoadMoreItemsCommand { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoryViewModel.LoadMoreItemsCommand'
        /// <summary>
        /// Gets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public ICommand SearchCommand => new Command<string>((string query) =>
            {
                if (!string.IsNullOrEmpty(query) && query.Length > 0)
                {
                    Spares = Inventory.Spares.Where(p => p.Code.Contains(query) ||
                                                            p.Description1.Contains(query) ||
                                                            p.Description2.Contains(query)).ToList();
                }
                else
                    Spares = Inventory.Spares.OrderBy(u => u.Code).ToList();
            });
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryViewModel"/> class.
        /// </summary>
        int index;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'InventoryViewModel.InventoryViewModel()'
        public InventoryViewModel()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'InventoryViewModel.InventoryViewModel()'
        {
            IsBusy = true;
            if (DBManager.IsInventoryLoaded)
            {
                ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
                ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
                LoadMoreItemsCommand = new Command<object>(LoadMoreItems, CanLoadMoreItems);
               // AddProducts(0, 10);//Spares = GetItems<Spare>().ToList(); 
                if (Componenets == null)
                    Componenets = new List<string>();
                Componenets.AddRange(DBManager.realm.All<Motor>().ToList().Select(x => x.Code).AsEnumerable());

            }
            IsBusy = false;
        }

        /// <summary>
        /// ListViews the tapped command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewTappedCommandMethod(object obj)
        {
            IsBusy = true;
            if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Spare)
            {

                Spare spr = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Spare;
                if (spr.IsValid)
                {
                    await Navigation.PushAsync(new SparePage(spr));
                }
                else
                    await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Spare + Properties.Resources.NotAvailable, Properties.Resources.Ok);
            }
            IsBusy = false;
        }
        /// <summary>
        /// ListViews the holding command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewHoldingCommandMethod(object obj)
        {

            if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Spare)
                && DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
            {


                Spare spr = (Spare)(obj as ItemHoldingEventArgs).ItemData;
                if (spr.IsValid)
                {
                    string action = await page.DisplayActionSheet(spr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);

                    if (action == Properties.Resources.Edit)
                    {
                        await Navigation.PushAsync(new SparePage(spr, true));
                    }
                    else if (action == Properties.Resources.Details)
                    {
                        await Navigation.PushAsync(new SparePage(spr));
                    }
                    else if (action == Properties.Resources.Remove)
                    {
                       
                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(spr);
                        });
                            await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Deleted, Properties.Resources.Ok);
                    }
                }
                else
                    await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Spare+ Properties.Resources.NotAvailable, Properties.Resources.Ok);
            }
        }
        /// <summary>
        /// Refreshes the items.
        /// </summary>
        public void RefreshItems()
        {
            IsBusy = true;
            if (Spares != null)
                Spares.Clear();
            Spares = Inventory.Spares.OrderBy(u => u.Code).ToList();
            IsBusy = false;
        }
        private bool CanLoadMoreItems(object obj)
        {

            if (Spares.Count >= Inventory.Spares.Count)
                return false;
            return true;

        }
        private async void LoadMoreItems(object obj)
        {
            var listView = obj as SfListView;
            try
            {
                listView.IsBusy = true;
                await Task.Delay(500);
                index = index + 10;
                var count = index + 10 >= Inventory.Spares.Count ? Inventory.Spares.Count - index : 10;
                AddProducts(index, count);
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
            finally
            {
                listView.IsBusy = false;
            }
        }
        internal async void AddProducts(int index, int count)
        {
            try
            {
                int SparesCountloc;
                if (Spares == null)
                 SparesCountloc = 0;                
                else
                    SparesCountloc = Spares.Count;
                Spares = Inventory.Spares.OrderBy(u => u.Code).ToList().Skip((index - 1) * count)
                     .Take(count).ToList();
                if (Componenets == null)
                    Componenets = new List<string>();
                Componenets.AddRange(DBManager.realm.All<Motor>().ToList().Select(x => x.Code).AsEnumerable());
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
    }
}
