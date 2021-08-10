// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-12-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-02-2021
// ***********************************************************************
// <copyright file="App.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.Utilities;
using PlantBU.Views;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlantBU
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'App'
    public partial class App : Application
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'App'
    {
        
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'App.App()'
        public App()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'App.App()'
        {

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDgyNjYzQDMxMzkyZTMyMmUzMFlQWFZZVkRaWXE0ck11amIwWGYyUThwNHlNRGpVSDVCdTV2MlhPcDNOUXc9");
           // IronOcr.Installation.LicenseKey = "IRONOCR.WALIDALKADY.18667-8B79BB900A-IT23KQNC7P3ZXDPT-VV7ZIP5XILPR-XCHDHYNFATTL-I4RCGNRJFJ2V-UWEYHNZ3H2DT-LF72CM-TM7Y2CIIOJ6BUA-DEPLOYMENT.TRIAL-PWKQQE.TRIAL.EXPIRES.09.SEP.2021";

            InitializeComponent();
            // Use the dependency service to get a platform-specific implementation and initialize it.
            if (Device.RuntimePlatform != Device.UWP)
            {
                DependencyService.Get<INotificationManager>().Initialize();
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'App.OnStart()'
        protected override void OnStart()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'App.OnStart()'
        {
            
            try
            {
                CultureInfo language = new CultureInfo(Preferences.Get("Lang", "en"));
                Thread.CurrentThread.CurrentUICulture = language;
                PlantBU.Properties.Resources.Culture = language;

                
            }
            catch
            {              
            }
            finally
            {
                MainPage = new NavigationPage(new MainPage());
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'App.OnSleep()'
        protected override void OnSleep()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'App.OnSleep()'
        {
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'App.OnResume()'
        protected override void OnResume()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'App.OnResume()'
        {
        }
       

    }
}
