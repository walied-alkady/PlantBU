// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-18-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-06-2021
// ***********************************************************************
// <copyright file="EquipmentViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.Utilities;
using PlantBU.Views;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class EquipmentViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    public class EquipmentViewModel : BaseViewModel
    {

        /// <summary>
        /// Gets or sets the equipment.
        /// </summary>
        /// <value>The equipment.</value>
        public Equipment Equipment { get { return _Equipment; } set { _Equipment = value; OnPropertyChanged("Equipment"); } }
        /// <summary>
        /// The equipment
        /// </summary>
        Equipment _Equipment;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.EquipmentLine'
        public string EquipmentLine { get { return _EquipmentLine; } set { _EquipmentLine = value; OnPropertyChanged("EquipmentLine"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.EquipmentLine'
        /// <summary>
        /// The equipment
        /// </summary>
        string _EquipmentLine;

        /// <summary>
        /// Gets or sets the motors.
        /// </summary>
        /// <value>The motors.</value>
        public List<Motor> Motors { get { return _Motors; } set { _Motors = value; OnPropertyChanged("Motors"); } }
        /// <summary>
        /// The motors
        /// </summary>
        List<Motor> _Motors;
        /// <summary>
        /// Gets the motors count.
        /// </summary>
        /// <value>The motors count.</value>
        public string MotorsCount
        {
            get
            {
                if (Motors != null)
                    return "Total Motors: " + Motors.Count().ToString();
                else
                    return "";
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.Sensors'
        public List<Sensor> Sensors { get { return _Sensors; } set { _Sensors = value; OnPropertyChanged("Sensors"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.Sensors'
        List<Sensor> _Sensors;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.OtherComponent'
        public List<OtherComponent> OtherComponent { get { return _OtherComponent; } set { _OtherComponent = value; OnPropertyChanged("OtherComponent"); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.OtherComponent'
        List<OtherComponent> _OtherComponent;
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
        /// Gets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public Command SearchCommandMotors => new Command<string>((string query) =>
        {
            Motors = GetItems<Motor>(query, "Area");

        });
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.SearchCommandSensors'
        public Command SearchCommandSensors => new Command<string>((string query) =>
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.SearchCommandSensors'
        {
            Sensors = GetItems<Sensor>(query, "Area");

        });
        /// <summary>
        /// Initializes a new instance of the <see cref="EquipmentViewModel"/> class.
        /// </summary>
        public EquipmentViewModel()
        {
            IsBusy = true;
            LineList = DBManager.realm.All<ProductionLine>().ToList();
            ShopList = DBManager.realm.All<Shop>().ToList();
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            IsBusy = false;
        }
        /// <summary>
        /// ListViews the tapped command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Equipment)
                {

                    Equipment eq = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Equipment;
                    if (eq.IsValid)
                    {
                        Equipment = eq;
                        Motors = eq.Motors.ToList();
                        await Navigation.PushAsync(new EquipmentPage(eq));
                    }
                    else
                        await page.DisplayAlert("PlantBU", "Equipment is not Available", Properties.Resources.Ok);
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Motor)
                {
                    Motor mtr = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Motor;
                    if (mtr.IsValid)
                        await Navigation.PushAsync(new MotorPage(mtr));
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Motor + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Sensor)
                {
                    Sensor mtr = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Sensor;
                    if (mtr.IsValid)
                        await Navigation.PushAsync(new SensorPage(mtr));
                    else
                        await page.DisplayAlert("PlantBU", Properties.Resources.Sensor + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is OtherComponent)
                {
                    OtherComponent mtr = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as OtherComponent;
                    if (mtr.IsValid)
                        await Navigation.PushAsync(new OtherComponentPage(mtr));
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.OtherComponent + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        /// <summary>
        /// ListViews the holding command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Equipment))
                {

                    Equipment eq = (Equipment)(obj as ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {
                        string action;
                        if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                            action = await page.DisplayActionSheet(eq.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);
                        else
                            action = await page.DisplayActionSheet(eq.Code, Properties.Resources.Cancel, null, Properties.Resources.Details);

                        if (action == Properties.Resources.Edit)
                        {
                            await Navigation.PushAsync(new EquipmentPage(eq, true));
                        }
                        else if (action == Properties.Resources.Details)
                        {
                            await Navigation.PushAsync(new EquipmentPage(eq));
                        }
                        else if (action == Properties.Resources.Remove)
                        {
                            RemoveItem<Equipment>(eq);

                            await page.DisplayAlert(Properties.Resources.Info, eq.Code + Properties.Resources.Deleted, Properties.Resources.Ok);
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Equipment + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Motor))
                {
                    Motor mtr = (Motor)(obj as ItemHoldingEventArgs).ItemData;
                    string action = "";
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Drawing, Properties.Resources.Edit, Properties.Resources.Remove);
                    else
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Drawing);

                    if (action == Properties.Resources.Edit)
                        await Navigation.PushAsync(new MotorPage(mtr, true));
                    else if (action == Properties.Resources.Details)
                        await Navigation.PushAsync(new MotorPage(mtr));
                    else if (action == Properties.Resources.Drawing)
                    {
                        string doc = await getdrawingAsync(Equipment.Shop);
                        if (doc != null && doc.Length > 0)
                            await Navigation.PushAsync(new DrawingPage(doc));
                    }
                    else if (action == Properties.Resources.Remove)
                    {
                        RemoveItem(mtr);
                        await page.DisplayAlert(Properties.Resources.Info, mtr + Properties.Resources.Deleted, Properties.Resources.Ok);
                    }
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Sensor))
                {
                    Sensor mtr = (Sensor)(obj as ItemHoldingEventArgs).ItemData;
                    string action = "";
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Drawing, Properties.Resources.Edit, Properties.Resources.Remove);
                    else
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Drawing);

                    if (action == Properties.Resources.Edit)
                    {
                        await Navigation.PushAsync(new SensorPage(mtr, true));
                    }
                    else if (action == Properties.Resources.Details)
                    {
                        await Navigation.PushAsync(new SensorPage(mtr));
                    }
                    else if (action == Properties.Resources.Drawing)
                    {
                        string doc = await getdrawingAsync(Equipment.Shop);
                        if (doc.Length > 0)
                            await Navigation.PushAsync(new DrawingPage(doc));
                    }
                    else if (action == Properties.Resources.Remove)
                    {
                        RemoveItem(mtr);
                        await page.DisplayAlert(Properties.Resources.Info, mtr + Properties.Resources.Deleted, Properties.Resources.Ok);
                    }
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(OtherComponent))
                {
                    OtherComponent mtr = (OtherComponent)(obj as ItemHoldingEventArgs).ItemData;
                    string action = "";
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Drawing, Properties.Resources.Edit, Properties.Resources.Remove);
                    else
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Drawing);

                    if (action == Properties.Resources.Edit)
                    {
                        await Navigation.PushAsync(new OtherComponentPage(mtr, true));
                    }
                    else if (action == Properties.Resources.Details)
                    {
                        await Navigation.PushAsync(new OtherComponentPage(mtr));
                    }
                    else if (action == Properties.Resources.Drawing)
                    {
                        string doc = await getdrawingAsync(Equipment.Shop);
                        if (doc.Length > 0)
                            await Navigation.PushAsync(new DrawingPage(doc));
                    }
                    else if (action == Properties.Resources.Remove)
                    {
                        RemoveItem(mtr);
                        await page.DisplayAlert(Properties.Resources.Info, mtr + Properties.Resources.Deleted, Properties.Resources.Ok);
                    }
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
        /// <typeparam name="T"></typeparam>
        public void RefreshItems<T>()
        {
            if (Motors != null)
                Motors.Clear();
            Motors = GetItems<Motor>();
            if (Sensors != null)
                Sensors.Clear();
            Sensors = GetItems<Sensor>();
        }
        async Task<string> getdrawingAsync(string shop)
        {
            try
            {
                // Get the path to a file on internal storage

                List<string> drawings;
                drawings = new List<string>();

                //from local folder
                List<string> draws = new List<string>();
                var folderPath = @"https://titanbu-tokvg.mongodbstitch.com/Drawings/";
                var files = Directory.EnumerateFiles(folderPath).Where(xx => xx.Contains(shop));
                if (files != null || files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        string str1 = file.Replace(folderPath, "");
                        str1 = str1.Replace(".pdf", "");
                        drawings.Add(str1);
                    }
                }
                var action = await page.DisplayActionSheet(Properties.Resources.Select, Properties.Resources.Cancel, null, drawings.ToArray());
                if (action == Properties.Resources.Cancel)
                    return "";
                else
                    return action;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("PlantBU", ex.Message, Properties.Resources.Ok);
                return null;
            }
        }
        async Task<string> getdrawingFromDropBoxAsync(string shop)
        {
            try
            {
                // Get the path to a file on internal storage

                List<string> drawings;
                drawings = new List<string>();

                //from local folder
                List<string> draws = new List<string>();
                var appFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var folderPath = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Drawings");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                var files = Directory.EnumerateFiles(folderPath).Where(xx => xx.Contains(shop));
                if (files == null || files.Count() == 0)
                {
                    var action1 = await page.DisplayActionSheet(Properties.Resources.DrawingNoMessage, Properties.Resources.Cancel, null, Properties.Resources.Ok);
                    string path;
                    if (action1 == Properties.Resources.Ok)
                    {
                        path = await SaveDropboxFiles(shop);
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Savedto + path, Properties.Resources.Ok);
                    }
                    else
                        return "";
                }
                else if (await CheckUpdateDropboxFiles(shop))
                {
                    var action1 = await page.DisplayActionSheet(Properties.Resources.DrawingUpdateMessage, Properties.Resources.Cancel, null, Properties.Resources.Ok);
                    string path;
                    if (action1 == Properties.Resources.Ok)
                    {
                        path = await SaveDropboxFiles(shop);
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Savedto + path, Properties.Resources.Ok);
                    }
                    else
                        return "";
                }
                else if (files != null || files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        string str1 = file.Replace(Xamarin.Essentials.FileSystem.AppDataDirectory + "\\Drawings\\", "");
                        str1 = str1.Replace(".pdf", "");
                        drawings.Add(str1);
                    }
                }
                var action = await page.DisplayActionSheet(Properties.Resources.Select, Properties.Resources.Cancel, null, drawings.ToArray());
                if (action == Properties.Resources.Cancel)
                    return "";
                else
                    return action;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("PlantBU", ex.Message, Properties.Resources.Ok);
                return null;
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.SaveDropboxFiles(string)'
        public async Task<string> SaveDropboxFiles(string fileName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.SaveDropboxFiles(string)'
        {
            try
            {
                DropBoxHelper dbox = new DropBoxHelper();
                //var files = await dbox.GetPdfFiles();
                var files = await GetUpdateDropboxFiles(fileName);
                List<Stream> st = new List<Stream>();
                if (files != null)
                    foreach (var fl in files)
                    {
                        if (fl.Contains(fileName))
                            st.Add(await dbox.DownloadPdfStream(fl));
                    }
                Stream pdfStream = st.First();
                var _rootDir = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Drawings");
                if (!Directory.Exists(_rootDir))
                    Directory.CreateDirectory(_rootDir);

                var filePath = Path.Combine(_rootDir, fileName + ".pdf");
                foreach (var strs in st)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await strs.CopyToAsync(memoryStream);
                        File.WriteAllBytes(filePath, memoryStream.ToArray());
                        memoryStream.Position = 0;
                        memoryStream.SetLength(0);
                    }
                }
                return filePath;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
                return null;
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.CheckUpdateDropboxFiles(string)'
        public async Task<bool> CheckUpdateDropboxFiles(string fileName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.CheckUpdateDropboxFiles(string)'
        {
            try
            {
                DropBoxHelper dbox = new DropBoxHelper();
                var _rootDir = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Drawings");
                if (!Directory.Exists(_rootDir))
                    Directory.CreateDirectory(_rootDir);

                var filesDbox = await dbox.GetPdfFiles();
                filesDbox = filesDbox.Where(x => x.Contains(fileName)).ToList();
                var DirfilesLocal = Directory.GetFiles(_rootDir);
                List<String> FilesLocal = new List<string>();
                if (DirfilesLocal != null)
                {
                    FilesLocal = DirfilesLocal.Where(x => x.Contains(fileName)).ToList();
                    if (filesDbox.Count != FilesLocal.Count)
                        return false;

                }
                return true;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
                return false;
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.GetUpdateDropboxFiles(string)'
        public async Task<List<string>> GetUpdateDropboxFiles(string fileName)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentViewModel.GetUpdateDropboxFiles(string)'
        {
            try
            {
                DropBoxHelper dbox = new DropBoxHelper();
                var _rootDir = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Drawings");
                if (!Directory.Exists(_rootDir))
                    Directory.CreateDirectory(_rootDir);

                var filesDbox = await dbox.GetPdfFiles();
                filesDbox = filesDbox.Where(x => x.Contains(fileName)).ToList();
                var DirfilesLocal = Directory.GetFiles(_rootDir);
                List<String> FilesLocal = new List<string>();
                if (DirfilesLocal != null)
                {
                    List<string> newlist = new List<string>();
                    FilesLocal = DirfilesLocal.Where(x => x.Contains(fileName)).ToList();
                    if (filesDbox.Count != FilesLocal.Count)
                        foreach (var fl in filesDbox)
                        {
                            if (!FilesLocal.Contains(fl))
                                newlist.Add(fl);
                        }
                    return newlist;
                }
                return null;
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
                return null;
            }
        }
    }
}
