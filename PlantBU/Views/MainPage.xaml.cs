// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-12-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="MainPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using PlantBU.Properties;
using System.Threading.Tasks;
using PlantBU.Utilities;
using Realms;
using Realms.Sync.Exceptions;

namespace PlantBU.Views
{
    /// <summary>
    /// Class MainPage.
    /// Implements the <see cref="Syncfusion.XForms.Backdrop.SfBackdropPage" />
    /// </summary>
    /// <seealso cref="Syncfusion.XForms.Backdrop.SfBackdropPage" />
    public partial class MainPage
    {
        INotificationManager notificationManager;
        int notificationNumber = 0;
        ToolbarItem BakupRestore, Defaults;
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            try
            {
                
                // Register Syncfusion license
                InitializeComponent();
                        
                (BindingContext as MainViewModel).Navigation = Navigation;
                (BindingContext as MainViewModel).page = this;
                if(Device.RuntimePlatform != Device.UWP) { 
                    notificationManager = DependencyService.Get<INotificationManager>();
                    notificationManager.NotificationReceived += (sender, eventArgs) =>
                    {
                        var evtData = (NotificationEventArgs)eventArgs;
                        ShowNotification(evtData.Title, evtData.Message);
                    };
                }
                if (Device.RuntimePlatform == Device.UWP)
                {
                    BakupRestore = new ToolbarItem()
                    {
                        Text = Properties.Resources.Back_Restore,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId = "BakupRestore"
                    };
                    BakupRestore.Clicked += ToolbarItem_Clicked;
                    Defaults = new ToolbarItem()
                    {
                        Text = Properties.Resources.Defaults,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId = "Defaults"
                    };
                    Defaults.Clicked += ToolbarItem_Clicked;
                    this.ToolbarItems.Add(BakupRestore);
                    this.ToolbarItems.Add(Defaults);
                }
                CircularProgressBar.IsVisible = false;
            }
            catch (Exception ex)
            {
                if(CircularProgressBar!=null)
                     CircularProgressBar.IsVisible = false;
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
                //Environment.Exit(0);
            }

        }
        /// <summary>
        /// When overridden, allows application developers to customize behavior immediately
        /// prior to the Page becoming visible.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                

                //Connect to database
                if (!DBManager.IsDBReady || DBManager.SessionStatus == Properties.Resources.InActive)
                {
                    (BindingContext as MainViewModel).IsBusy = true;
                    (BindingContext as MainViewModel).IsLoginEnabled = false;
                    (BindingContext as MainViewModel).IsEnabled = false;
                    await DBManager.ConnectCloud();
                    (BindingContext as MainViewModel).employees = DBManager.realm.All<Employee>().ToList();
                    (BindingContext as MainViewModel).IsLoginView = true;
                    (BindingContext as MainViewModel).IsLoginEnabled = true;
                    (BindingContext as MainViewModel).IsEnabled = false;
                    (BindingContext as MainViewModel).IsBusy = false;
                   
                }
                //Connected But no user
                else if (DBManager.SessionStatus == Properties.Resources.Active && DBManager.CurrentUser == null)
                {
                    (BindingContext as MainViewModel).IsLoginView = true;
                    (BindingContext as MainViewModel).IsEnabled = false;
                    return;
                }
                //Resuming
                else if (DBManager.SessionStatus == Properties.Resources.Active && DBManager.CurrentUser != null)
                {
                    (BindingContext as MainViewModel).IsLoginView = false;
                    (BindingContext as MainViewModel).IsEnabled = true;
                    SetUserMenu();
                }

