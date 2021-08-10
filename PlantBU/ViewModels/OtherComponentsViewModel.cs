using PlantBU.DataModel;
using PlantBU.Views;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    class OtherComponentsViewModel : BaseViewModel
    {
        public List<OtherComponent> OtherComponents { get { return _OtherComponents; } set { _OtherComponents = value; OnPropertyChanged("OtherComponents"); } }
        List<OtherComponent> _OtherComponents;
        public Command<object> ListViewTappedCommand { get; private set; }
        public Command<object> ListViewHoldingCommand { get; private set; }

        public OtherComponentsViewModel()
        {
            IsBusy = true;
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            OtherComponents = GetItems<OtherComponent>();
            IsBusy = false;
        }
        public async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is OtherComponent)
                {
                    OtherComponent mtr = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as OtherComponent;
                    if (mtr.IsValid)
                        await Navigation.PushAsync(new OtherComponentPage(mtr));
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.OtherComponent + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
        /// <summary>
        /// ListViews the holding command method.
        /// </summary>
        /// <param name="obj">The object.</param>
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(OtherComponent))
                {
                    OtherComponent mtr = (OtherComponent)(obj as ItemHoldingEventArgs).ItemData;
                    string action = "";
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);
                    else
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details);

                    if (action == Properties.Resources.Edit)
                        await Navigation.PushAsync(new OtherComponentPage(mtr, true));
                    else if (action == Properties.Resources.Details)
                        await Navigation.PushAsync(new OtherComponentPage(mtr));
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
