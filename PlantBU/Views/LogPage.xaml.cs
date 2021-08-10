// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-09-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="LogPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.Utilities;
using PlantBU.ViewModels;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.SfDataGrid.XForms.Renderers;
using Syncfusion.XForms.ComboBox;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    /// <summary>
    /// Class LogPage.
    /// Implements the <see cref="Xamarin.Forms.ContentPage" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogPage : ContentPage
    {
        Log log { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'LogPage.LogPage(Log, bool)'
        public LogPage(Log sch, bool editable = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'LogPage.LogPage(Log, bool)'
        {
            InitializeComponent();
            try
            {
                log = sch;
                (BindingContext as LogViewModel).Log = sch;
                (BindingContext as LogViewModel).page = this;
                (BindingContext as LogViewModel).Navigation = Navigation;
                SheduleItemAutoComplete.Text = log.ItemCode;
                SheduleItemDescriptionEditor.Text = log.ItemDescription;
                //SheduleAssigneeAutoComplete.Text = log.AssigneeFirstName + " " + log.AssigneeLastName;
                RepairAutoComplete.Text = log.Repair;
                RepairDetailsEditor.Text = log.Repairdetails;
                datefromPicker.Date = log.DateLog.DateTime;
                ExtraDataEditor.Text = log.Notes;
                var assn = DBManager.realm.All<Employee>().Where(x => x.CompanyCode == sch.AssigneeCompanyCode).FirstOrDefault();
                AssigneecomboBox.SelectedValue = assn;
                AssigneecomboBox.Text = string.IsNullOrEmpty(assn?.FullNameEN) ? "" : assn.FullNameEN;
                ExtraDataEditor.Text = sch?.Notes;
                CostEditor.Text = sch?.Cost.ToString();
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'LogPage.OnBackButtonPressed()'
        protected override bool OnBackButtonPressed()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'LogPage.OnBackButtonPressed()'
        {
            if (string.IsNullOrEmpty(SheduleItemAutoComplete.Text))
            {
                SheduleItemAutoComplete.Watermark = Properties.Resources.CodeMessage;
                SheduleItemAutoComplete.BorderColor = Color.Red;
                return true;
            }
            if (!RegexLib.IsComponentCode(SheduleItemAutoComplete.Text))
            {
                SheduleItemAutoComplete.BorderColor = Color.Red;
                return true;
            }
            return base.OnBackButtonPressed();

        }
        private async void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                log = (BindingContext as LogViewModel).Log;

                DBManager.realm.Write(() =>
                {
                    if (sender is Editor)
                        switch ((sender as Editor).StyleId)
                        {
                            case "ExtraDataEditor":
                                //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                                log.Notes = e.NewTextValue;
                                break;
                            case "CostEditor":
                                //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                                log.Cost = double.Parse(e.NewTextValue);
                                break;
                            case "SheduleItemDescriptionEditor":
                                log.ItemDescription = e.NewTextValue;
                                break;
                            
                        }
                    if (sender is SfAutoComplete)
                        switch ((sender as SfAutoComplete).StyleId)
                        {
                            case "RepairAutoComplete":
                                log.Repair = (sender as SfAutoComplete).Text;
                                break;
                            case "RepairDetailsAutoComplete":
                                log.Repairdetails = e.NewTextValue;
                                break;
                        }

                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void datefromPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            try
            {
                log = (BindingContext as LogViewModel).Log;

                DBManager.realm.Write(() =>
                {
                    switch ((sender as SfDatePicker).StyleId)
                    {
                        case "datefromPicker":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            log.DateLog = e.NewDate.ToLocalTime();
                            break;
                       
                    }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void ComboBox_SelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    if (((sender as SfComboBox).SelectedValue as Employee)!=null)
                        log.AssigneeCompanyCode = ((sender as SfComboBox).SelectedValue as Employee)?.CompanyCode;
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void AutoComplete_SelectionChanged(object sender, Syncfusion.SfAutoComplete.XForms.SelectionChangedEventArgs e)
        {
            try
            {
                log = (BindingContext as LogViewModel).Log;
                DBManager.realm.Write(() =>
                {
                    switch ((sender as SfAutoComplete).StyleId)
                    {
                        case "SheduleItemAutoComplete":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            log.ItemCode = (e.Value as ScheduleItems)?.Code;
                            var temp = DBManager.realm.All<Motor>().Where(x => x.Code == (sender as SfAutoComplete).Text).FirstOrDefault();
                            if (temp != null)
                                log.ItemDescription = temp?.Description;                            
                            break;
                    }
                });

            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

        private async void RepairAutoComplete_ValueChanged(object sender, Syncfusion.SfAutoComplete.XForms.ValueChangedEventArgs e)
        {
            try
            {
                log = (BindingContext as LogViewModel).Log;

                DBManager.realm.Write(() =>
                {

                    if (sender is SfAutoComplete)
                        switch ((sender as SfAutoComplete).StyleId)
                        {

                            case "RepairAutoComplete":
                                log.Repair = (sender as SfAutoComplete).Text;
                                break;
                        }

                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
    }
}