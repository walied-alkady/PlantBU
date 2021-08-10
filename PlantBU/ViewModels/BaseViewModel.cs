// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-12-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-08-2021
// ***********************************************************************
// <copyright file="BaseViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.Views;
using Realms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class BaseViewModel.
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>The page.</value>
        public Page page { get { return _page; } set { _page = value; OnPropertyChanged("page"); } }
        /// <summary>
        /// The page
        /// </summary>
        Page _page;
        /// <summary>
        /// Gets or sets the navigation.
        /// </summary>
        /// <value>The navigation.</value>
        public INavigation Navigation { get { return _Navigation; } set { _Navigation = value; OnPropertyChanged("Navigation"); } }
        /// <summary>
        /// The navigation
        /// </summary>
        INavigation _Navigation;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy { get { return _IsBusy; } set { _IsBusy = value; OnPropertyChanged("IsBusy"); } }
        /// <summary>
        /// The is busy
        /// </summary>
        bool _IsBusy;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.IsBusyTitle'
        public string IsBusyTitle { get { return _IsBusyTitle; } set { _IsBusyTitle = value; OnPropertyChanged("IsBusyTitle"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.IsBusyTitle'
        /// <summary>
        /// The is busy
        /// </summary>
        string _IsBusyTitle;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled { get { return _IsEnabled; } set { _IsEnabled = value; OnPropertyChanged("IsEnabled"); } }
        /// <summary>
        /// The is enabled
        /// </summary>
        bool _IsEnabled;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.LoggedUser'
        public string LoggedUser { get { return _LoggedUser; } set { _LoggedUser = value; OnPropertyChanged("LoggedUser"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.LoggedUser'
        string _LoggedUser;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.IsAdmin'
        public bool IsAdmin { get { return _IsAdmin; } set { _IsAdmin = value; OnPropertyChanged("IsAdmin"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.IsAdmin'
      
        bool _IsAdmin;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.SessionState'
        public string SessionState { get { return _SessionState; } set { _SessionState = value; OnPropertyChanged("SessionState"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.SessionState'

        string _SessionState;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.IsNotConnected'
        public bool IsNotConnected { get { return _IsNotConnected; } set { _IsNotConnected = value; OnPropertyChanged("IsNotConnected"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.IsNotConnected'

        bool _IsNotConnected;

        /// <summary>
        /// Gets or sets the line list.
        /// </summary>
        /// <value>The line list.</value>
        public List<ProductionLine> LineList { get; set; }
        /// <summary>
        /// Gets or sets the shop list.
        /// </summary>
        /// <value>The shop list.</value>
        public List<Shop> ShopList { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        /// <summary>
        /// Gets or sets the inventory list.
        /// </summary>
        /// <value>The inventory list.</value>
        public List<Spareitem> InventoryList { get { return _InventoryList; } set { _InventoryList = value; OnPropertyChanged("InventoryList"); } }
        /// <summary>
        /// The inventory list
        /// </summary>
        List<Spareitem> _InventoryList;
        /// <summary>
        /// Gets or sets the inventory selected item.
        /// </summary>
        /// <value>The inventory selected item.</value>
        public Spareitem InventorySelectedItem { get { return _InventorySelectedItem; } set { _InventorySelectedItem = value; OnPropertyChanged("InventorySelectedItem"); } }
        /// <summary>
        /// The inventory selected item
        /// </summary>
        Spareitem _InventorySelectedItem;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.BaseViewModel()'
        public BaseViewModel()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.BaseViewModel()'
        {
            IsBusyTitle = Properties.Resources.Loading;
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                IsAdmin = true;
            else
                IsAdmin = false;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.~BaseViewModel()'
        ~BaseViewModel()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.~BaseViewModel()'
        {
            Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
        }
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public async void AddItem<T>()
        {
            var xx = typeof(T).Name;
            switch (xx)
            {
                case "Equipment":
                    Equipment eq = new Equipment()
                    {
                        Code = "New Equipment",
                        Description = ""
                    };
                    DBManager.AddItem(eq);
                    await Navigation.PushAsync(new EquipmentPage(eq, true));
                    break;
                case "Motor":
                    Motor mtr = new Motor()
                    {
                        Code = "New Motor",
                        Description = ""
                    };
                    DBManager.AddItem(mtr);
                    await Navigation.PushAsync(new MotorPage(mtr, true));
                    break;
                case "Sensor":
                    Sensor snsr = new Sensor()
                    {
                        Code = "New Sensor",
                        Description = ""
                    };
                    DBManager.AddItem(snsr);
                    await Navigation.PushAsync(new SensorPage(snsr, true));
                    break;
                case "OtherComponent":
                    OtherComponent other = new OtherComponent()
                    {
                        Code = "New Component",
                        Description = ""
                    };
                    DBManager.AddItem(other);
                    await Navigation.PushAsync(new OtherComponentPage(other, true));
                    break;
                case "Spare":
                    Spare spr = new Spare()
                    {
                        Code = "New item",
                        Description1 = ""
                    };
                    DBManager.AddItem(spr);
                    await Navigation.PushAsync(new SparePage(spr, true));
                    break;
                case "Employee":
                    Employee emp = new Employee()
                    {
                        FirstName = "New Employee",
                        LastName = ""
                    };
                    DBManager.AddItem(emp);
                    await Navigation.PushAsync(new EmployeePage(emp, true));
                    break;
                case "ExpensItem":
                    ExpensItem exp = new ExpensItem()
                    {
                        Title = "New Expense Item",
                        Description = "Expense details"
                    };
                    DBManager.AddItem(exp);
                    await Navigation.PushAsync(new ExpensePage(exp, true));
                    break;

            }

        }
        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        public async void RemoveItem<T>(T item)
        {
            var xx = typeof(T).Name;
            switch (xx)
            {
                case "Equipment":
                    bool answer = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as Equipment).Code + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer == true)
                    {
                        DBManager.RemoveItem(item as Equipment);
                    }
                    break;
                case "Motor":
                    bool answer1 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as Motor).Code + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer1 == true)
                    {
                        var getAllById = DBManager.realm.All<Motor>().Where(x => x.Id == (item as Motor).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "Sensor":
                    bool answer11 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as Sensor).Code + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer11 == true)
                    {
                        var getAllById = DBManager.realm.All<Sensor>().Where(x => x.Id == (item as Sensor).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "OtherComponent":
                    bool answer12 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as Sensor).Code + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer12 == true)
                    {
                        var getAllById = DBManager.realm.All<OtherComponent>().Where(x => x.Id == (item as OtherComponent).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "Schedule":
                    bool answer3 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as Schedule).ItemCode + " "+ Properties.Resources.Schedule + " "+ (item as Schedule).Repairdetails + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer3 == true)
                    {
                        var getAllById = DBManager.realm.All<Schedule>().Where(x => x.Id == (item as Schedule).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "Employee":
                    bool answer4 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as Employee).FirstName + " " + (item as Employee).LastName + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer4 == true)
                    {
                        var getAllById = DBManager.realm.All<Employee>().Where(x => x.Id == (item as Employee).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "ExpensItem":
                    bool answer5 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as ExpensItem).Title + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer5 == true)
                    {
                        var getAllById = DBManager.realm.All<ExpensItem>().Where(x => x.Id == (item as ExpensItem).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "Budgetitem":
                    bool answer6 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as Budgetitem).Title + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer6 == true)
                    {
                        var getAllById = DBManager.realm.All<Budgetitem>().Where(x => x.Id == (item as Budgetitem).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "Shop":
                    bool answer7 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as Shop).ShopName + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer7 == true)
                    {
                        var getAllById = DBManager.realm.All<Shop>().Where(x => x.Id == (item as Shop).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "ProductionLine":
                    bool answer8 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm + (item as ProductionLine).Code + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer8 == true)
                    {
                        var getAllById = DBManager.realm.All<ProductionLine>().Where(x => x.Id == (item as ProductionLine).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
                case "SafetyReport":
                    bool answer9 = await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DeleteConfirm +Properties.Resources.SafetyReport + " ?", Properties.Resources.Ok, Properties.Resources.No);
                    if (answer9 == true)
                    {
                        var getAllById = DBManager.realm.All<SafetyReport>().Where(x => x.Id == (item as SafetyReport).Id).First();

                        DBManager.realm.Write(() =>
                        {
                            DBManager.realm.Remove(getAllById);
                        });
                    }
                    break;
            }

        }
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="searchText">The search text.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public List<T> GetItems<T>(string searchText = null, string criteria = "")
        {
            var xx = typeof(T).Name;
            if (DBManager.realm != null && !DBManager.realm.IsInTransaction && !DBManager.realm.IsClosed)
            {
                DBManager.realm.Refresh();
                if (xx == "Equipment" && !string.IsNullOrEmpty(searchText))
                {
                    switch (criteria)
                    {
                        case "Code":
                            return DBManager.realm.All<Equipment>().Where(p => p.Code.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList() as List<T>;
                        case "Description":
                            return DBManager.realm.All<Equipment>().Where(p => p.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList() as List<T>;
                        case "Area":
                            return DBManager.realm.All<Equipment>().Where(p => p.ShopExtra.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList() as List<T>;
                        default:
                            return DBManager.realm.All<Equipment>().OrderBy(u => u.Code).ToList() as List<T>;
                    }
                }
                else if (xx == "Equipment" && string.IsNullOrEmpty(searchText))
                    return DBManager.realm.All<Equipment>().OrderBy(u => u.Code).ToList() as List<T>;
                else if (xx == "Motor")
                {
                    if (!string.IsNullOrEmpty(searchText) && searchText.Length > 0)
                    {
                        double rpm; bool isRpm = double.TryParse(searchText, out rpm);
                        double kw; bool iskw = double.TryParse(searchText, out kw);
                        return DBManager.realm.All<Motor>().Where(p => p.Code.Contains(searchText)
                                                                  || p.Description.Contains(searchText)
                                                                  || (isRpm && rpm / p.RPM < .15)
                                                                  || (iskw && (kw / p.Power) < .02)
                                                                   ).ToList() as List<T>;
                    }
                    else
                        return DBManager.realm.All<Motor>().OrderBy(u => u.Code).ToList() as List<T>;

                }
                else if (xx == "Sensor")
                {
                    if (!string.IsNullOrEmpty(searchText) && searchText.Length > 0)
                    {
                        return DBManager.realm.All<Sensor>().Where(p => p.Code.Contains(searchText)
                                                                  || p.Description.Contains(searchText)

                                                                   ).ToList() as List<T>;
                    }
                    else
                        return DBManager.realm.All<Sensor>().OrderBy(u => u.Code).ToList() as List<T>;

                }
                else if (xx == "OtherComponent")
                {
                    if (!string.IsNullOrEmpty(searchText) && searchText.Length > 0)
                    {
                        return DBManager.realm.All<OtherComponent>().Where(p => p.Code.Contains(searchText)
                                                                  || p.Description.Contains(searchText)

                                                                   ).ToList() as List<T>;
                    }
                    else
                        return DBManager.realm.All<OtherComponent>().OrderBy(u => u.Code).ToList() as List<T>;

                }
                else if (xx == "Log")
                {
                    switch (criteria)
                    {
                        case "Code":
                            var emp = DBManager.realm.All<Employee>();
                            return DBManager.realm.All<Log>().Where(p => p.ItemDescription.Contains(searchText) ||
                            p.DateLog.ToString().Contains(searchText) ||
                             p.Cost.ToString().Contains(searchText)
                            ).ToList() as List<T>;

                        default:
                            return DBManager.realm.All<Log>().OrderBy(u => u.DateLog).ToList() as List<T>;
                    }
                }
                else if (xx == "Schedule")
                {
                    switch (criteria)
                    {
                        case "Code":
                            return DBManager.realm.All<Schedule>().Where(p => p.ItemDescription.Contains(searchText) ||
                            p.DateScheduleFrom.ToString().Contains(searchText) ||
                            p.DateScheduleTo.ToString().Contains(searchText) ||
                           p.SparesCost.ToString().Contains(searchText) ||
                             p.RepairCost.ToString().Contains(searchText)
                            ).ToList() as List<T>;

                        default:
                            return DBManager.realm.All<Schedule>().OrderBy(u => u.DateScheduleFrom).ToList() as List<T>;
                    }
                }
                else if (xx == "Employee")
                {
                    if (!string.IsNullOrEmpty(searchText) && searchText.Length > 0)
                    {
                        return DBManager.realm.All<Employee>().Where(p => p.FirstName.Contains(searchText) ||
                                                                   p.LastName.Contains(searchText)).ToList() as List<T>;
                    }
                    else
                        return DBManager.realm.All<Employee>().OrderBy(u => u.LastName).ToList() as List<T>;

                }
                else if (xx == "ExpensItem")
                {
                    if (!string.IsNullOrEmpty(searchText) && searchText.Length > 0)
                    {
                        return DBManager.realm.All<ExpensItem>().Where(p => p.Description.Contains(searchText)
                                                                  || p.ItemType.Contains(searchText)
                                                                  || p.PartCode.Contains(searchText)
                                                                  || p.Title.Contains(searchText)
                                                                  || p.Value.ToString().Contains(searchText)
                                                                  || p.DateExpense.ToString().Contains(searchText)
                                                                   ).ToList() as List<T>;
                    }
                    else
                        return DBManager.realm.All<ExpensItem>().OrderBy(u => u.Value).ToList() as List<T>;

                }
                else if (xx == "Budgetitem")
                {
                    if (!string.IsNullOrEmpty(searchText) && searchText.Length > 0)
                    {
                        return DBManager.realm.All<Budgetitem>().Where(p => p.Description.Contains(searchText)
                                                                  || p.ItemType.Contains(searchText)
                                                                  || p.PartCode.Contains(searchText)
                                                                  || p.Title.Contains(searchText)
                                                                  || p.Value.ToString().Contains(searchText)
                                                                   ).ToList() as List<T>;
                    }
                    else
                        return DBManager.realm.All<Budgetitem>().OrderBy(u => u.Value).ToList() as List<T>;

                }
                else if (xx == "SafetyReport")
                {
                   /* if (!string.IsNullOrEmpty(searchText) && searchText.Length > 0)
                    {
                        return DBManager.realm.All<SafetyReport>().Where(p => p.Description.Contains(searchText)
                                                                  || p.ItemType.Contains(searchText)
                                                                  || p.PartCode.Contains(searchText)
                                                                  || p.Title.Contains(searchText)
                                                                  || p.Value.ToString().Contains(searchText)
                                                                   ).ToList() as List<T>;
                    }
                    else*/
                        return DBManager.realm.All<SafetyReport>().OrderBy(u => u.IssueDate).ToList() as List<T>;

                }
            }
            else
                page.DisplayAlert(Properties.Resources.Info, Properties.Resources.DatabaseNotAv, Properties.Resources.Ok);
            return null;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.LoadInventoryList()'
        public void LoadInventoryList()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.LoadInventoryList()'
        {
            InventoryList = new List<Spareitem>();
            InventorySelectedItem = new Spareitem();
            foreach (var x in DBManager.realm.All<Spare>().ToList())
            {
                InventoryList.Add(new Spareitem()
                {
                    code = x.Code,
                    description1 = x.Description1,
                    description2 = x.Description2
                });
            }
        }
        async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            IsNotConnected = e.NetworkAccess != NetworkAccess.Internet;
            if (IsNotConnected) {
                IsBusy = true;
                LoggedUser = "";
                IsBusyTitle = Properties.Resources.NetworkWaiting;
            }
            else
            {   
                LoggedUser = PlantBU.Properties.Resources.LoggedUser + DBManager.CurrentUser.FirstName + " " + DBManager.CurrentUser.LastName; 
                IsBusyTitle = Properties.Resources.Loading;
                await DBManager.ConnectCloud();
                IsBusy = false;
            }
                


        }

       
        #region INotifyPropertyChanged
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.PropertyChanged'
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BaseViewModel.PropertyChanged'
        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage">The storage.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion       
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Spareitem'
    public class Spareitem
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Spareitem'
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string code { get; set; }
        /// <summary>
        /// Gets or sets the description1.
        /// </summary>
        /// <value>The description1.</value>
        public string description1 { get; set; }
        /// <summary>
        /// Gets or sets the description2.
        /// </summary>
        /// <value>The description2.</value>
        public string description2 { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Spareitem.Value'
        public double Value { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Spareitem.Value'
    }
}
