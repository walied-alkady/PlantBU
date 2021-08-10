// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-01-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-07-2021
// ***********************************************************************
// <copyright file="SpareViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class SpareViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    class SpareViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the spare.
        /// </summary>
        /// <value>The spare.</value>
        public Spare Spare { get { return _Spare; } set { _Spare = value; OnPropertyChanged("Spare"); } }
        /// <summary>
        /// The spare
        /// </summary>
        Spare _Spare;
        /// <summary>
        /// Gets or sets the related items.
        /// </summary>
        /// <value>The related items.</value>
        public List<RelatedItem> RelatedItems { get { return _RelatedItems; } set { _RelatedItems = value; OnPropertyChanged("RelatedItems"); } }
        /// <summary>
        /// The related items
        /// </summary>
        List<RelatedItem> _RelatedItems;
        /// <summary>
        /// Gets or sets the picker selected.
        /// </summary>
        /// <value>The picker selected.</value>
        public string PickerSelected { get { return _PickerSelected; } set { _PickerSelected = value; OnPropertyChanged("PickerSelected"); } }
        /// <summary>
        /// The picker selected
        /// </summary>
        string _PickerSelected;
        /// <summary>
        /// Gets the ListView tapped command.
        /// </summary>
        /// <value>The ListView tapped command.</value>
        public Command<object> ListViewTappedCommand { get; private set; }
        /// <summary>
        /// Gets the ListView holding command.
        /// </summary>
        /// <value>The ListView holding command.</value>
        public Command<object> ListViewHoldingCommand { get; private set; }
        /// <summary>
        /// Gets the search command.
        /// </summary>
        /// <value>The search command.</value>
        public Command SearchCommand => new Command<string>((string query) =>
        {
            /* if (!string.IsNullOrEmpty(PickerSelected))
                 switch (PickerSelected)
                 {
                     case "Code":
                         Motors = GetItems<Motor>(query, "Code");
                         break;
                     case "Description":
                         Motors = GetItems<Motor>(query, "Description");
                         break;
                     case "Area":
                         Motors = GetItems<Motor>(query, "Area");
                         break;
                 }*/

        });
        /// <summary>
        /// Initializes a new instance of the <see cref="SpareViewModel"/> class.
        /// </summary>
        public SpareViewModel()
        {
            ListViewTappedCommand = new Command<object>(ListViewTappedCommandMethod);
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            PickerSelected = "Code";

        }
        /// <summary>
        /// Loads the related.
        /// </summary>
        public void LoadRelated()
        {
            RelatedItems = new List<RelatedItem>();
            if (DBManager.realm.All<SparePart>().Count() > 0)
                foreach (SparePart x in DBManager.realm.All<SparePart>().ToList())
                {
                    if (x.InventoryCode == Spare.Code)
                    {
                        Motor st1 = DBManager.realm.All<Motor>().Where(mt => mt.Code == x.ItemCode).Count() > 0 ? DBManager.realm.All<Motor>().Where(mt => mt.Code == x.ItemCode).First() : null;
                        Sensor st2 = DBManager.realm.All<Sensor>().Where(mt => mt.Code == x.ItemCode).Count() > 0 ? DBManager.realm.All<Sensor>().Where(mt => mt.Code == x.ItemCode).First() : null;
                        OtherComponent st3 = DBManager.realm.All<OtherComponent>().Where(mt => mt.Code == x.ItemCode).Count() > 0 ? DBManager.realm.All<OtherComponent>().Where(mt => mt.Code == x.ItemCode).First() : null;
                        if (st1 != null)
                            RelatedItems.Add(new RelatedItem() { Code = st1.Code, Description = st1.Description });
                        else if (st2 != null)
                            RelatedItems.Add(new RelatedItem() { Code = st2.Code, Description = st2.Description });
                        else if (st3 != null)
                            RelatedItems.Add(new RelatedItem() { Code = st3.Code, Description = st3.Description });
                    }
                }
        }
        /// <summary>
        /// ListViews the tapped command method.
        /// </summary>
        /// <param name="obj">The object.</param>
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async void ListViewTappedCommandMethod(object obj)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {

            /*if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Equipment)
            {

                Equipment eq = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Equipment;
                if (eq.IsValid)
                {
                    Equipment = eq;
                    Motors = eq.Motors.ToList();
                    await Navigation.PushAsync(new EquipmentPage(eq));
                }
                else
                    await page.DisplayAlert("PlantBU", "Equipment is not Available", "ok");
            }
            else if ((obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData is Motor)
            {
                Motor mtr = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as Motor;
                if (mtr.IsValid)
                    await Navigation.PushAsync(new MotorPage(mtr));
                else
                    await page.DisplayAlert("PlantBU", "Motor is not Available", "ok");
            }*/
        }
        /// <summary>
        /// ListViews the holding command method.
        /// </summary>
        /// <param name="obj">The object.</param>
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async void ListViewHoldingCommandMethod(object obj)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {

            /* if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Equipment))
             {

                 Equipment eq = (Equipment)(obj as ItemHoldingEventArgs).ItemData;
                 if (eq.IsValid)
                 {
                     string action = await page.DisplayActionSheet(eq.Code, "Cancel", null, "Details", "Edit", "Remove");

                     if (action == "Edit")
                     {
                         await Navigation.PushAsync(new EquipmentPage(eq, true));
                     }
                     else if (action == "Details")
                     {
                         await Navigation.PushAsync(new EquipmentPage(eq));
                     }
                     else if (action == "Remove")
                     {
                         RemoveItem<Equipment>(eq);

                         await page.DisplayAlert("Info", eq.Code + ": deleted!", "Ok");
                     }
                 }
                 else
                     await page.DisplayAlert("PlantBU", "Equipment is not Available", "ok");
             }
             else if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Motor))
             {
                 Motor mtr = (Motor)(obj as ItemHoldingEventArgs).ItemData;
                 string action = await page.DisplayActionSheet(mtr.Code, "Cancel", null, "Details", "Edit", "Remove");

                 if (action == "Edit")
                 {
                     await Navigation.PushAsync(new MotorPage(mtr, true));
                 }
                 else if (action == "Details")
                 {
                     await Navigation.PushAsync(new MotorPage(mtr));
                 }
                 else if (action == "Remove")
                 {
                     RemoveItem(mtr);
                     await page.DisplayAlert("Info", mtr + ": deleted!", "Ok");
                 }
             }*/
        }
        /// <summary>
        /// Refreshes the items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RefreshItems<T>()
        {
            /*   if (Motors != null)
                   Motors.Clear();
               Motors = GetItems<Motor>()*/
            ;
        }
    }
    /// <summary>
    /// Class RelatedItem.
    /// </summary>
    public class RelatedItem
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }
    }
}
