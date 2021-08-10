// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-19-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-02-2021
// ***********************************************************************
// <copyright file="EquipmentPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.ViewModels;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.XForms.ComboBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    /// <summary>
    /// Class EquipmentPage.
    /// Implements the <see cref="Xamarin.Forms.ContentPage" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EquipmentPage : ContentPage
    {
        CancellationTokenSource cts;

        /// <summary>
        /// Gets or sets the shop list.
        /// </summary>
        /// <value>The shop list.</value>
        public List<String> ShopList { get { return _ShopList; } set { _ShopList = value; OnPropertyChanged("ShopList"); } }
        /// <summary>
        /// The shop list
        /// </summary>
        List<string> _ShopList;
        /// <summary>
        /// Gets or sets the line list.
        /// </summary>
        /// <value>The line list.</value>
        public List<string> LineList { get { return _LineList; } set { _LineList = value; OnPropertyChanged("LineList"); } }
        /// <summary>
        /// The line list
        /// </summary>
        List<string> _LineList;
        /// <summary>
        /// Gets or sets the equipment.
        /// </summary>
        /// <value>The equipment.</value>
        Equipment Equipment { get { return _Equipment; } set { _Equipment = value; OnPropertyChanged("Equipment"); } }

        Equipment _Equipment;

        List<Motor> Motors { get { return _Motors; } set { _Motors = value; OnPropertyChanged("Motors"); } }

        List<Motor> _Motors;
        ToolbarItem AddMotor, AddSensor, AddOther;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentPage.EquipmentPage(Equipment, bool)'
        public EquipmentPage(Equipment eq = null, bool IsEnabled = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentPage.EquipmentPage(Equipment, bool)'
        {
            InitializeComponent();
            try
            {
                LineList = DBManager.realm.All<ProductionLine>().ToList()
                      .Select(l => l.Code).Distinct().ToList();
                ShopList = DBManager.realm.All<Shop>().ToList()
                    .Select(l => l.ShopName).ToList();
                (BindingContext as EquipmentViewModel).Navigation = Navigation;
                (BindingContext as EquipmentViewModel).page = this;
                (BindingContext as EquipmentViewModel).Equipment = eq;
                (BindingContext as EquipmentViewModel).EquipmentLine = eq.ProductionLineBackRef.FirstOrDefault()?.Code;
                ShopcomboBox.Text = eq.Shop;
                LinecomboBox.Text = eq.ProductionLineBackRef.FirstOrDefault()?.Code;
                (BindingContext as EquipmentViewModel).Motors = eq.Motors.ToList();
                (BindingContext as EquipmentViewModel).Sensors = eq.Sensors.ToList();
                (BindingContext as EquipmentViewModel).OtherComponent = eq.OtherComponents.ToList();
                (BindingContext as EquipmentViewModel).IsEnabled = IsEnabled;
                if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                {
                    AddMotor = new ToolbarItem()
                    {
                        Text = Properties.Resources.AddMotor,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId = "AddMotor"
                    };
                    AddMotor.Clicked += ToolbarItem_Clicked;
                    
                    AddSensor = new ToolbarItem()
                    {
                        Text = Properties.Resources.AddSensor,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId = "AddSensor"
                    };
                    AddOther = new ToolbarItem()
                    {
                        Text = Properties.Resources.AddOther,
                        Order = ToolbarItemOrder.Secondary,
                        Priority = 2,
                        StyleId = "AddOther"
                    };
                    AddOther.Clicked += ToolbarItem_Clicked;
                    this.ToolbarItems.Add(AddMotor);
                    this.ToolbarItems.Add(AddSensor);
                    this.ToolbarItems.Add(AddOther);

                }

            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EquipmentPage.OnDisappearing()'
        protected override void OnDisappearing()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EquipmentPage.OnDisappearing()'
        {
            if (cts != null && !cts.IsCancellationRequested)
                cts.Cancel();
            base.OnDisappearing();
        }
        private void Editor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.Equipment = (BindingContext as EquipmentViewModel).Equipment;
                DBManager.realm.Write(async () =>
                {
                    if (sender is Entry)
                    {
                        switch ((sender as Entry).StyleId)
                        {
                            case "CodeEntry":
                                Equipment.Code = (sender as Entry).Text;
                                break;
                            case "DescriptionEntry":
                                Equipment.Description = (sender as Entry).Text;
                                break;
                            case "TypeEntry":
                                Equipment.Type = (sender as Entry).Text;
                                break;
                            case "BrandEntry":
                                Equipment.Brand = (sender as Entry).Text;
                                break;
                            case "BrandTypeEntry":
                                Equipment.BrandType = (sender as Entry).Text;
                                break;
                            case "ExtraDataEntry":
                                Equipment.Extra = (sender as Entry).Text;
                                break;
                            case "ShopExtraEntry":
                                Equipment.ShopExtra = (sender as Entry).Text;
                                break;
                        }
                    }
                    else if (sender is SfComboBox)
                    {
                        switch ((sender as SfComboBox).StyleId)
                        {
                            case "LinecomboBox":
                                var line = DBManager.realm.All<ProductionLine>().Where(ln => ln.Code == (sender as SfComboBox).Text);
                                if (line.Any())
                                {
                                    Equipment newEq = Equipment;
                                    line.First().EquipmentAdd(newEq);
                                    (BindingContext as EquipmentViewModel).Equipment = newEq;

                                    var prs = DBManager.realm.All<ProductionLine>();
                                    foreach (ProductionLine pr in prs)
                                    {
                                        pr.Equipments.ToList().Contains(Equipment);
                                        pr.EquipmentDelete(Equipment);
                                    }
                                }
                                else
                                    await DisplayAlert("Error", Properties.Resources.Invalide + Properties.Resources.ProductionLine, Properties.Resources.Ok);
                                break;
                            case "ShopcomboBox":
                                var shp = DBManager.realm.All<Shop>().Where(sh => sh.ShopName == (sender as SfComboBox).Text);
                                if (shp.Any())
                                {
                                    Equipment.Shop = (sender as SfComboBox).Text;
                                }
                                else
                                    await DisplayAlert("Error", Properties.Resources.Invalide + Properties.Resources.Shop, Properties.Resources.Ok);
                                break;

                        }
                    }
                   
                });
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
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
                            case "AddMotor":
                                Motor newMtr = new Motor()
                                {
                                    Code = "New Motor",
                                };
                                Equipment.ComponentAdd(newMtr);
                                await Navigation.PushAsync(new MotorPage(newMtr, true));
                                break;
                            case "AddSensor":
                                Sensor newSens = new Sensor()
                                {
                                    Code = "New Sensor",
                                };
                                Equipment.ComponentAdd(newSens);
                                await Navigation.PushAsync(new SensorPage(newSens, true));
                                break;
                            case "AddOther":
                                OtherComponent newOtherComponent = new OtherComponent()
                                {
                                    Code = "New Component",
                                };
                                Equipment.ComponentAdd(newOtherComponent);
                                await Navigation.PushAsync(new OtherComponentPage(newOtherComponent, true));
                                break;
                            case "EqLocation":
                                await SetEquipmentLocation();
                                break;
                        }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async Task SetEquipmentLocation()
        {
           
                try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Xamarin.Essentials.Geolocation.GetLocationAsync(request, cts.Token);
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                await DBManager.realm.Write(async () =>
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
                {
                if (location != null)
                {
                        Equipment.LocationAltitude=  location.Altitude.Value;
                        Equipment.LocationLongitude = location.Longitude;
                        Equipment.LocationLatitude = location.Latitude;
                    }
                });                
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
            
        }
    }
}