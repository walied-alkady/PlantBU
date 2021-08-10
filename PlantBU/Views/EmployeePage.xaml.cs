// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-08-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-08-2021
// ***********************************************************************
// <copyright file="EmployeePage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using Syncfusion.SfDataGrid.XForms.Renderers;
using Syncfusion.XForms.ComboBox;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    /// <summary>
    /// Class EmployeePage.
    /// Implements the <see cref="Xamarin.Forms.ContentPage" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeePage : ContentPage
    {
        /// <summary>
        /// Gets or sets the employee.
        /// </summary>
        /// <value>The employee.</value>
        Employee employee { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePage"/> class.
        /// </summary>
        /// <param name="emp">The emp.</param>
        /// <param name="IsEnabled">if set to <c>true</c> [is enabled].</param>
        public EmployeePage(Employee emp, bool IsEnabled = false)
        {
            InitializeComponent();
            try
            {
                employee = emp;
                CodeEditor.Text = employee.CompanyCode;
                FirstNameEditor.Text = employee.FirstName;
                LastNameEditor.Text = employee.LastName;
                FullNameEnEditor.Text = employee.FullNameEN;
                FullNameArEditor.Text = employee.FullNameAr;
                BirthDatePicker.Date = employee.BirthDate.DateTime;
                comboBox.Text = employee.Title;
                UserNameEditor.Text = employee.UserName;
                EmailEditor.Text = employee.Email;
                PassWordEditor.Text = employee.Password;
            }
            catch (Exception ex)
            {
                DisplayAlert("PlantBU", ex.Message, Properties.Resources.Ok);
            }
        }
        private void Editor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    if (sender is Editor)
                    {
                        switch ((sender as Editor).StyleId)
                        {
                            case "CodeEditor":
                                employee.CompanyCode = (sender as Editor).Text;
                                break;
                            case "FirstNameEditor":
                                employee.FirstName = (sender as Editor).Text;
                                break;
                            case "LastNameEditor":
                                employee.LastName = (sender as Editor).Text;
                                break;
                            case "FullNameEnEditor":
                                employee.FullNameEN = (sender as Editor).Text;
                                break;
                            case "FullNameArEditor":
                                employee.FullNameAr = (sender as Editor).Text;
                                break;
                            case "UserNameEditor":
                                employee.UserName = (sender as Editor).Text;
                                break;
                            case "EmailEditor":
                                employee.Email = (sender as Editor).Text;
                                break;
                            case "PassWordEditor":
                                employee.Password = (sender as Editor).Text;

                                break;
                        }
                    }
                    else if (sender is SfComboBox)
                    {
                        switch ((sender as SfComboBox).StyleId)
                        {
                            case "comboBox":
                                employee.Title = (sender as SfComboBox).Text;
                                break;

                        }
                    }
                   /* else if (sender is SfDatePicker)
                        switch ((sender as SfDatePicker).StyleId)
                        {
                            case "BirthDatePicker":
                                employee.BirthDate = (sender as SfDatePicker).Date;
                                break;

                        }*/
                });
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void datefromPicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            try
            {
                DBManager.realm.Write(() =>
                {
                    switch ((sender as SfDatePicker).StyleId)
                    {
                        case "BirthDatePicker":
                            //var sch = DBManager.realm.All<Schedule>().Where(x => x.Id == schedule.Id).First();
                            employee.BirthDate = e.NewDate;
                            break;
                    }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is ToolbarItem)
                {

                    switch ((sender as ToolbarItem).StyleId)
                    {

                        case "SafetyReports":
                            await Navigation.PushAsync(new SafetyReportAnalysisPage(employee));
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