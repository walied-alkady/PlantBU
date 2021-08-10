// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-19-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-06-2021
// ***********************************************************************
// <copyright file="MotorViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.Views;
using Syncfusion.ListView.XForms;
using Syncfusion.SfAutoComplete.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class MotorViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    public class SensorViewModel : BaseViewModel
    {
       
        public Sensor Sensor { get { return _Sensor; } set { _Sensor = value; OnPropertyChanged("Sensor"); } }
        
        private Sensor _Sensor;
        
        public List<String> CodesList { get { var x = DBManager.realm.All<Sensor>().ToList().Select(x1 => x1.Code).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }
        public List<String> BrandsList { get { var x = DBManager.realm.All<Sensor>().ToList().Select(x1 => x1.Brand).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }
        public List<String> BrandsTypesList { get { var x = DBManager.realm.All<Sensor>().ToList().Select(x1 => x1.BrandType).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }
        public List<String> SoftwareList { get { var x = DBManager.realm.All<Sensor>().ToList().Select(x1 => x1.Softawre).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }
        public List<String> WiringList { get; set; } = new List<string>() { "2-wire", "4-wire", "Current Loop", "Other" };
        public List<String> OperationalUnitList { get { var x = DBManager.realm.All<Sensor>().ToList().Select(x1 => x1.OperationalUnit).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }
        public List<String> InstrumentList { get; set; } = new List<string>(){ "Thermometer",
"Pressure Transmitter",
"Flowmeter",
"Weigh Sensor",
"Vibro Sensor",
"Positioner",
"Level Sensor",
"Speed/RPM",
"Other"
 };
        public List<String> CommunicationList { get; set; } = new List<string>(){ "ProfiBUS",
"Ethernet/ProfiNET",
"Can-BUS",
"ModBUS",
"RS232",
"RS422",
"RS485",
"Other",
"-"
 };
        public List<String> SignalTypeList { get; set; } = new List<string>() { "0…10V", "2…10V", "0…20mA", "4…20mA", "Other" };
        public List<String> SupplyVoltageList { get; set; } = new List<string>(){"24V",
"48V",
"110V",
"230V",
"380V",
"400V",
"Other"

 };
        public List<String> GalvanicIsolatorList { get; set; } = new List<string>(){"Internal",
"External",
"Other"

 };

        
        public Command<object> ListViewHoldingCommand { get; private set; }
        

        
        public SensorViewModel()
        {

            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
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
       
        
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(SparePart))
                {

                    SparePart eq = (SparePart)(obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {
                        string action = await page.DisplayActionSheet(eq.InventoryCode + "\r" + eq.Description1, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Remove);

                        if (action == Properties.Resources.Details)
                        {
                            var spareinv = DBManager.realm.All<Spare>().Where(x => x.Code == eq.InventoryCode).First();
                            if (spareinv != null)
                                await Navigation.PushAsync(new SparePage(spareinv));
                            else
                            {
                                await page.DisplayAlert(Properties.Resources.Info, eq.InventoryCode + Properties.Resources.NotFound, Properties.Resources.Ok);
                            }
                        }

                        else if (action == Properties.Resources.Remove)
                        {
                            RemoveItem<SparePart>(eq);
                            (obj as ItemHoldingEventArgs).Handled = true;
                            await page.DisplayAlert(Properties.Resources.Info, eq.InventoryCode + Properties.Resources.Deleted, Properties.Resources.Ok);
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Equipment + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Sensor))
                {
                    Sensor mtr = (Sensor)(obj as ItemHoldingEventArgs).ItemData;
                    string action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Cancel, Properties.Resources.Edit, Properties.Resources.Remove);

                    if (action == Properties.Resources.Edit)
                    {
                        await Navigation.PushAsync(new SensorPage(mtr, true));
                    }
                    else if (action == Properties.Resources.Details)
                    {
                        await Navigation.PushAsync(new SensorPage(mtr));
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
    }
    

}
