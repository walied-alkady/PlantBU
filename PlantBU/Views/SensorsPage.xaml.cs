using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlantBU.DataModel;
using PlantBU.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SensorsPage'
    public partial class SensorsPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SensorsPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SensorsPage.SensorsPage()'
        public SensorsPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SensorsPage.SensorsPage()'
        {
            InitializeComponent();
            (BindingContext as SensorsViewModel).page = this;
            (BindingContext as SensorsViewModel).Navigation = Navigation;
        }
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (listView.DataSource != null)
            {
                this.listView.DataSource.Filter = FilterEquipments;
                this.listView.DataSource.RefreshFilter();
            }
        }
        private bool FilterEquipments(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var Sensor = obj as Sensor;
            double value; bool IsDouble = double.TryParse(searchBar.Text, out value);

            if (
                    (Sensor.Code != null ? Sensor.Code.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.Description != null ? Sensor.Description.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.Brand != null ? Sensor.Brand.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.BrandType != null ? Sensor.BrandType.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.InstrumentType != null ? Sensor.InstrumentType.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.SignalType != null ? Sensor.SignalType.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.SupplyVoltage != null ? Sensor.SupplyVoltage.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.Wiring != null ? Sensor.Wiring.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.Communication != null ? Sensor.Communication.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.GalvanicIsolation != null ? Sensor.GalvanicIsolation.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.OperationalUnit != null ? Sensor.OperationalUnit.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.ScaleRange != null ? Sensor.ScaleRange.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Sensor.ExtraData != null ? Sensor.ExtraData.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (IsDouble ? (value == Sensor.LimitHH ) : false)
                 || (IsDouble ? (value == Sensor.LimitH ) : false)
                 || (IsDouble ? (value == Sensor.LimitL ) : false)
                 || (IsDouble ? (value == Sensor.LimitLL ) : false)
                 )
                return true;
            else
                return false;
        }
    }
}