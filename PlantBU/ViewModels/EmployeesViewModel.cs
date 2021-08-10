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
    class EmployeesViewModel : BaseViewModel
    {


        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        /// <value>The employees.</value>
        public List<Employee> Employees { get { return _Employees; } set { _Employees = value; OnPropertyChanged("Employees"); } }
        /// <summary>
        /// The employees
        /// </summary>
        List<Employee> _Employees;
        /// <summary>
        /// Gets the schedules count.
        /// </summary>
        /// <value>The schedules count.</value>
        public string SchedulesCount
        {
            get
            {
                if (Employees != null)
                    return "Total Employees: " + Employees.Count().ToString();
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
        /// <summary>
        /// Gets the add employee command.
        /// </summary>
        /// <value>The add employee command.</value>
        public Command AddEmployeeCommand { get; private set; }
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
        /// Initializes a new instance of the <see cref="EmployeesViewModel"/> class.
        /// </summary>
        public EmployeesViewModel()
        {
            IsBusy = true;
            AddEmployeeCommand = new Command(AddEmployeeCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            RefreshCommand = new Command(RefreshItems);
            RefreshItems();
            //RefreshItems();
            IsBusy = false;
        }
        /// <summary>
        /// Adds the employee command method.
        /// </summary>
        public void AddEmployeeCommandMethod()
        {
            Employee newemp = new Employee() { FirstName = "New Employee" };

        }
        /// <summary>
        /// ListViews the holding command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewHoldingCommandMethod(object obj)
        {

            if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Employee))
            {

                Employee eq = (Employee)(obj as ItemHoldingEventArgs).ItemData;
                if (eq.IsValid)
                {
                    string action = await page.DisplayActionSheet(eq.FirstName + " " + eq.LastName, Properties.Resources.Cancel, null, Properties.Resources.Edit, Properties.Resources.Remove);

                    if (action == Properties.Resources.Remove)
                    {
                        RemoveItem<Employee>(eq);
                        (obj as ItemHoldingEventArgs).Handled = true;
                        await page.DisplayAlert(Properties.Resources.Info, eq.FirstName + " " + eq.LastName + Properties.Resources.Deleted, Properties.Resources.Ok);
                        Employees.Remove(eq);
                        RefreshItems();
                    }

                    else
                         if (action == Properties.Resources.Edit)
                    {
                        await Navigation.PushAsync(new EmployeePage(eq, true));
                    }
                }
                else
                    await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Employee + Properties.Resources.NotAvailable, Properties.Resources.Ok);
            }
        }
        /// <summary>
        /// Refreshes the items.
        /// </summary>
        public void RefreshItems()
        {
            IsBusy = true;
            if (Employees != null)
                Employees.Clear();
            Employees = GetItems<Employee>();
            IsBusy = false;
        }
    }
}
