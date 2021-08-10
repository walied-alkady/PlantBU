// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-08-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="ScheduleViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class ScheduleViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    class ScheduleViewModel : BaseViewModel
    {
        public Schedule Schedule { get { return _Schedule; } set { _Schedule = value; OnPropertyChanged("Schedule"); } }
      
        Schedule _Schedule;

        /// <summary>
        /// Gets or sets the schedule items.
        /// </summary>
        /// <value>The schedule items.</value>
        public List<ScheduleItems> ScheduleItems { get { return _ScheduleItems; } set { _ScheduleItems = value; OnPropertyChanged("ScheduleItems"); } }
        /// <summary>
        /// The schedule items
        /// </summary>
        List<ScheduleItems> _ScheduleItems;
        /// <summary>
        /// Gets or sets the repairs.
        /// </summary>
        /// <value>The repairs.</value>
        public List<string> repairs { get { return _repairs; } set { _repairs = value; OnPropertyChanged("repairs"); } }
        /// <summary>
        /// The repairs
        /// </summary>
        List<string> _repairs;
        /// <summary>
        /// Gets or sets the repairs details.
        /// </summary>
        /// <value>The repairs details.</value>
        public Dictionary<string, string> repairsDetails { get { return _repairsDetails; } set { _repairsDetails = value; OnPropertyChanged("repairsDetails"); } }
        /// <summary>
        /// The repairs details
        /// </summary>
        Dictionary<string, string> _repairsDetails;
        /* public List<string> Assignees { get { return _Assignees.ToList().Select(x=>string.Concat(x.FirstName," " , x.LastName)).ToList(); } }
         List<Employee> _Assignees;*/
        /// <summary>
        /// Gets or sets the assignees.
        /// </summary>
        /// <value>The assignees.</value>
        public List<Employee> Assignees { get { return _Assignees; } set { _Assignees = value; OnPropertyChanged("Assignees"); } }
        /// <summary>
        /// The assignees
        /// </summary>
        List<Employee> _Assignees;
        /// <summary>
        /// Gets the add schedule command.
        /// </summary>
        /// <value>The add schedule command.</value>
        public Command AddScheduleCommand { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleViewModel"/> class.
        /// </summary>

        public Command<object> ListViewHoldingCommand { get; private set; }

        public ScheduleViewModel()
        {
            IsBusy = true;
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            LoadInventoryList();
            ScheduleItems = new List<ScheduleItems>();
            repairs = new List<string>();
            repairsDetails = new Dictionary<string, string>();
            ShopList = DBManager.realm.All<Shop>().ToList();
            foreach (Motor x in DBManager.realm.All<Motor>().ToList())
                {
                    ScheduleItems.Add(new ViewModels.ScheduleItems()
                    {
                        Code = x.Code,
                        Description = x.Description
                    });
                }
            foreach (Sensor x in DBManager.realm.All<Sensor>().ToList())
            {
                ScheduleItems.Add(new ViewModels.ScheduleItems()
                {
                    Code = x.Code,
                    Description = x.Description
                });
            }
            foreach (OtherComponent x in DBManager.realm.All<OtherComponent>().ToList())
            {
                ScheduleItems.Add(new ViewModels.ScheduleItems()
                {
                    Code = x.Code,
                    Description = x.Description
                });
            }
            FieldInfo[] fi = typeof(MotorMaintainenanceDetails).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            foreach (FieldInfo info in fi)
            {
                repairs.Add(info.Name);

            }
            MotorMaintainenanceDetails structValue = new MotorMaintainenanceDetails();

            foreach (FieldInfo info in fi)
            {
                repairsDetails.Add(info.Name, info.GetValue(structValue).ToString());
            }
            Assignees = DBManager.realm.All<Employee>().ToList();
            
            IsBusy = false;
        }
        /// <summary>
        /// Adds the schedule command method.
        /// </summary>
        public void AddScheduleCommandMethod()
        {

        }
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(ScheduleSparePart))
                {

                    ScheduleSparePart eq = (ScheduleSparePart)(obj as ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {
                        string action = await page.DisplayActionSheet(Properties.Resources.Info, Properties.Resources.Cancel, null, Properties.Resources.Remove);

                       if (action == Properties.Resources.Remove)
                        {

                            DBManager.realm.Write(() =>
                            {
                                DBManager.realm.Remove(eq);
                            });
                            await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Schedule + Properties.Resources.Deleted, Properties.Resources.Ok);
                        }
                        
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.Schedule + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
            }
            catch (Exception ex)
            {
                await page.DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }

    }
    /// <summary>
    /// Class ScheduleItems.
    /// </summary>
    public class ScheduleItems
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
    
#pragma warning disable CS1587 // XML comment is not placed on a valid language element
/// <summary>
    /// Class Assignee.
    /// </summary>
}
#pragma warning restore CS1587 // XML comment is not placed on a valid language element