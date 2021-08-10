// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-12-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-08-2021
// ***********************************************************************
// <copyright file="EquipmentsViewModel.cs" company="PlantBU">
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
    /// <summary>
    /// Class EquipmentsViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    public class EquipmentsViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the equipments.
        /// </summary>
        /// <value>The equipments.</value>
        public List<Equipment> Equipments { get { return _Equipments; } set { _Equipments = value; OnPropertyChanged("Equipments"); } }
        /// <summary>
        /// The equipments
        /// </summary>
        List<Equipment> _Equipments;
        /// <summary>
        /// Gets the equipments count.
        /// </summary>
        /// <value>The equipments count.</value>
        public string EquipmentsCount
        {
            get
            {
                if (Equipments != null)
                    return "Total Equipments: " + Equipments.Count().ToString();
                else
                    return "";
            }
        }
        /// <summary>
        /// Gets or sets the picker selected.
        /// </summary>
        /// <value>The picker selected.</value>
        public string PickerSelected { get { return _PickerSelected; } set { _PickerSelected = value; OnPropertyChanged("PickerSelected"); } }
        /// <summary>
        /// The picker selected
        /// </summary>
        string _PickerSelected;
        /// <summary>
        /// Gets or sets the sflist.
        /// </summary>
        /// <value>The sflist.</value>
        public SfListView sflist { get { return _sflist; } set { _sflist = value; OnPropertyChanged("sflist"); } }
        /// <summary>
        /// The sflist
        /// </summary>
        SfListView _sflist;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentsViewModel.IsExpand'
        public bool IsExpand { get { return _IsExpand; } set { _IsExpand = value; OnPropertyChanged("IsExpand"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentsViewModel.IsExpand'
        /// <summary>
        /// The sflist
        /// </summary>
        bool _IsExpand;
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
        /// Gets the refresh command.
        /// </summary>
        /// <value>The refresh command.</value>
        public Command RefreshCommand { get; private set; }
        /// <summary>
        /// Gets the search command.
        /// </summary>
        /// <value>The search command.</value>
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentsViewModel"/> class.
        /// </summary>
        public EquipmentsViewModel()
        {
            IsBusy = true;
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            RefreshCommand = new Command(RefreshItems);
            RefreshItems();
            IsBusy = false;
        }
        /// <summary>
        /// ListViews the tapped command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewTappedCommandMethod(object obj)
        {
            try { 
            if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Equipment)
            {

                Equipment eq = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Equipment;
                if (eq.IsValid)
                {
                    await Navigation.PushAsync(new EquipmentPage(eq));
                }
                else
                    await page.DisplayAlert("PlantBU", Properties.Resources.Equipment+ Properties.Resources.NotAvailable, Properties.Resources.Ok);
            }
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
            try { 
            if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Equipment))
            {

                Equipment eq = (Equipment)(obj as ItemHoldingEventArgs).ItemData;
                if (eq.IsValid)
                {
                    string action = "";
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        action = await page.DisplayActionSheet(eq.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);
                    else
                        action = await page.DisplayActionSheet(eq.Code, Properties.Resources.Cancel, null, Properties.Resources.Details);

                    if (action == Properties.Resources.Edit)
                    {
                        await Navigation.PushAsync(new EquipmentPage(eq, true));
                    }
                    else if (action == Properties.Resources.Details)
                    {
                        await Navigation.PushAsync(new EquipmentPage(eq));
                    }
                    else if (action == Properties.Resources.Remove)
                    {
                        RemoveItem<Equipment>(eq);
                        (obj as ItemHoldingEventArgs).Handled = true;
                        await page.DisplayAlert(Properties.Resources.Info, eq.Code + Properties.Resources.Deleted,Properties.Resources.Ok);
                    }
                }
                else
                    await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Equipment + Properties.Resources.NotAvailable, Properties.Resources.Ok);
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
            if (Equipments != null)
                Equipments.Clear();
            Equipments = GetItems<Equipment>();
        }

    }
}
