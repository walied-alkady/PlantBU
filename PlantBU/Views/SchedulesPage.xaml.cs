// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-07-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-08-2021
// ***********************************************************************
// <copyright file="SchedulesPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.ViewModels;
using Syncfusion.ListView.XForms;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SchedulesPage'
    public partial class SchedulesPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SchedulesPage'
    {
        ToolbarItem LoadSchedule, LoadScheduleSpares, ClearAllSchedule, ClearDoneSchedule, SaveSchedule;
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulesPage"/> class.
        /// </summary>
        public SchedulesPage()
        {
            InitializeComponent();
            try
            {
               

                if (Device.RuntimePlatform == Device.UWP)
                {
                    LoadSchedule= new ToolbarItem()
                    {
                        Text = Properties.Resources.LoadScheduleFile,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId= "LoadSchedule"
                    };
                    LoadSchedule.Clicked += ToolbarItem_Clicked;
                    LoadScheduleSpares = new ToolbarItem()
                    {
                        Text = Properties.Resources.LoadScheduleSpares,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId= "LoadScheduleSpares"
                    };
                    LoadScheduleSpares.Clicked += ToolbarItem_Clicked;
                    ClearDoneSchedule = new ToolbarItem()
                    {
                        Text = Properties.Resources.ClearDone,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId= "ClearDoneSchedule"
                    };
                    ClearDoneSchedule.Clicked += ToolbarItem_Clicked;
                    ClearAllSchedule = new ToolbarItem()
                    {
                        Text = Properties.Resources.ClearAll,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId = "ClearAllSchedule"
                    };
                    ClearAllSchedule.Clicked += ToolbarItem_Clicked;
                    SaveSchedule = new ToolbarItem()
                    {
                        Text = Properties.Resources.SaveSchedule,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId= "SaveSchedule"
                    };
                    SaveSchedule.Clicked += ToolbarItem_Clicked;
                    // "this" refers to a Page object
                    this.ToolbarItems.Add(LoadSchedule); 
                    this.ToolbarItems.Add(LoadScheduleSpares); 
                    this.ToolbarItems.Add(ClearDoneSchedule);
                    this.ToolbarItems.Add(ClearAllSchedule);
                    this.ToolbarItems.Add(SaveSchedule);
                }
                (BindingContext as SchedulesViewModel).Navigation = Navigation;
                (BindingContext as SchedulesViewModel).page = this;
                 if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                     (BindingContext as SchedulesViewModel).IsAdmin = true;
                 else
                     (BindingContext as SchedulesViewModel).IsAdmin = false;
                Preferences.Set("NewSchedule", 0);
                Preferences.Set("NewSchedulesCount", 0);
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (listView.DataSource != null)
            {
                this.listView.DataSource.Filter = FilterLogs;
                this.listView.DataSource.RefreshFilter();
            }
        }
        private bool FilterLogs(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;
            var schedule = obj as Schedule;
            var emp = DBManager.realm.All<Employee>().Where(x => x.CompanyCode == schedule.AssigneeCompanyCode).FirstOrDefault();
            var empDone = DBManager.realm.All<Employee>().Where(x => x.CompanyCode == schedule.AssigneeDoneCompanyCode).FirstOrDefault();

            if (
                    (schedule.ItemCode != null ? schedule.ItemCode.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.ItemDescription != null ? schedule.ItemDescription.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.Repair != null ? schedule.Repair.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.Repairdetails != null ? schedule.Repairdetails.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.Notes != null ? schedule.Notes.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (emp != null ? emp.FirstName.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (emp != null ? emp.LastName.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (empDone != null ? empDone.FirstName.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (empDone != null ? empDone.LastName.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.DateScheduleFrom != null ? schedule.DateScheduleFrom.Month.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.DateScheduleFrom != null ? schedule.DateScheduleFrom.Day.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.DateScheduleFrom != null ? schedule.DateScheduleFrom.Year.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.DateScheduleTo != null ? schedule.DateScheduleTo.Month.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.DateScheduleTo != null ? schedule.DateScheduleTo.Day.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (schedule.DateScheduleTo != null ? schedule.DateScheduleTo.Year.ToString().ToLower().Contains(searchBar.Text.ToLower()) : false))
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
                    Plant pl = DBManager.realm.All<Plant>().First();
                    Schedule newSchedule = null;
                    Equipment newEq = null;
                    switch ((sender as ToolbarItem).StyleId)
                        {
                            case "Add":
                            newSchedule = new Schedule()
                                {
                                    ItemDescription = "New Item Details",
                                    Repair = "New Schedule",
                                    DateScheduleFrom = DateTime.Now.Date.ToLocalTime(),
                                    DateScheduleTo = DateTime.Now.Date.AddDays(1).ToLocalTime(),
                                    SetDate = DateTime.Now.Date.ToLocalTime()
                                };
                                pl.ScheduleAdd(newSchedule);
                                (BindingContext as SchedulesViewModel).RefreshItems();
                                await Navigation.PushAsync(new SchedulePage(newSchedule, true));
                                break;
                        case "LoadSchedule":
                            var options = new PickOptions
                            {
                                PickerTitle = Properties.Resources.CSVFileMessageSelect,
                               // FileTypes = customFileType,
                            };
                            await PickAndShowSchedule(options);
                            break;
                        case "LoadScheduleSpares":
                            var options1 = new PickOptions
                            {
                                PickerTitle = Properties.Resources.CSVFileMessageSelect,
                            };
                            await PickAndShowScheduleSpares(options1);
                            break;
                        case "ClearAllSchedule":
                            DBManager.realm.Write(() =>
                            {
                                DBManager.realm.RemoveAll<Schedule>();
                            });
                            break;
                        case "ClearDoneSchedule":
                            List<Schedule> CompleteSch = DBManager.realm.All<Schedule>().Where(x => x.StatusSchedule == true).ToList();
                            if (CompleteSch != null && CompleteSch.Count > 0)
                            {
                                foreach (var xx in CompleteSch)
                                {
                                    var itemCodeEq = DBManager.realm.All<Equipment>().Where(x => x.Code == xx.ItemCode.Substring(0, 7)).FirstOrDefault();
                                    /* var itemCodeM = DBManager.realm.All<Motor>().Where(x => x.Code == xx.ItemCode).FirstOrDefault();
                                     var itemCodeS = DBManager.realm.All<Sensor>().Where(x => x.Code == xx.ItemCode).FirstOrDefault();
                                     var itemCodeO = DBManager.realm.All<OtherComponent>().Where(x => x.Code == xx.ItemCode).FirstOrDefault();*/

                                    var itemCodeComp = itemCodeEq.ComponentGet(xx.ItemCode);
                                    if (itemCodeEq == null)
                                    {
                                        bool AddEquip = await DisplayAlert(Properties.Resources.Info, Properties.Resources.Equipment + Properties.Resources.NotAvailable, Properties.Resources.Add, Properties.Resources.Cancel);
                                        if (AddEquip)
                                        {
                                            var proLines = DBManager.realm.All<ProductionLine>().ToList().Select(x => x.Code).ToArray();
                                            var action = await DisplayActionSheet(Properties.Resources.Select, Properties.Resources.Cancel, null, proLines);
                                            if (string.IsNullOrEmpty(action))
                                                continue;
                                            var proLine = DBManager.realm.All<ProductionLine>().Where(x => x.Code == action).First();
                                            DBManager.realm.Write(() =>
                                            {
                                                newEq = new Equipment() { Code = xx.ItemCode.Substring(0, 7), Description = xx.ItemDescription };
                                            });
                                            var actionComp = await DisplayActionSheet(Properties.Resources.SelectComponent, Properties.Resources.Cancel, null, Properties.Resources.Motor, Properties.Resources.Sensor, Properties.Resources.OtherComponent);
                                            if (actionComp == Properties.Resources.Motor)
                                            {
                                                Motor mt = new Motor() { Code = xx.ItemCode, Description = xx.ItemDescription };
                                                foreach (var spp in xx.SpareParts)
                                                {
                                                    mt.SparePartAdd(new SparePart() { InventoryCode = spp.InventoryCode, Description1 = spp.Description1, Description2 = spp.Description2, QtyRequired = spp.QtyRequired });
                                                }
                                                newEq.ComponentAdd(mt);
                                            }
                                            else if (actionComp == Properties.Resources.Sensor)
                                            {
                                                Sensor mt = new Sensor() { Code = xx.ItemCode, Description = xx.ItemDescription };
                                                foreach (var spp in xx.SpareParts)
                                                {
                                                    mt.SparePartAdd(new SparePart() { InventoryCode = spp.InventoryCode, Description1 = spp.Description1, Description2 = spp.Description2, QtyRequired = spp.QtyRequired });
                                                }
                                                newEq.ComponentAdd(mt);
                                            }
                                            else if (actionComp == Properties.Resources.OtherComponent)
                                            {
                                                OtherComponent mt = new OtherComponent() { Code = xx.ItemCode, Description = xx.ItemDescription };
                                                foreach (var spp in xx.SpareParts)
                                                {
                                                    mt.SparePartAdd(new SparePart() { InventoryCode = spp.InventoryCode, Description1 = spp.Description1, Description2 = spp.Description2, QtyRequired = spp.QtyRequired });
                                                }
                                                newEq.ComponentAdd(mt);
                                            }
                                            proLine.EquipmentAdd(newEq);
                                            // await Navigation.PushAsync(new EquipmentPage(newEq, true));

                                        }
                                    }
                                    else if (itemCodeComp == null)
                                    {
                                        var actionComp = await DisplayActionSheet(Properties.Resources.SelectComponent, Properties.Resources.Cancel, null, Properties.Resources.Motor, Properties.Resources.Sensor, Properties.Resources.OtherComponent);
                                        if (actionComp == Properties.Resources.Motor)
                                        {
                                            Motor mt = new Motor() { Code = xx.ItemCode, Description = xx.ItemDescription };
                                            foreach (var spp in xx.SpareParts)
                                            {
                                                mt.SparePartAdd(new SparePart() { InventoryCode = spp.InventoryCode, Description1 = spp.Description1, Description2 = spp.Description2, QtyRequired = spp.QtyRequired });
                                            }
                                            itemCodeEq.ComponentAdd(mt);
                                        }
                                        else if (actionComp == Properties.Resources.Sensor)
                                        {
                                            Sensor mt = new Sensor() { Code = xx.ItemCode, Description = xx.ItemDescription };
                                            foreach (var spp in xx.SpareParts)
                                            {
                                                mt.SparePartAdd(new SparePart() { InventoryCode = spp.InventoryCode, Description1 = spp.Description1, Description2 = spp.Description2, QtyRequired = spp.QtyRequired });
                                            }
                                            itemCodeEq.ComponentAdd(mt);
                                        }
                                        else if (actionComp == Properties.Resources.OtherComponent)
                                        {
                                            OtherComponent mt = new OtherComponent() { Code = xx.ItemCode, Description = xx.ItemDescription };
                                            foreach (var spp in xx.SpareParts)
                                            {
                                                mt.SparePartAdd(new SparePart() { InventoryCode = spp.InventoryCode, Description1 = spp.Description1, Description2 = spp.Description2, QtyRequired = spp.QtyRequired });
                                            }
                                            itemCodeEq.ComponentAdd(mt);
                                        }
                                    }
                                    Log lg;
                                    lg = new Log()
                                    {
                                        ItemCode = xx.ItemCode,
                                        ItemDescription = xx.ItemDescription,
                                        DateLog = DateTimeOffset.Now,
                                        Repair = xx.Repair,
                                        Repairdetails = xx.Repairdetails,
                                        Notes = xx.Notes,
                                        AssigneeCompanyCode = DBManager.CurrentUser.CompanyCode,
                                        Cost = xx.RepairCost + xx.SparesCost
                                    };
                                    pl.LogAdd(lg);
                                    if (xx.RepairCost > 0)
                                    {
                                        var ex = new ExpensItem()
                                        {
                                            Title = xx.Repair,
                                            Description = xx.ItemCode + xx.ItemDescription + xx.Repairdetails,
                                            DateExpense = DateTime.Now,
                                            Value = xx.RepairCost
                                        };
                                        pl.ExpenseAdd(ex);
                                    }
                                    if (xx.SparesCost > 0)
                                    {
                                        var ex = new ExpensItem()
                                        {
                                            Title = xx.Repair,
                                            Description = xx.ItemCode + xx.ItemDescription + xx.Repairdetails,
                                            DateExpense = DateTime.Now,
                                            Value = xx.SparesCost
                                        };
                                        pl.ExpenseAdd(ex);
                                    }
                                    //pl.ScheduleDelete(xx);
                                }
                            }
                            await DBManager.realm.WriteAsync(realm =>
                            {
                                realm.RemoveAll<Schedule>();
                            });
                            break;
                        case "SchedulePerformace":
                            await Navigation.PushAsync(new EmployeeSchedulePerformancePage());
                            break;
                        case "SaveSchedule":
                            ShareXlsData();
                            break;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        async Task<FileResult> PickAndShowSchedule(PickOptions options)
        {
            try
            {
                IsBusy = true;
                var result = await FilePicker.PickAsync(options);
                IProgress<int> progress = new Progress<int>(ReportProgress);

                if (result != null)
                {
                    if (result.FileName.EndsWith("csv", StringComparison.OrdinalIgnoreCase))
                    {
                        //result.FileName;
                        var stream = await result.OpenReadAsync();
                       
                        await DBManager.realm.WriteAsync(realm =>
                        {
                            var schedulesImports = CSVs.GetStreamData<Schedule>(stream);
                            Plant pl = realm.All<Plant>().First();
                            foreach (Schedule sps in schedulesImports)
                                pl.Schedules.Add(sps);
                        });
                        stream.Close();
                    }
                    else
                        await DisplayAlert(Properties.Resources.Info, Properties.Resources.CSVFileMessageError, Properties.Resources.Ok);
                }
               
                IsBusy = false;
                return result;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
                IsBusy = false; 
            }

            return null;
        }
        async Task<FileResult> PickAndShowScheduleSpares(PickOptions options)
        {
            try
            {
                IsBusy = true;
                var result = await FilePicker.PickAsync(options);
                IProgress<int> progress = new Progress<int>(ReportProgress);

                if (result != null)
                {
                    if (result.FileName.EndsWith("csv", StringComparison.OrdinalIgnoreCase))
                    {
                        //result.FileName;
                        var stream = await result.OpenReadAsync();

                        await DBManager.realm.WriteAsync(realm =>
                        {
                            var schedulesImports = CSVs.GetStreamData<ScheduleSparePartImport>(stream);
                            var pl = realm.All<Schedule>().ToList();
                            
                            foreach (Schedule sps in pl)
                            {
                                var partsList = schedulesImports.Where(x => x.CodeItem == sps.ItemCode)?.ToList();
                                foreach(var part in partsList)
                                {
                                    if (realm.All<Spare>().Where(x => x.Code == part.InventoryCode).FirstOrDefault() != null)
                                    {
                                        Spare sp = realm.All<Spare>().Where(x => x.Code == part.InventoryCode).First();
                                        sps.SpareParts.Add(new ScheduleSparePart()
                                        {
                                            InventoryCode = part.InventoryCode,
                                            Description1 = sp.Description1,
                                            Description2 = sp.Description2,
                                            QtyRequired = part.QtyRequired,
                                            Value = sp.Value,
                                             Partition = "Public"
                                        }); 
                                    }
                                }
                                
                            }
                        });
                        stream.Close();
                    }
                    else
                        await DisplayAlert(Properties.Resources.Info, Properties.Resources.CSVFileMessageError, Properties.Resources.Ok);
                }

                IsBusy = false;
                return result;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
                IsBusy = false;
            }

            return null;
        }
        private void listView_Loaded(object sender, ListViewLoadedEventArgs e)
        {
            listView.CollapseAll();
        }
        void ReportProgress(int value)
        {
            //Update the UI to reflect the progress value that is passed back.
            /*CustomContentProgressBarLabel.Text = string.Format("{0:00} %", value);
            CircularProgressBar.Progress = value;*/

        }
        async void ShareXlsData()
        {
            try
            {
                (BindingContext as SchedulesViewModel).IsBusy = true;
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2013;

                    //"App" is the class of Portable project
                    Assembly assembly = typeof(App).GetTypeInfo().Assembly;
                    Stream inputStream = assembly.GetManifestResourceStream("PlantBU.Schedules.xlsx");
                    application.DefaultVersion = ExcelVersion.Excel2013;
                    IWorkbook workbook = application.Workbooks.Open(inputStream);
                    IWorksheet sheet = workbook.Worksheets[0];

                    var schedules = DBManager.realm.All<Schedule>().ToList();
                    //Adding Header 
                    int c = 2;
                    foreach (var sc in schedules)
                    {
                        sheet.Range[c, 1].Text = sc.Id.ToString(); 
                        sheet.Range[c, 2].Text = sc.Partition;
                        sheet.Range[c, 3].Text = sc.SetDate.ToString();
                        sheet.Range[c, 4].Text = sc.Area;
                        sheet.Range[c, 5].Text = sc.ItemCode;
                        sheet.Range[c, 6].Text = sc.ItemDescription;
                        sheet.Range[c, 7].Text = sc.Repair;
                        sheet.Range[c, 8].Text = sc.Repairdetails;
                        sheet.Range[c, 9].Text = sc.RepairCost.ToString();
                        sheet.Range[c, 10].Text = sc.Notes;
                        sheet.Range[c, 11].Text = sc.DateScheduleFrom.ToString();
                        sheet.Range[c, 12].Text = sc.DateScheduleTo.ToString();
                        sheet.Range[c, 13].Text = sc.StatusSchedule.ToString();
                        sheet.Range[c, 14].Text = sc.AssigneeCompanyCode;
                        sheet.Range[c, 15].Text = sc.AssigneeDoneCompanyCode;
                        c++;
                    }
                    
                    /* //Access a range by specifying cell row and column index
                     sheet.Range[9, 1].Text = "Accessing a Range by specify cell row and column index ";

                     //Access a Range by specifying using defined name
                     IName name = workbook.Names.Add("Name");
                     name.RefersToRange = sheet.Range["A11"];
                     sheet.Range["Name"].Text = "Accessing a Range by specifying using defined name";

                     //Accessing a Range of cells by specifying cells address
                     sheet.Range["A13:C13"].Text = "Accessing a Range of Cells (Method 1)";

                     //Accessing a Range of cells specifying cell row and column index
                     sheet.Range[15, 1, 15, 3].Text = "Accessing a Range of Cells (Method 2)";*/
                    workbook.Version = ExcelVersion.Excel2013;
                    //Saving the workbook as stream
                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream);

                    //Save the stream into XLSX file
                    List<string> Paths = new List<string>();
                    string path = await Xamarin.Forms.DependencyService.Get<ISave>()
                        .SaveAndView("Schedule_" +
                        DateTime.Now.ToLocalTime().Date.Day +
                        DateTime.Now.ToLocalTime().Date.Month +
                        DateTime.Now.ToLocalTime().Date.Year +
                        ".xlsx", "application/msexcel", stream,true);                  
                    stream.Position = 0;
                    workbook.Close();
                    excelEngine.Dispose();
                    (BindingContext as SchedulesViewModel).IsBusy = true;

                }
            (BindingContext as SchedulesViewModel).IsBusy = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
                (BindingContext as SchedulesViewModel).IsBusy = true;
            }
           
        }
    }
}

