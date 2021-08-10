// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-07-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="SchedulesViewModel.cs" company="PlantBU">
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
    /// Class SchedulesViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    class SchedulesViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the schedules.
        /// </summary>
        /// <value>The schedules.</value>
        public List<Schedule> Schedules { get { return _Schedules; } set { _Schedules = value; OnPropertyChanged("Schedules"); } }
        /// <summary>
        /// The schedules
        /// </summary>
        List<Schedule> _Schedules;
        /// <summary>
        /// Gets the schedules count.
        /// </summary>
        /// <value>The schedules count.</value>
        public string SchedulesCount
        {
            get
            {
                if (Schedules != null)
                    return Schedules.Count().ToString();
                else
                    return "";
            }
        }
        public string SchedulesPercent
        {
            get
            {
                if (Schedules != null)
                {
                    double xvalue = DBManager.realm.All<Schedule>().Count(x=> x.StatusSchedule == true);
                    double xvalue1 = DBManager.realm.All<Schedule>().Count();
                    if (xvalue1 != 0)
                    {
                        int result = System.Convert.ToInt32(xvalue / xvalue1 * 100);
                        return result.ToString();
                    }
                    else
                        return "";
                }
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets or sets the sflist.
        /// </summary>
        /// <value>The sflist.</value>
        public SfListView sflist { get { return _sflist; } set { _sflist = value; OnPropertyChanged("sflist"); } }
        /// <summary>
        /// The sflist
        /// </summary>
        SfListView _sflist;
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
       
        public SchedulesViewModel()
        {
            IsBusy = true;
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            RefreshCommand = new Command(RefreshItems);
            Schedules = DBManager.realm.All<Schedule>().ToList();
            IsBusy = false;
        }
        public async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Schedule)
                {

                    Schedule eq = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Schedule;
                    if (eq.IsValid)
                    {
                        await Navigation.PushAsync(new SchedulePage(eq));
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Schedule + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Schedule))
                {

                    Schedule eq = (Schedule)(obj as ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {
                        string action = "";
                        if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin || eq.AssigneeCompanyCode == DBManager.CurrentUser.CompanyCode)
                            action = await page.DisplayActionSheet(eq.ItemCode, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);
                        else
                            action = await page.DisplayActionSheet(eq.ItemCode, Properties.Resources.Cancel, null, Properties.Resources.Details);

                        if (action == Properties.Resources.Details)
                        {
                            await Navigation.PushAsync(new SchedulePage(eq));
                        }
                        else if (action == Properties.Resources.Remove)
                        {
                           
                            DBManager.realm.Write(() =>
                            {
                                DBManager.realm.Remove(eq);
                            });
                            await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Schedule + Properties.Resources.Deleted, Properties.Resources.Ok);
                        }
                        else if (action == Properties.Resources.Edit)
                        {
                            await Navigation.PushAsync(new SchedulePage(eq, true));
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Schedule + Properties.Resources.NotAvailable, Properties.Resources.Ok);
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
        public async void RefreshItems()
        {
            try
            {
                IsBusy = true;
                if (Schedules != null)
                    Schedules.Clear();
                Schedules = GetItems<Schedule>();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

    }
}
