using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantPage'
    public partial class PlantPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'PlantPage.PlantPage()'
        public PlantPage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'PlantPage.PlantPage()'
        {
            InitializeComponent();
            (BindingContext as PlantViewModel).Navigation = Navigation;
            (BindingContext as PlantViewModel).page = this;
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button)
                {
                    
                        if (DBManager.realm.All<Plant>().Any())
                            switch ((sender as Button).StyleId)
                            {
                                case "AddLine":
                                    var newstring = DBManager.realm.All<ProductionLine>().ToList().Select(x => x.Code).ToList().Where(y => y.Length > 4).Select(z => z.Substring(4, 1)).Select(int.Parse).ToList();
                                    string newstringName;
                                    if (newstring.Any())
                                        newstringName = "Line" + (newstring.Max() + 1).ToString();
                                    else
                                        newstringName = "Line1";

                                    ProductionLine NewPL = new ProductionLine() { Code = newstringName, Description = "New Line" };
                                    Plant Bu = (BindingContext as PlantViewModel).Plant;
                                    Bu.ProductionLineAdd(NewPL);
                                    (BindingContext as PlantViewModel).CheckLinesShops();
                                    await Navigation.PushAsync(new ProductionLinePage(NewPL));
                                    break;
                            }
                        else
                            await DisplayAlert(Properties.Resources.Info, Properties.Resources.Initialize + Properties.Resources.Plant, Properties.Resources.Ok);
                   
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        private async void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (sender is Entry)
                {
                    var newstring = DBManager.realm.All<Plant>().ToList().Select(x => x.Code).ToList().Where(y => y.Length > 4).Select(z => z.Substring(4, 1)).Select(int.Parse).ToList();
                    string newstringName;
                    if (newstring.Any())
                        newstringName = "Plant" + (newstring.Max() + 1).ToString();
                    else
                        newstringName = "Plant1";
                    DBManager.realm.Write(() =>
                    {
                        Plant plant;
                        if (!DBManager.realm.All<Plant>().Any())
                        {
                            plant = DBManager.realm.Add<Plant>(new Plant() { Code = newstringName, Description = "New Plant" });
                        }
                        else
                            plant = DBManager.realm.All<Plant>().First();

                        switch ((sender as Entry).StyleId)
                        {
                            case "BUCodeEntry":
                                plant.Code = (sender as Entry).Text;
                                break;
                            case "DescriptionEntry":
                                plant.Description = (sender as Entry).Text;
                                break;
                            case "LocationEntry":
                                plant.Location = (sender as Entry).Text;
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