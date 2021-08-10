using PlantBU.DataModel;
using PlantBU.Views;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    class SensorsViewModel : BaseViewModel
    {
        public List<Sensor> Sensors { get { return _Sensors; } set { _Sensors = value; OnPropertyChanged("Sensors"); } }
        List<Sensor> _Sensors;
        public Command<object> ListViewTappedCommand { get; private set; }
        public Command<object> ListViewHoldingCommand { get; private set; }

        public SensorsViewModel()
        {
            IsBusy = true;
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            Sensors = GetItems<Sensor>();
            IsBusy = false;
        }
        public async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Sensor)
                {
                    Sensor mtr = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Sensor;
                    if (mtr.IsValid)
                        await Navigation.PushAsync(new SensorPage(mtr));
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Sensor + Properties.Resources.NotAvailable, Properties.Resources.Ok);
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
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Sensor))
                {
                    Sensor mtr = (Sensor)(obj as ItemHoldingEventArgs).ItemData;
                    string action = "";
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);
                    else
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details);

                    if (action == Properties.Resources.Edit)
                        await Navigation.PushAsync(new SensorPage(mtr, true));
                    else if (action == Properties.Resources.Details)
                        await Navigation.PushAsync(new SensorPage(mtr));
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
