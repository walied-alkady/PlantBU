using PlantBU.DataModel;
using PlantBU.Views;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    class MotorsViewModel:BaseViewModel
    {
        public List<Motor> Motors { get { return _Motors; } set { _Motors = value; OnPropertyChanged("Motors"); } }
        List<Motor> _Motors;
        public Command<object> ListViewTappedCommand { get; private set; }
        public Command<object> ListViewHoldingCommand { get; private set; }

        public MotorsViewModel()
        {
            IsBusy = true;
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            Motors = GetItems<Motor>();
            IsBusy = false;
        }
        public async void ListViewTappedCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Motor)
                {
                    Motor mtr = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Motor;
                    if (mtr.IsValid)
                        await Navigation.PushAsync(new MotorPage(mtr));
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Motor + Properties.Resources.NotAvailable, Properties.Resources.Ok);
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
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Motor))
                {
                    Motor mtr = (Motor)(obj as ItemHoldingEventArgs).ItemData;
                    string action = "";
                    if (DBManager.CurrentUserType == DBManager.Usertyypes.Admin)
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);
                    else
                        action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details);

                    if (action == Properties.Resources.Edit)
                        await Navigation.PushAsync(new MotorPage(mtr, true));
                    else if (action == Properties.Resources.Details)
                        await Navigation.PushAsync(new MotorPage(mtr));
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
