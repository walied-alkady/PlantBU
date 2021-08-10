using PlantBU.DataModel;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ShopPage'
    public partial class ShopPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ShopPage'
    {
        Shop Shop { get { return _Shop; } set { _Shop = value; OnPropertyChanged("Shop"); } }
        private Shop _Shop;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'ShopPage.ShopPage(Shop, bool)'
        public ShopPage(Shop emp, bool IsEnabled = false)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'ShopPage.ShopPage(Shop, bool)'
        {
            InitializeComponent();
            try
            {
                Shop = emp;
                CodeEditor.Text = Shop.ShopName;
                DescriptionEditor.Text = Shop.ShopDescription;
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
                if (sender is Editor)
                {
                   
                        DBManager.realm.Write(() =>
                        {
                            switch ((sender as Editor).StyleId)
                            {
                                case "CodeEditor":
                                    Shop.ShopName = (sender as Editor).Text;
                                    break;
                                case "DescriptionEditor":
                                    Shop.ShopDescription = (sender as Editor).Text;
                                    break;
                            }
                        });
                  
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
    }
}