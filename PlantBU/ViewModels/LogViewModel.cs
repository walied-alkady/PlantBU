// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-09-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="LogViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class LogViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    class LogViewModel : BaseViewModel
    {
        public Log Log { get { return _Log; } set { _Log = value; OnPropertyChanged("Log"); } }
        /// <summary>
        /// The log items
        /// </summary>
        Log _Log;
        /// <summary>
        /// Gets or sets the log items.
        /// </summary>
        /// <value>The log items.</value>
        public List<Equipment> Equipments { get { return _Equipments; } set { _Equipments = value; OnPropertyChanged("Equipments"); } }
        /// <summary>
        /// The log items
        /// </summary>
        List<Equipment> _Equipments;

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
        /// Initializes a new instance of the <see cref="LogViewModel"/> class.
        /// </summary>
        public LogViewModel()
        {
            IsBusy = true;
            ScheduleItems = new List<ScheduleItems>();
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
            repairs = new List<string>();
            repairsDetails = new Dictionary<string, string>();
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



    }
    
}