                Realms.Sync.Session.Error += (session, errorArgs) =>
                {
                    var sessionException = (SessionException)errorArgs.Exception;
                    switch (sessionException.ErrorCode)
                    {
                        case ErrorCode.AccessTokenExpired:
                            if (Device.RuntimePlatform != Device.UWP)
                            {
                                string title = $"Access Token Expired";
                                string message = $"Notifcation ( {notificationNumber} ) PlantBU Started!";
                                notificationManager.SendNotification(title, message);
                            }
                            break;
                        case ErrorCode.BadUserAuthentication:
                            if (Device.RuntimePlatform != Device.UWP)
                            {
                                string title = $"Bad User Authentication";
                                string message = $"Notifcation ( {notificationNumber} ) PlantBU Started!";
                                notificationManager.SendNotification(title, message);
                            }
                            break;
                        case ErrorCode.PermissionDenied:
                            if (Device.RuntimePlatform != Device.UWP)
                            {
                                string title = $"Permission Denied";
                                string message = $"Notifcation ( {notificationNumber} ) PlantBU Started!";
                                notificationManager.SendNotification(title, message);
                            }
                            break;
                        case ErrorCode.Unknown:
                            if (Device.RuntimePlatform != Device.UWP)
                            {
                                string title = $"Unknown";
                                string message = $"Notifcation ( {notificationNumber} ) PlantBU Started!";
                                notificationManager.SendNotification(title, message);
                            }
                            break;
                    }
                };


                //Notifications
                /*
                DBManager.LogsChanged = DBManager.realm.All<Log>().SubscribeForNotifications((sender1, changes, error) =>
                {
                    DBManager.LogChanges(changes);

                    if (Device.RuntimePlatform != Device.UWP && changes.InsertedIndices.Count()>0)
                    {
                        string title = $"PlantBU";
                        string message = $"{changes.InsertedIndices.Count()} New Logs !";
                        notificationManager.SendNotification(title, message);
                    }
                });
                DBManager.SchedulessChanged = DBManager.realm.All<Schedule>().SubscribeForNotifications((sender1, changes, error) =>
                {
                    DBManager.ScheduleChanges(changes );
                    if (Device.RuntimePlatform != Device.UWP && changes.InsertedIndices.Count() > 0)
                    {
                        string title = $"PlantBU";
                        string message = $"{changes.InsertedIndices.Count()} New Schedules!";
                        notificationManager.SendNotification(title, message);
                    }
                });
                */
                if (Preferences.Get("Lang", "en") == "en")
                    Lang.Text = "EN";
                else
                    Lang.Text = "عربي";
             
                 Employee LoggedEmpl = null;
                foreach (var em in DBManager.realm.All<Employee>().ToList())
                {
                    if (!string.IsNullOrEmpty(em?.UserName) && Preferences.ContainsKey(em.UserName))
                    {
                        LoggedEmpl = em;
                    }
                }
                if (LoggedEmpl != null)
                    LogInUser(LoggedEmpl);

                //TODO: Test part
               
