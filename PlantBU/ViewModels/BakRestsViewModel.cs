// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-26-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-06-2021
// ***********************************************************************
// <copyright file="BakRestsViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using Realms;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class BakRestsViewModel.
    /// </summary>
    class BakRestsViewModel
    {
        /// <summary>
        /// Gets or sets the baks.
        /// </summary>
        /// <value>The baks.</value>
        public List<string> baks { get; set; }
        /// <summary>
        /// Gets or sets the ListView selected item.
        /// </summary>
        /// <value>The ListView selected item.</value>
        public string ListViewSelectedItem { get; set; }
        /// <summary>
        /// Gets or sets the navigation.
        /// </summary>
        /// <value>The navigation.</value>
        internal INavigation Navigation { get; set; }
        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        /// <value>The page.</value>
        internal Page page { get; set; }
        /// <summary>
        /// Gets the ListView tapped command.
        /// </summary>
        /// <value>The ListView tapped command.</value>
        public Command<object> ListViewTappedCommand { get; private set; }
        /// <summary>
        /// Gets the ListView holding command.
        /// </summary>
        /// <value>The ListView holding command.</value>
        public Command<object> ListViewHoldingCommand { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BakRestsViewModel"/> class.
        /// </summary>
        public BakRestsViewModel()
        {
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            BackUpsReLoad();
        }
        /// <summary>
        /// Backs the ups re load.
        /// </summary>
        public async void BackUpsReLoad()
        {
            try
            {
                string pth = DBManager.realm.Config.DatabasePath;
                baks = new List<string>();
                baks.Clear();
                foreach (var fl in Directory.GetFiles(Path.GetDirectoryName(pth)))
                {
                    if (Path.GetFileName(fl) != "Titan.realm" && Path.GetFileName(fl) != "Titan.realm.lock")
                    {
                        string st = Path.GetFileName(fl).Substring(6);
                        baks.Add(st.Replace(".realm", ""));
                    }

                }
            }
            catch (Exception ex)
            {

                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        /// <summary>
        /// ListViews the tapped command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is string)
                {
                    bool answer = await page.DisplayAlert(Properties.Resources.Error, "Confirm Restore!", "Yes", "No");

                    if (answer)
                    {
                        string bak = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as string;
                        RealmConfiguration conf = new RealmConfiguration("Titan_" + bak + ".realm");
                        bool xx = DBManager.Restore(conf);
                        if (xx)
                        {
                            await page.DisplayAlert(Properties.Resources.Error, "Restore Successfull!", "ok");
                            BackUpsReLoad();
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                await page.DisplayAlert(Properties.Resources.Error, ex.Message, "ok");
            }
        }
        /// <summary>
        /// ListViews the holding command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        private async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() != typeof(string))
                    return;
                string bak = (string)(obj as ItemHoldingEventArgs).ItemData;
                string action = await page.DisplayActionSheet("PantBU", "Cancel", null, "Backup", "Remove Backup");

                if (action == "Backup")
                {
                    RealmConfiguration conf = new RealmConfiguration("Titan_" + bak + ".realm");
                    DBManager.Restore(conf);
                    BackUpsReLoad();
                    await page.DisplayAlert(Properties.Resources.Error, "Restore Successfull!", "ok");
                }
                else if (action == "Remove Backup")
                {
                    RealmConfiguration conf = new RealmConfiguration("Titan_" + bak + ".realm");
                    if (File.Exists(conf.DatabasePath))
                    {
                        Realm.DeleteRealm(conf);
                        BackUpsReLoad();
                        await page.DisplayAlert(Properties.Resources.Error, bak + ":Backup deleted!", "Ok");
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Error, bak + ":Backup Not Found!", "Ok");
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, "ok");
            }
        }

    }
}
