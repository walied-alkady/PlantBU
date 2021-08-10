// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-08-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="SchedulePage.xaml.cs" company="PlantBU">
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
    /// Class SchedulePage.
    /// Implements the <see cref="Xamarin.Forms.ContentPage" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SchedulePage : ContentPage
    {
        /// <summary>
        /// Gets or sets the schedule.
        /// </summary>
        /// <value>The schedule.</value>
        Schedule schedule { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulePage"/> class.
        /// </summary>
        /// <param name="sch">The SCH.</param>
        /// <param name="editable">if set to <c>true</c> [editable].</param>
        public SchedulePage(Schedule sch, bool editable = false)
        {
            InitializeComponent();
            try
            {
                (BindingContext as ScheduleViewModel).page = this;
                (BindingContext as ScheduleViewModel).Navigation = Navigation;
                (BindingContext as ScheduleViewModel).Schedule = sch;
                SheduleItemAutoComplete.Text = sch?.ItemCode;
                SheduleItemDescriptionEditor.Text = sch?.ItemDescription;

                if (string.IsNullOrEmpty(sch.Area))
                {
                    Equipment eqt = DBManager.realm.All<Equipment>().Where(x => x.Code == sch.ItemCode.Substring(0, 7)).FirstOrDefault();
                    ShopcomboBox.Text = eqt?.Shop;
                }
                else
                    ShopcomboBox.Text = sch.Area;
                var assn = DBManager.realm.All<Employee>().Where(x => x.CompanyCode  == sch.AssigneeCompanyCode).FirstOrDefault();
                AssigneecomboBox.SelectedValue = assn;
                AssigneecomboBox.Text = string.IsNullOrEmpty(assn?.FullNameEN) ?"": assn.FullNameEN;
                RepairAutoComplete.Text = sch?.Repair;
                RepairDetailsAutoComplete.Text = sch?.Repairdetails;
                datefromPicker.Date = sch.DateScheduleFrom.LocalDateTime;
                dateToPicker.Date = sch.DateScheduleTo.LocalDateTime;
                datePicker.Date = sch.SetDate.LocalDateTime;
                ExtraDataEditor.Text = sch?.Notes;
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        
#pragma warning disable CS1572 // XML comment has a param tag for 'sender', but there is no parameter by that name
#pragma warning disable CS1572 // XML comment has a param tag for 'e', but there is no parameter by that name
/// <summary>
        /// Handles the Completed event of the Editor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected override bool OnBackButtonPressed()
#pragma warning restore CS1572 // XML comment has a param tag for 'e', but there is no parameter by that name
#pragma warning restore CS1572 // XML comment has a param tag for 'sender', but there is no parameter by that name
        {
            if (string.IsNullOrEmpty(SheduleItemAutoComplete.Text))
            {
                SheduleItemAutoComplete.Watermark = Properties.Resources.CodeMessage;
                SheduleItemAutoComplete.BorderColor = Color.Red;
                return true; 
            }
            if(!RegexLib.IsComponentCode(SheduleItemAutoComplete.Text))
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
                schedule = (BindingContext as ScheduleViewModel).Schedule;

                DBManager.realm.Write(() =>
                {
                     if (sender is Editor)
                        switch ((sender as Editor).StyleId)
                    {
                        case "ExtraDataEditor":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            schedule.Notes = e.NewTextValue;
                            break;
                        case "CostEditor":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            schedule.RepairCost = double.Parse(e.NewTextValue);
                            break;
                        case "SheduleItemDescriptionEditor":
                            schedule.ItemDescription = e.NewTextValue;
                            break;
                        case "RepairDetailsAutoComplete":
                            schedule.Repairdetails = e.NewTextValue;
                            break;
                        
                      
                        }
                    if (sender is SfAutoComplete)
                        switch ((sender as SfAutoComplete).StyleId)
                        {
                            
                            case "RepairAutoComplete":
                                schedule.Repair = (sender as SfAutoComplete).Text;
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
                schedule = (BindingContext as ScheduleViewModel).Schedule;

                DBManager.realm.Write(() =>
                {
                    switch ((sender as SfDatePicker).StyleId)
                    {
                        case "datefromPicker":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            schedule.DateScheduleFrom = e.NewDate.ToLocalTime();
                            break;
                        case "dateToPicker":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            schedule.DateScheduleTo = e.NewDate.ToLocalTime();
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void SetSpare_Clicked(object sender, EventArgs e)
        {
            try
            {
                schedule = (BindingContext as ScheduleViewModel).Schedule;

                if ((InventoryAutoComplete.SelectedItem as Spareitem) != null)
                    DBManager.realm.Write(() =>
                {
                    double qty;
                    schedule.SpareParts.Add(new ScheduleSparePart()
                    {
                        InventoryCode = (InventoryAutoComplete.SelectedItem as Spareitem).code,
                        Description1 = (InventoryAutoComplete.SelectedItem as Spareitem).description1,
                        Description2 = (InventoryAutoComplete.SelectedItem as Spareitem).description2,
                        QtyRequired = double.TryParse(QtyEntry.Text, out qty) ? qty : 0,
                        Value = DBManager.realm.All<Spare>().Where(x => x.Code == (InventoryAutoComplete.SelectedItem as Spareitem).code)
                        .FirstOrDefault().Value
                    });
                });
                else
                    await DisplayAlert(Properties.Resources.Error, "Select Item!", Properties.Resources.Ok);
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
                    if (((sender as SfComboBox).SelectedValue as Employee) != null)
                        schedule.AssigneeCompanyCode = ((sender as SfComboBox).SelectedValue as Employee)?.CompanyCode ;
                    if (sender is SfComboBox)
                        switch ((sender as SfComboBox).StyleId)
                        {

                            case "ShopcomboBox":
                                schedule.Area = (sender as SfComboBox).Text;
                                break;
                        }
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
                schedule = (BindingContext as ScheduleViewModel).Schedule;
                DBManager.realm.Write(() =>
                {
                    switch ((sender as SfAutoComplete).StyleId)
                    {
                        case "SheduleItemAutoComplete":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            schedule.ItemCode = (e.Value as ScheduleItems)?.Code;
                            var temp = DBManager.realm.All<Motor>().Where(x => x.Code == (sender as SfAutoComplete).Text).FirstOrDefault();
                            if (temp != null)
                                schedule.ItemDescription = temp?.Description;

                            if (string.IsNullOrEmpty(schedule.Area))
                            {
                                Equipment eqt = DBManager.realm.All<Equipment>().Where(x => x.Code == schedule.ItemCode.Substring(0, 7)).FirstOrDefault();
                                ShopcomboBox.Text = eqt?.Shop;
                            }
                            else
                                ShopcomboBox.Text = schedule?.Area;
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
                schedule = (BindingContext as ScheduleViewModel).Schedule;

                DBManager.realm.Write(() =>
                {
                   
                    if (sender is SfAutoComplete)
                        switch ((sender as SfAutoComplete).StyleId)
                        {

                            case "RepairAutoComplete":
                                schedule.Repair = (sender as SfAutoComplete).Text;
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