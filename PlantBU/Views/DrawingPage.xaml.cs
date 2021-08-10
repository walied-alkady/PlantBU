using PlantBU.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DrawingPage'
    public partial class DrawingPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DrawingPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'DrawingPage.DrawingPage(string)'
        public DrawingPage(string shop)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'DrawingPage.DrawingPage(string)'
        {
            InitializeComponent();
            try
            {
                (BindingContext as DrawingViewModel).Navigation = Navigation;
                (BindingContext as DrawingViewModel).page = this;
                (BindingContext as DrawingViewModel).OpenDrawing(shop);
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

    }
}