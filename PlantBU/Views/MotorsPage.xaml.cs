using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MotorsPage'
    public partial class MotorsPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MotorsPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'MotorsPage.MotorsPage()'
        public MotorsPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'MotorsPage.MotorsPage()'
        {
            InitializeComponent();
            (BindingContext as MotorsViewModel).page = this;
            (BindingContext as MotorsViewModel).Navigation = Navigation;
        }
        private void OnFilterTextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (searchBar.Text.StartsWith("s:"))
            {
                this.listView.DataSource.Filter = FilterSimilarEquipments;
                this.listView.DataSource.RefreshFilter();
            }
            else if (listView.DataSource != null)
            {
                this.listView.DataSource.Filter = FilterEquipments;
                this.listView.DataSource.RefreshFilter();
            }
        }
        private bool FilterEquipments(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var Motor = obj as Motor;
            double value; bool IsDouble = double.TryParse(Regex.Match(searchBar.Text, @"([-+]?[0-9]*\.?[0-9]+)").Value, out value);
            if (IsDouble && searchBar.Text.ToLower().EndsWith("kw") && Math.Abs((Motor.Power - value) / Motor.Power) <= 0.05)
                return true;
            else if (IsDouble && searchBar.Text.ToLower().EndsWith("rpm") && Math.Abs((Motor.RPM - value) / Motor.RPM) <= 0.05)
                return true;
            else if (IsDouble && searchBar.Text.ToLower().EndsWith("a") && Math.Abs((Motor.Current - value) / Motor.Current) <= 0.05)
                return true;
            else if (IsDouble && searchBar.Text.ToLower().EndsWith("kg") && Math.Abs((Motor.Weight - value) / Motor.Weight) <= 0.05)
                return true;
            else if (IsDouble && searchBar.Text.ToLower().EndsWith("v") && (value == Motor.Volt) )
                return true;
            else if (IsDouble && searchBar.Text.ToLower().EndsWith("p") && (value == Motor.Poles))
                return true;
            else if (
                    (Motor.Code != null ? Motor.Code.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.Description != null ? Motor.Description.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.Brand != null ? Motor.Brand.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.BrandType != null ? Motor.BrandType.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.Mounting != null ? Motor.Mounting.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.FrameSize != null ? Motor.FrameSize.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.Cooling != null ? Motor.Cooling.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.Duty != null ? Motor.Duty.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.ThermalClass != null ? Motor.ThermalClass.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.BearingDE != null ? Motor.BearingDE.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.BearingNDE != null ? Motor.BearingNDE.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 )
                return true;
            else
                return false;
        }
        private bool FilterSimilarEquipments(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;
            var Motor = obj as Motor;
            double value; bool IsDouble = double.TryParse(searchBar.Text, out value);
            if (
                 ((Motor.Brand != null ? Motor.Brand.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 && (Motor.BrandType != null ? Motor.BrandType.ToLower().Contains(searchBar.Text.ToLower()) : false))
                 || ((Motor.FrameSize != null ? Motor.Mounting.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (Motor.BearingDE != null ? Motor.BearingDE.ToLower().Contains(searchBar.Text.ToLower()) : false)
                 || (IsDouble ? (Motor.Power - value / value) <= 0.05 && (Motor.Power + value / value) >= 1.05 : false)
                 || (IsDouble ? (Motor.RPM - value / value) <= 0.05 && (Motor.RPM + value / value) >= 1.05 : false)
                 ))
                return true;
            else
                return false;
        }
       
    }
}

