using PlantBU.DataModel;
using PlantBU.ViewModels;
using Syncfusion.SfAutoComplete.XForms;
using Syncfusion.XForms.ComboBox;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentPage'
    public partial class OtherComponentPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentPage'
    {
        OtherComponent OtherComponent { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentPage.OtherComponentPage(OtherComponent, bool)'
        public OtherComponentPage(OtherComponent mtr = null, bool IsEnabled = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'OtherComponentPage.OtherComponentPage(OtherComponent, bool)'
        {
            InitializeComponent();
            try
            {
                (BindingContext as OtherComponentViewModel).page = this;
                (BindingContext as OtherComponentViewModel).Navigation = Navigation;
                (BindingContext as OtherComponentViewModel).OtherComponent = mtr;
                (BindingContext as OtherComponentViewModel).IsEnabled = IsEnabled;
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
                if (sender is SfAutoComplete || sender is SfComboBox || sender is Editor)
                {
                    OtherComponent = (BindingContext as OtherComponentViewModel).OtherComponent;
                    DBManager.realm.Write(() =>
                    {
                        if (sender is SfAutoComplete)
                        {
                            switch ((sender as SfAutoComplete).StyleId)
                            {
                                case "CodeAutoComplete":
                                    OtherComponent.Code = (sender as SfAutoComplete).Text;
                                    break;
                                case "DescriptionAutoComplete":
                                    OtherComponent.Description = (sender as SfAutoComplete).Text;
                                    break;
                                case "BrandAutoComplete":
                                    OtherComponent.Brand = (sender as SfAutoComplete).Text;
                                    break;
                                case "BrandTypeAutoComplete":
                                    OtherComponent.BrandType = (sender as SfAutoComplete).Text;
                                    break;
                            }
                        }
                        else if (sender is Editor)
                        {
                            switch ((sender as Editor).StyleId)
                            {

                                case "ExtraDataEditor":
                                    OtherComponent.ExtraData = (sender as Editor).Text;
                                    break;
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            OtherComponent = (BindingContext as OtherComponentViewModel).OtherComponent;
            if (InventoryAutoComplete.SelectedItem != null)
                DBManager.realm.Write(() =>
                {
                    OtherComponent.SpareParts.Add(new SparePart()
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