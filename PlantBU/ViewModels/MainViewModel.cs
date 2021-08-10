// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-02-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="MainViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.Properties;
using PlantBU.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class MainViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    class MainViewModel : BaseViewModel
    {
        

        public List<Model> MenuItems { get { return _MenuItems; } set { _MenuItems = value; OnPropertyChanged("MenuItems"); } }
        List<Model> _MenuItems;
        public List<Employee> employees { get { return _employees; } set { _employees = value; OnPropertyChanged("employees"); } }
        List<Employee> _employees;
        
        public bool IsLoginView { get { _IsNotLoginView = !_LoginView; return _LoginView ; } set { _LoginView = value; _IsNotLoginView = !_LoginView; OnPropertyChanged("IsLoginView"); } }
        bool _LoginView;
        public bool IsNotLoginView { get { return _IsNotLoginView; } set { _IsNotLoginView = value; OnPropertyChanged("IsNotLoginView"); } }
        bool _IsNotLoginView;
        public bool IsLoginEnabled { get { return _LoginDisable; } set { _LoginDisable = value; OnPropertyChanged("IsLoginEnabled"); } }
        bool _LoginDisable;
        
        public int LogsBadge { get { return _LogsBadge; } set { _LogsBadge = value; OnPropertyChanged("LogsBadge"); } }
        int _LogsBadge;
        public int ScheduleBadge { get { return _ScheduleBadge; } set { _ScheduleBadge = value; OnPropertyChanged("ScheduleBadge"); } }
        int _ScheduleBadge;

        public string LogsBadgeString { get { return _LogsBadgeLogsBadgeString; } set { _LogsBadgeLogsBadgeString = value; OnPropertyChanged("LogsBadgeString"); } }
        string _LogsBadgeLogsBadgeString;
        public string ScheduleBadgeString { get { return _ScheduleBadgeString; } set { _ScheduleBadgeString = value; OnPropertyChanged("ScheduleBadgeString"); } }
        string _ScheduleBadgeString;

        public Command<object> ListViewTappedCommand { get; private set; }

        public MainViewModel()
        {
           

            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            //GetUserMunue();
            if (DBManager.IsDBReady)
                employees = DBManager.realm.All<Employee>().ToList();
        }
        public async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Model)
                {

                    string MenuItem = ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Model).Category;
                    if (MenuItem == Resources.Equipments)
                        await Navigation.PushAsync(new EquipmentsPage());
                    if (MenuItem == Resources.Logs)
                        await Navigation.PushAsync(new LogsPage());
                    if (MenuItem == Resources.Schedules)
                        await Navigation.PushAsync(new SchedulesPage());
                    if (MenuItem == Resources.Inventory)
                        if (DBManager.realm.All<Inventory>().Count() == 1)
                            await Navigation.PushAsync(new InventoryPage(DBManager.realm.All<Inventory>().First()));
                        else if (DBManager.realm.All<Inventory>().Count() > 1)
                            await Navigation.PushAsync(new InventoriesPage());
                    if (MenuItem == Resources.EmployeeInfo)
                    {
                        if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                            await Navigation.PushAsync(new EmployeesPage());
                        if (DBManager.CurrentUserType == DBManager.Usertyypes.User)
                            await Navigation.PushAsync(new EmployeePage(DBManager.CurrentUser, true));
                    }
                    if (MenuItem == Resources.Expenses)
                        await Navigation.PushAsync(new ExpensesPage());
                    if (MenuItem == Resources.Plant)
                        await Navigation.PushAsync(new PlantPage());
                    if (MenuItem == Resources.Budget)
                        await Navigation.PushAsync(new BudgetPage());
                    if (MenuItem == Resources.Safety)
                        await Navigation.PushAsync(new SafetyReportsPage());
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        public async void GetNewBadges()
        {
            try
            {
                var NewLog = Preferences.Get("NewLog", 0);
                var NewSchedule = Preferences.Get("NewSchedule", 0);
                if (DBManager.realm.All<Log>().Any())
                {
                    if (NewLog == 1)
                    {
                        // var xx = DBManager.realm.All<Log>().ToList();
                        // LogsBadge = NewLog;//DBManager.realm.All<Log>().ToList().Where(x => x.Id.CreationTime.Date == DateTimeOffset.Now.Date).ToList().Count();
                        LogsBadgeString =Properties.Resources.New;
                        LogsBadge = Preferences.Get("NewLogsCount", 0);

                    }
                    else
                    { LogsBadge = 0; LogsBadgeString = ""; }
                }
                if (DBManager.realm.All<Schedule>().Any())
                {
                    if (NewSchedule == 1)
                    {
                        //ScheduleBadge = NewSchedule;//DBManager.realm.All<Schedule>().ToList().Where(x => x.Id.CreationTime.Date == DateTimeOffset.Now.Date).ToList().Count();
                        ScheduleBadge = Preferences.Get("NewSchedulesCount", 0);
                        ScheduleBadgeString = Properties.Resources.New;

                    }
                    else
                    { ScheduleBadge = 0; ScheduleBadgeString = ""; }
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }

        }
        public async void GetAdminMunue()
        {
            try
            {
                GetNewBadges();
                MenuItems = new List<Model>()
                    {
                 new Model(){Category = Resources.Plant, Color = Color.FromHex("#e3165b")},
                new Model(){Category =Resources.Equipments,  Color = Color.FromHex("#e3165b")},
                new Model(){Category = Resources.Logs,Count = LogsBadge , Color = Color.Brown},
                new Model(){Category = Resources.Schedules,Count = ScheduleBadge , Color = Color.Brown},
                new Model(){Category = Resources.Expenses,  Color = Color.FromHex("#2088da")},
                new Model(){Category = Resources.Budget,  Color = Color.FromHex("#2088da")},
                new Model(){Category = Resources.Inventory,  Color = Color.FromHex("#2088da")},
                new Model(){Category = Resources.Safety,  Color = Color.FromHex("#2088da")},
                new Model(){Category = Resources.EmployeeInfo , Color = Color.FromHex("#2088da")} };
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        public async void GetManagerMunue()
        {
            try
            {
                GetNewBadges();
                MenuItems = new List<Model>()
                    {
               new Model(){Category = Resources.Plant, Color = Color.FromHex("#e3165b")},
                new Model(){Category =Resources.Equipments,  Color = Color.FromHex("#e3165b")},
                new Model(){Category = Resources.Logs,Count = LogsBadge , Color = Color.Brown},
                new Model(){Category = Resources.Schedules,Count = ScheduleBadge , Color = Color.Brown},
                 new Model(){Category = Resources.Expenses,  Color = Color.FromHex("#2088da")},
                  new Model(){Category = Resources.Budget,  Color = Color.FromHex("#2088da")},
                new Model(){Category = Resources.Inventory,  Color = Color.FromHex("#2088da")},
                                new Model(){Category = Resources.Safety,  Color = Color.FromHex("#2088da")},

                new Model(){Category = Resources.EmployeeInfo , Color = Color.FromHex("#2088da")} };
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        public async void GetUserMunue()
        {
            try
            {
                GetNewBadges();
                MenuItems = new List<Model>()
                    {
               new Model(){Category =Resources.Equipments,  Color = Color.FromHex("#e3165b")},
                new Model(){Category = Resources.Logs,Count = LogsBadge , Color = Color.Brown},
                new Model(){Category = Resources.Schedules,Count = ScheduleBadge , Color = Color.Brown},
                new Model(){Category = Resources.Inventory,  Color = Color.FromHex("#2088da")},
                                new Model(){Category = Resources.Safety,  Color = Color.FromHex("#2088da")},

                new Model(){Category = Resources.EmployeeInfo , Color = Color.FromHex("#2088da")} };
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Model'
    public class Model
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Model'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Model.Category'
        public string Category { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Model.Category'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Model.Count'
        public int Count { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Model.Count'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Model.NewItem'
        public string NewItem { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Model.NewItem'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Model.Color'
        public Color Color { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Model.Color'
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ZeroValueConverter'
    public class ZeroValueConverter : IValueConverter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ZeroValueConverter'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ZeroValueConverter.Convert(object, Type, object, CultureInfo)'
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ZeroValueConverter.Convert(object, Type, object, CultureInfo)'
        {
            if (!value.ToString().Equals("0"))
            {
                return value.ToString();// + " New";
            }

            return string.Empty;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ZeroValueConverter.ConvertBack(object, Type, object, CultureInfo)'
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ZeroValueConverter.ConvertBack(object, Type, object, CultureInfo)'
        {
            throw new NotImplementedException();
        }
    }
}
