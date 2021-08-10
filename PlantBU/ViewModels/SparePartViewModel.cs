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

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class SpareViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    class SparePartViewModel : BaseViewModel
    {
#pragma warning disable CS0108 // 'SparePartViewModel.InventoryList' hides inherited member 'BaseViewModel.InventoryList'. Use the new keyword if hiding was intended.
        public List<Partitem> InventoryList { get { return _InventoryList; } set { _InventoryList = value; OnPropertyChanged("InventoryList"); } }
#pragma warning restore CS0108 // 'SparePartViewModel.InventoryList' hides inherited member 'BaseViewModel.InventoryList'. Use the new keyword if hiding was intended.
        List<Partitem> _InventoryList;

        public SparePartViewModel()
        {
            InventoryList = new List<Partitem>();
            foreach (var x in DBManager.realm.All<Spare>().ToList())
            {
                InventoryList.Add(new Partitem()
                {
                    code = x.Code,
                    description1 = x.Description1,
                    description2 = x.Description2
                });
            }
        }


    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Partitem'
    public class Partitem
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Partitem'
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string code { get; set; }
        /// <summary>
        /// Gets or sets the description1.
        /// </summary>
        /// <value>The description1.</value>
        public string description1 { get; set; }
        /// <summary>
        /// Gets or sets the description2.
        /// </summary>
        /// <value>The description2.</value>
        public string description2 { get; set; }
    }
}
