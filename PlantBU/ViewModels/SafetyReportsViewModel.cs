// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-08-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-08-2021
// ***********************************************************************
// <copyright file="EmployeesViewModel.cs" company="PlantBU">
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
    /// Class EmployeesViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    class SafetyReportsViewModel : BaseViewModel
    {
        public SafetyReport SafetyReport { get { return _SafetyReport; } set { _SafetyReport = value; OnPropertyChanged("SafetyReport"); } }
        SafetyReport _SafetyReport;
        public List<SafetyReport> SafetyReports { get { return _SafetyReports; } set { _SafetyReports = value; OnPropertyChanged("SafetyReports"); } }
        List<SafetyReport> _SafetyReports;
        public string SafetyReportsCount
        {
            get
            {
                if (SafetyReports != null)
                    return "Total Safety Reports: " + SafetyReports.Count().ToString();
                else
                    return "";
            }
        }
        public Command<object> ListViewTappedCommand { get; private set; }
        public Command<object> ListViewHoldingCommand { get; private set; }
        public Command RefreshCommand { get; private set; }
        public Command SearchCommand => new Command<string>((string query) =>
        {
            SafetyReports = DBManager.realm.All<SafetyReport>().Where(x => x.ReportDetailsObservation.Contains(query) ||
           x.ReportDetailsArea.Contains(query) ||
           x.ReportDetailsLine.Contains(query)).ToList();
        });
        public SafetyReportsViewModel()
        {
            IsBusy = true;
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            _SafetyReport = new SafetyReport();
            SafetyReports = GetItems<SafetyReport>();
            IsBusy = false;
        }
        public async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is SafetyReport)
                {

                    SafetyReport eq = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as SafetyReport;
                    if (eq.IsValid)
                    {
                        await page.Navigation.PushAsync(new SafetyReportPage(eq)); 
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
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(SafetyReport))
                {

                    SafetyReport eq = (SafetyReport)(obj as ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {
                        string action = await page.DisplayActionSheet(Properties.Resources.Report, Properties.Resources.Cancel, null, Properties.Resources.Edit, Properties.Resources.Remove);

                        if (action == Properties.Resources.Remove)
                        {
                            RemoveItem<SafetyReport>(eq);
                            await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.SafetyReport + Properties.Resources.Deleted , Properties.Resources.Ok);
                            RefreshItems();
                        }

                        else
                             if (action == Properties.Resources.Edit)
                        {
                            await page.Navigation.PushAsync(new SafetyReportPage(eq,false,true));
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.SafetyReport + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

        public async void RefreshItems()
        {
            try
            {
                IsBusy = true;
                if (SafetyReports != null)
                    SafetyReports.Clear();
                SafetyReports = GetItems<SafetyReport>();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
    }
}