               /* await DBManager.realm.WriteAsync(realm =>
                {
                    realm.RemoveAll<Schedule>();
                });*/
            }

            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);

            }
        }
        
        /// <summary>
        /// Handles the ItemSelected event of the ListView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectedItemChangedEventArgs"/> instance containing the event data.</param>
        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem.ToString() == Properties.Resources.Equipment)
                    await Navigation.PushAsync(new EquipmentsPage());
                if (e.SelectedItem.ToString() == Properties.Resources.Logs)
                    await Navigation.PushAsync(new LogsPage());
                if (e.SelectedItem.ToString() == Properties.Resources.Schedules)
                    await Navigation.PushAsync(new SchedulesPage());
                if (e.SelectedItem.ToString() == Properties.Resources.Inventory)
                {
                    if (DBManager.realm.All<Inventory>().Count() == 1)
                        await Navigation.PushAsync(new InventoryPage(DBManager.realm.All<Inventory>().First()));
                    else if (DBManager.realm.All<Inventory>().Count() > 1)
                        await Navigation.PushAsync(new InventoriesPage());
                }
                if (e.SelectedItem.ToString() == Properties.Resources.EmployeeInfo)
                {
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        await Navigation.PushAsync(new EmployeesPage());
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.User)
                        await Navigation.PushAsync(new EmployeePage(DBManager.CurrentUser, true));
                }
                if (e.SelectedItem.ToString() == Properties.Resources.Expenses)
                    await Navigation.PushAsync(new ExpensesPage());
                if (e.SelectedItem.ToString() == Properties.Resources.Plant)
                    await Navigation.PushAsync(new PlantPage());
                if (e.SelectedItem.ToString() == Properties.Resources.Budget)
                    await Navigation.PushAsync(new BudgetPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

        /// <summary>
        /// Handles the Clicked event of the BakupRestore control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async System.Threading.Tasks.Task BakupRestore_ClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new BakRestPage());
        }

        /// <summary>
        /// Reports the progress.
        /// </summary>
        /// <param name="value">The value.</param>
        async void ReportProgress(int value)
        {
            //Update the UI to reflect the progress value that is passed back.
            try
            {
                CustomContentProgressBarLabel.Text = string.Format("{0:00} %", value);
                CircularProgressBar.Progress = value;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);

            }
        }
        private async void LogIn_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!DBManager.realm.All<Employee>().Any() && UserPasswordEntry.Text == "Titan_12")
                {
                    DBManager.realm.Write(() =>
                    {
                        DBManager.realm.Add<Employee>(new Employee()
                        {
                            FirstName = "Walid",
                            LastName = "ALkady",
                            UserName = "welkady",
                            Password = "Titan_12"
                        });
                    });
                    DBManager.CurrentUserType = DBManager.Usertyypes.Admin;
                    (BindingContext as MainViewModel).IsEnabled = true;
                    (BindingContext as MainViewModel).IsLoginView = false;
                    SetUserMenu();
                    (BindingContext as MainViewModel).LoggedUser = Properties.Resources.LoggedUser + "Walid" + " " + "ALkady";
                    LogOut.IsEnabled = true;
                    ForgetUser.IsEnabled = true;
                    if (!Preferences.ContainsKey("welkady"))
                        Preferences.Set("welkady", "Titan_12");
                }
               
                //Employee empl = (UsercomboBox.SelectedItem as Employee);
                else if ((UsercomboBox.SelectedItem as Employee) != null
                    && (UsercomboBox.SelectedItem as Employee).UserName != ""
                    && UserPasswordEntry.Text == (UsercomboBox.SelectedItem as Employee).Password) //empl != null
                {
                    Employee empl = (UsercomboBox.SelectedItem as Employee);
                    LogInUser(empl);
                    if (!Preferences.ContainsKey(empl.UserName))
                        Preferences.Set(empl.UserName, empl.Password);
                }

                else
                    await DisplayAlert(Properties.Resources.Info, Properties.Resources.UserError, Properties.Resources.Ok);

                if (DBManager.IsDBconnected && !DBManager.IsDBLoadedWithData)
                {
                    string action = await DisplayActionSheet(Properties.Resources.LoadDefaultData, Properties.Resources.Cancel, null, Properties.Resources.Ok, Properties.Resources.No);
                    if (action == Properties.Resources.Ok)
                    {
                        CircularProgressBar.IsVisible = true;
                        var progressIndicator = new Progress<int>(ReportProgress);
                        await DBManager.DBDefaults(progressIndicator);
                        CircularProgressBar.IsVisible = false;
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void SetUserMenu()
        {
            try
            {
                if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                    (BindingContext as MainViewModel).GetAdminMunue();
                else if (DBManager.CurrentUser.Title == "Manager" || DBManager.CurrentUser.Title == "Engineer" ||
                   DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                    (BindingContext as MainViewModel).GetManagerMunue();
                else
                    (BindingContext as MainViewModel).GetUserMunue();
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
                            case "ForgetUser":
                                Employee empl = DBManager.realm.All<Employee>().Where(x => x.Id == DBManager.CurrentUser.Id).FirstOrDefault();

                                Preferences.Clear();
                                DBManager.CurrentUser = null;
                                (BindingContext as MainViewModel).IsEnabled = false;
                                (BindingContext as MainViewModel).IsLoginView = true;
                                (BindingContext as MainViewModel).LoggedUser = Properties.Resources.LogInMessage;
                                (BindingContext as MainViewModel).GetUserMunue();
                                LogOut.IsEnabled = false;
                                ForgetUser.IsEnabled = false;
                                break;
                            case "LogOut":
                                DBManager.CurrentUser = null;
                                (BindingContext as MainViewModel).IsEnabled = false;
                                (BindingContext as MainViewModel).IsLoginView = true;
                                (BindingContext as MainViewModel).LoggedUser = Properties.Resources.LogInMessage;
                                (BindingContext as MainViewModel).GetUserMunue();
                                LogOut.IsEnabled = false;
                                ForgetUser.IsEnabled = false;
                                break;
                            case "Lang":
                            string SavedLang= Preferences.Get("Lang", "en"); CultureInfo language = null;
                            if (SavedLang == "en")
                            {
                                language = new CultureInfo("ar-EG");
                                Thread.CurrentThread.CurrentUICulture = language;
                                PlantBU.Properties.Resources.Culture = language;
                                Preferences.Set("Lang", "ar-EG"); 
                                Lang.Text = "عربي"; }
                            else 
                            {
                                language = new CultureInfo("en");
                                Thread.CurrentThread.CurrentUICulture = language;
                                PlantBU.Properties.Resources.Culture = language; 
                                Preferences.Set("Lang", "en"); Lang.Text = "EN"; }
                            App.Current.MainPage = new NavigationPage(new MainPage());
                                break;

                        case "Defaults":
                            CircularProgressBar.IsVisible = true;
                            var progressIndicator = new Progress<int>(ReportProgress);
                            await DBManager.DBDefaults(progressIndicator);
                            CircularProgressBar.IsVisible = false;
                            break;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void UsercomboBox_SelectionChanged(object sender, Syncfusion.XForms.ComboBox.SelectionChangedEventArgs e)
        {
            try
            {
                Employee empl = (UsercomboBox.SelectedItem as Employee);
                if (empl != null && Preferences.ContainsKey(empl.UserName))
                {
                    DBManager.CurrentUser = empl;
                    UserPasswordEntry.Text = Preferences.Get(empl.UserName, "");
                    (BindingContext as MainViewModel).IsLoginEnabled = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void UserPasswordEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!DBManager.realm.All<Employee>().Any())
                {
                    (BindingContext as MainViewModel).IsLoginEnabled = true;
                    return;
                }
                if ((UsercomboBox.SelectedItem != null && (UserPasswordEntry.Text != "") || UserPasswordEntry.Text == "Titan_12"))
                    (BindingContext as MainViewModel).IsLoginEnabled = true;
                else
                    (BindingContext as MainViewModel).IsLoginEnabled = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private void LogInUser(Employee LoggedEmpl)
        {
            
                    DBManager.CurrentUser = LoggedEmpl;
                    if (LoggedEmpl.UserName == "welkady")
                        DBManager.CurrentUserType = DBManager.Usertyypes.Admin;
                    else
                        DBManager.CurrentUserType = DBManager.Usertyypes.User;
                    (BindingContext as MainViewModel).IsEnabled = true;
                    (BindingContext as MainViewModel).IsLoginView = false;
                    SetUserMenu();
                    (BindingContext as MainViewModel).LoggedUser = PlantBU.Properties.Resources.LoggedUser + LoggedEmpl.FirstName + " " + LoggedEmpl.LastName;
                    LogOut.IsEnabled = true;
                    ForgetUser.IsEnabled = true;
        }
        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
                };
              //  stackLayout.Children.Add(msg);
            });
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        async Task<bool> CheckNetwork()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return false;
            else
                return true;
        }

    }
}

