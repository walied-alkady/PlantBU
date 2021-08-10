using PlantBU.DataModel;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLinePage'
    public partial class ProductionLinePage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLinePage'
    {
        ProductionLine ProductionLine { get { return _ProductionLine; } set { _ProductionLine = value; OnPropertyChanged("ProductionLine"); } }
        private ProductionLine _ProductionLine;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ProductionLinePage.ProductionLinePage(ProductionLine, bool)'
        public ProductionLinePage(ProductionLine emp, bool IsEnabled = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ProductionLinePage.ProductionLinePage(ProductionLine, bool)'
        {
            InitializeComponent();
            try
            {
                ProductionLine = emp;
                CodeEditor.Text = ProductionLine.Code;
                DescriptionEditor.Text = ProductionLine.Description;
                LineNoEditor.Text = ProductionLine.LineNo;
                ExtraDataEditor.Text = ProductionLine.ExtraData;
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void CodeEditor_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (sender is Editor)
            {
                try
                {
                    DBManager.realm.Write(() =>
                    {
                        switch ((sender as Editor).StyleId)
                        {
                            case "CodeEditor":
                                ProductionLine.Code = (sender as Editor).Text;
                                break;
                            case "DescriptionEditor":
                                ProductionLine.Description = (sender as Editor).Text;
                                break;
                            case "LineNoEditor":
                                ProductionLine.LineNo = (sender as Editor).Text;
                                break;
                            case "ExtraDataEditor":
                                ProductionLine.ExtraData = (sender as Editor).Text;
                                break;
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
}


