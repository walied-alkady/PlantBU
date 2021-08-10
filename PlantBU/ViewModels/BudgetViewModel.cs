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

    class BudgetViewModel : BaseViewModel
    {



        public List<Budgetitem> Budgetitems { get { return _Budgetitems; } set { _Budgetitems = value; OnPropertyChanged("Budgetitems"); } }
        List<Budgetitem> _Budgetitems;

        public string ExpensesCount
        {
            get
            {
                if (Budgetitems != null)
                    return "Total Budget Items: " + Budgetitems.Count().ToString();
                else
                    return "";
            }
        }

        /// <summary>
        /// Gets the ListView holding command.
        /// </summary>
        /// <value>The ListView holding command.</value>
        public Command<object> ListViewHoldingCommand { get; private set; }
        /// <summary>
        /// Gets the refresh command.
        /// </summary>
        /// <value>The refresh command.</value>
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeesViewModel"/> class.
        /// </summary>
        public BudgetViewModel()
        {
            IsBusy = true;
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            Budgetitems = GetItems<Budgetitem>();
            IsBusy = false;
        }

        public async void ListViewHoldingCommandMethod(object obj)
        {

            if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Budgetitem)
                && DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
            {

                Budgetitem eq = (Budgetitem)(obj as ItemHoldingEventArgs).ItemData;
                if (eq.IsValid)
                {
                    string action = await page.DisplayActionSheet(eq.Description, Properties.Resources.Cancel, null, Properties.Resources.Edit, Properties.Resources.Remove);

                    if (action == Properties.Resources.Remove)
                    {
                        RemoveItem<Budgetitem>(eq);

                        await page.DisplayAlert(Properties.Resources.Info, eq.Description + Properties.Resources.Deleted, Properties.Resources.Ok);
                    }

                    else
                         if (action == Properties.Resources.Edit)
                    {
                        await Navigation.PushAsync(new BudgetitemPage(eq, true));
                    }
                }
                else
                    await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.BudgetItem + Properties.Resources.NotAvailable, Properties.Resources.Ok);
            }
        }

    }
}
