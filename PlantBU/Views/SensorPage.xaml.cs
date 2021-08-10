using PlantBU.DataModel;
using PlantBU.ViewModels;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.XForms.ComboBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SensorPage'
    public partial class SensorPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SensorPage'
    {
        Sensor Sensor { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SensorPage.SensorPage(Sensor, bool)'
        public SensorPage(Sensor mtr = null, bool IsEnabled = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SensorPage.SensorPage(Sensor, bool)'
        {
           
            try
            {
                InitializeComponent();
                (BindingContext as SensorViewModel).page = this;
                (BindingContext as SensorViewModel).Navigation = Navigation;
                (BindingContext as SensorViewModel).Sensor = mtr;
                (BindingContext as SensorViewModel).IsEnabled = IsEnabled;
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void Editor_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Sensor = (BindingContext as SensorViewModel).Sensor;
                DBManager.realm.Write(() =>
                {
                    if (sender is SfAutoComplete && !string.IsNullOrEmpty((sender as SfAutoComplete).Text))
                    {
                        switch ((sender as SfAutoComplete).StyleId)
                        {
                            case "CodeAutoComplete":
                                Sensor.Code = (sender as SfAutoComplete).Text;
                                break;
                            case "DescriptionAutoComplete":
                                Sensor.Description = (sender as SfAutoComplete).Text;
                                break;
                            case "BrandAutoComplete":
                                Sensor.Brand = (sender as SfAutoComplete).Text;
                                break;
                            case "BrandTypeAutoComplete":
                                Sensor.BrandType = (sender as SfAutoComplete).Text;
                                break;
                            case "OperationUnitAutoComplete":
                                Sensor.OperationalUnit = (sender as SfAutoComplete).Text;
                                break;
                            case "SoftwareAutoComplete":
                                Sensor.Softawre = (sender as SfAutoComplete).Text;
                                break;
                         

                        }
                    }
                    else if (sender is SfComboBox && !string.IsNullOrEmpty((sender as SfComboBox).Text))
                    {
                        switch ((sender as SfComboBox).StyleId)
                        {
                            case "InstrumentcomboBox":
                                Sensor.InstrumentType = (sender as SfComboBox).Text;
                                break;
                            case "SignalcomboBox":
                                Sensor.SignalType = (sender as SfComboBox).Text;
                                break;
                            case "voltagecomboBox":
                                Sensor.SupplyVoltage = (sender as SfComboBox).Text;
                                break;
                            case "WiringcomboBox":
                                Sensor.Wiring = (sender as SfComboBox).Text;
                                break;
                            case "CommunicationcomboBox":
                                Sensor.Communication = (sender as SfComboBox).Text;
                                break;
                            case "GalvanicIsolationcomboBox":
                                Sensor.GalvanicIsolation = (sender as SfComboBox).Text;
                                break;
                           
                        }
                    }
                    else if (sender is Editor && !string.IsNullOrEmpty((sender as Editor).Text))
                    {
                        switch ((sender as Editor).StyleId)
                        {
                            case "OperationLimitHHEditor":
                                Sensor.LimitHH = double.Parse((sender as Editor).Text);
                                break;
                            case "OperationLimitHEditor":
                                Sensor.LimitH = double.Parse((sender as Editor).Text);
                                break;
                            case "OperationLimitLEditor":
                                Sensor.LimitL = double.Parse((sender as Editor).Text);
                                break;
                            case "OperationLimitLLEditor":
                                Sensor.LimitLL = double.Parse((sender as Editor).Text);
                                break;
                            case "ExtraDataEditor":
                                Sensor.ExtraData = (sender as Editor).Text;
                                break;
                        }
                                
                    }
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            Sensor = (BindingContext as SensorViewModel).Sensor;
            if (InventoryAutoComplete.SelectedItem != null)
                DBManager.realm.Write(() =>
                {
                    Sensor.SpareParts.Add(new SparePart()
                    {
                        InventoryCode = (InventoryAutoComplete.SelectedItem as Spareitem).code,
                        Description1 = (InventoryAutoComplete.SelectedItem as Spareitem).description1,
                        Description2 = (InventoryAutoComplete.SelectedItem as Spareitem).description2
                    });
                    // Spare xpare = DBManager.realm.All<Spare>().Where(x => x.Code == (_sfAuto.SelectedItem as Spareitem).code).First();
                });
        }
    }
}