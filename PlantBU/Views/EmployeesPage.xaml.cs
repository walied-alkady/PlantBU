// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-08-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-08-2021
// ***********************************************************************
// <copyright file="EmployeesPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeesPage'
    public partial class EmployeesPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeesPage'
    {

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeesPage.EmployeesPage()'
        public EmployeesPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeesPage.EmployeesPage()'
        {
            InitializeComponent();
            (BindingContext as EmployeesViewModel).Navigation = Navigation;
            (BindingContext as EmployeesViewModel).page = this;

        }
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (listView.DataSource != null)
            {
                this.listView.DataSource.Filter = FilterEmployees;
                this.listView.DataSource.RefreshFilter();
            }
        }
        private bool FilterEmployees(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var contacts = obj as Employee;
            if (
                    (contacts.FirstName != null ? contacts.FirstName.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.LastName != null ? contacts.LastName.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (contacts.Title != null ? contacts.Title.ToLower().Contains(searchBar.Text.ToLower()) : false))
                return true;
            else
                return false;
        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is ToolbarItem)
                {
                        switch ((sender as ToolbarItem).StyleId)
                        {

                            case "Add":
                                Employee newEmp = new Employee()
                                {
                                    FirstName = "New Employee",
                                    LastName = ""
                                };
                                Plant pl;
                                if (DBManager.realm.All<Plant>().Any())
                                {
                                    pl = DBManager.realm.All<Plant>().First();
                                    pl.EmployeeAdd(newEmp);
                                }
                                else
                                    throw new Exception(Properties.Resources.Plant + Properties.Resources.NotFound);
                                (BindingContext as EmployeesViewModel).RefreshItems();
                                await Navigation.PushAsync(new EmployeePage(newEmp, true));
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