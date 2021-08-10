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
    class ExpensesViewModel : BaseViewModel
    {


        /// <summary>
        /// Gets or sets the employees.
        /// </summary>
        /// <value>The employees.</value>
        public List<ExpensItem> Expenses { get { return _Expenses; } set { _Expenses = value; OnPropertyChanged("Expenses"); } }
        List<ExpensItem> _Expenses;
        /// <summary>
        /// Gets the schedules count.
        /// </summary>
        /// <value>The schedules count.</value>
        public string ExpensesCount
        {
            get
            {
                if (Expenses != null)
                    return "Total Expenses: " + Expenses.Count().ToString();
                else
                    return "";
            }
        }
        public double ExpensesSum { get { return _ExpensesSum; } set { _ExpensesSum = value; OnPropertyChanged("ExpensesSum"); } }
        double _ExpensesSum;
        public double BudgetSum { get { return _BudgetSum; } set { _BudgetSum = value; OnPropertyChanged("BudgetSum"); } }
        double _BudgetSum;
        public double BarPercent { get { return _BarPercent; } set { _BarPercent = value; OnPropertyChanged("BarPercent"); } }
        double _BarPercent;
        public bool OverBudget { get { return _OverBudget; } set { _OverBudget = value; OnPropertyChanged("OverBudget"); } }
        bool _OverBudget;
        public bool IsbarVisible { get { return _IsbarVisible; } set { _IsbarVisible = value; OnPropertyChanged("IsbarVisible"); } }
        bool _IsbarVisible;
        public bool IsbarTextVisible { get { return _IsbarTextVisible; } set { _IsbarTextVisible = value; OnPropertyChanged("IsbarTextVisible"); } }
        bool _IsbarTextVisible;
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
        public ExpensesViewModel()
        {
            IsBusy = true;
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            RefreshCommand = new Command(RefreshItems);
            RefreshItems();
            //RefreshItems();
            IsBusy = false;
            ExpensesSum = Expenses.Sum(x => x.Value);
            BudgetSum = DBManager.realm.All<Budgetitem>().ToList().Sum(x => x.Value);
            if (BudgetSum > 0)
            {
                IsbarVisible = true;
                if (ExpensesSum < BudgetSum)
                {
                    OverBudget = true;
                    BarPercent = ExpensesSum / BudgetSum * 100;
                }
                else
                {
                    OverBudget = false;
                    BarPercent = 100;
                }

            }
            else
            {
                IsbarTextVisible = true; IsbarVisible = false;
            }
        }
        /// <summary>
        /// Adds the employee command method.
        /// </summary>

        /// <summary>
        /// ListViews the holding command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(ExpensItem)
                    && DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                {

                    ExpensItem eq = (ExpensItem)(obj as ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {
                        string action = await page.DisplayActionSheet(eq.Description,Properties.Resources.Cancel, null, Properties.Resources.Edit, Properties.Resources.Remove);

                        if (action == Properties.Resources.Remove)
                        {
                            RemoveItem<ExpensItem>(eq);
                            (obj as ItemHoldingEventArgs).Handled = true;
                            await page.DisplayAlert(Properties.Resources.Info, eq.Description + Properties.Resources.Expense + Properties.Resources.Deleted , Properties.Resources.Ok);
                            Expenses.Remove(eq);
                            RefreshItems();
                        }

                        else
                             if (action == Properties.Resources.Edit)
                        {
                            await Navigation.PushAsync(new ExpensePage(eq, true));
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Expense + Properties.Resources.NotAvailable, Properties.Resources.Ok);
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
                if (Expenses != null)
                    Expenses.Clear();
                Expenses = GetItems<ExpensItem>();
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
    }
}
