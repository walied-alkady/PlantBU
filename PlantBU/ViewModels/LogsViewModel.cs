// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-30-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="LogsViewModel.cs" company="PlantBU">
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
    /// Class LogsViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    public class LogsViewModel : BaseViewModel
    {
        //private readonly Realm _realm;
        /// <summary>
        /// The logs
        /// </summary>
        private List<Log> _Logs;
        /// <summary>
        /// Gets or sets the logs.
        /// </summary>
        /// <value>The logs.</value>
        public List<Log> Logs
        {
            get { return _Logs; }
            set
            {
                _Logs = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// The log
        /// </summary>
        private Log _Log;
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>The log.</value>
        public Log Log
        {
            get { return _Log; }
            set { SetProperty(ref _Log, value); }
        }
        /// <summary>
        /// Gets the logs count.
        /// </summary>
        /// <value>The logs count.</value>
        public string LogsCount
        {
            get
            {
                if (_Logs != null)
                    return "Total Logs: " + _Logs.Count().ToString();
                else
                    return "";
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'LogsViewModel.ListViewTappedCommand'
        public Command<object> ListViewTappedCommand { get; private set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'LogsViewModel.ListViewTappedCommand'

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
        public Command SearchCommand => new Command<string>((string query) =>
        {
            Logs = DBManager.realm.All<Log>().Where(x => x.ItemCode.Contains(query) ||
           x.ItemDescription.Contains(query) ||
           x.Repair.Contains(query) ||
           x.Repairdetails.Contains(query) ||
           x.DateLog.ToString().Contains(query)).ToList();

        });

        /// <summary>
        /// Initializes a new instance of the <see cref="LogsViewModel"/> class.
        /// </summary>
        public LogsViewModel()
        {
            IsBusy = true;
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            _Log = new Log();
            Logs = GetItems<Log>();
            IsBusy = false;
        }
        /// <summary>
        /// Refreshes the items.
        /// </summary>
        public async void RefreshItems()
        {
            try
            {
                if (Logs != null)
                    Logs.Clear();
                Logs = GetItems<Log>();
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'LogsViewModel.ListViewTappedCommandMethod(object)'
        public async void ListViewTappedCommandMethod(object obj)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'LogsViewModel.ListViewTappedCommandMethod(object)'
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Log)
                {

                    Log eq = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Log;
                    if (eq.IsValid)
                    {
                        await Navigation.PushAsync(new LogPage(eq));
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Log, Properties.Resources.Ok);
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
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Log))
                {

                    Log eq = (Log)(obj as ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {

                        string action = "";
                        if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin || 
                            (eq.AssigneeCompanyCode == DBManager.CurrentUser.CompanyCode))
                            action = await page.DisplayActionSheet(eq.ItemCode, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.CopyToSchedules, Properties.Resources.Remove);
                        else
                            action = await page.DisplayActionSheet(eq.ItemCode, Properties.Resources.Cancel, null, Properties.Resources.Details);

                        if (action == Properties.Resources.Details)
                        {
                            await Navigation.PushAsync(new LogPage(eq));
                        }

                        else if (action == Properties.Resources.Remove)
                        {
                            DBManager.realm.Write(() =>
                            {
                                DBManager.realm.Remove(eq);
                            });
                            (obj as ItemHoldingEventArgs).Handled = true;
                            await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Log + Properties.Resources.Deleted, Properties.Resources.Ok);
                        }
                        else if (action == Properties.Resources.Edit)
                        {
                            await Navigation.PushAsync(new LogPage(eq, true));
                        }
                        else if (action == Properties.Resources.CopyToSchedules)
                        {
                            Schedule newEq = new Schedule()
                            {
                                ItemCode = eq.ItemCode,
                                ItemDescription = eq.ItemDescription,
                                Repair = eq.Repair,
                                Repairdetails = eq.Repairdetails, 
                                DateScheduleFrom = DateTime.Now.Date.ToLocalTime(),
                                DateScheduleTo = DateTime.Now.Date.AddDays(1).ToLocalTime(),
                                SetDate = DateTime.Now.Date.ToLocalTime(),
                                Notes = eq.Notes,
                                AssigneeCompanyCode = eq.AssigneeCompanyCode,
                            };
                            Plant pl = DBManager.realm.All<Plant>().First();
                            pl.ScheduleAdd(newEq);
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Log + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        /// <summary>
        /// Handles the TextChanged event of the SearchBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Logs = GetItems<Log>(e.NewTextValue);
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

    }
}
