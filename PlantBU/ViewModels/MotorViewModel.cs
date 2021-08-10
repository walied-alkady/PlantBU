// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-19-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-06-2021
// ***********************************************************************
// <copyright file="MotorViewModel.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.Views;
using Syncfusion.ListView.XForms;
using Syncfusion.SfAutoComplete.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace PlantBU.ViewModels
{
    /// <summary>
    /// Class MotorViewModel.
    /// Implements the <see cref="PlantBU.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="PlantBU.ViewModels.BaseViewModel" />
    public class MotorViewModel : BaseViewModel
    {
        #region Properties
        /// <summary>
        /// Gets or sets the motor.
        /// </summary>
        /// <value>The motor.</value>
        public Motor Motor { get { return _Motor; } set { _Motor = value; OnPropertyChanged("Motor"); } }
        /// <summary>
        /// The motor
        /// </summary>
        private Motor _Motor;
       

        /// <summary>
        /// Gets or sets the motor codes list.
        /// </summary>
        /// <value>The motor codes list.</value>
        public List<string> MotorCodesList { get { var x=  DBManager.realm.All<Motor>().ToList().Select(x1 => x1.Code).Distinct().ToList();x.RemoveAll(item => item == null); return x; } }
        public List<string> MotorDescriptionsList { get { var x = DBManager.realm.All<Motor>().ToList().Select(x1 => x1.Description).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }
        public List<string> MotorBrandsList { get { var x = DBManager.realm.All<Motor>().ToList().Select(x1 => x1.Brand).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }
        public List<string> MotorBrandsTypesList { get { var x = DBManager.realm.All<Motor>().ToList().Select(x1 => x1.BrandType).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }              
        public List<string> MotorBearingList { get { var x = DBManager.realm.All<Motor>().ToList().Select(x1 => x1.BearingDE).Distinct().ToList();
                x.AddRange(DBManager.realm.All<Motor>().ToList().Select(x2 => x2.BearingNDE).ToList()); x.RemoveAll(item => item == null); return x; } }        
        public List<string> MotorGreaseBrandList { get { var x = DBManager.realm.All<Motor>().ToList().Select(x1 => x1.GreaseBrand).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }        
        public List<string> MotorGreaseBrandTypeList { get { var x = DBManager.realm.All<Motor>().ToList().Select(x1 => x1.GreaseBrandType).Distinct().ToList(); x.RemoveAll(item => item == null); return x; } }
        public List<string> MotorFreqList { get; set; } = new List<string>() { "50", "60" };
        public List<string> MotorPolesList { get; set; } = new List<string>() { "2", "4", "6", "8" };
        public Command<object> ListViewHoldingCommand { get; private set; }
        #endregion
        
        public MotorViewModel()
        {
            ListViewHoldingCommand = new Command<object>(ListViewHoldingCommandMethod);
            InventoryList = new List<Spareitem>();
            InventorySelectedItem = new Spareitem();
            foreach (var x in DBManager.realm.All<Spare>().ToList())
            {
                InventoryList.Add(new Spareitem()
                {
                    code = x.Code,
                    description1 = x.Description1,
                    description2 = x.Description2
                });
            }
        }       
        public async void ListViewHoldingCommandMethod(object obj)
        {
            try
            {
                if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(SparePart))
                {

                    SparePart eq = (SparePart)(obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData;
                    if (eq.IsValid)
                    {
                        string action = await page.DisplayActionSheet(eq.InventoryCode + "\r" + eq.Description1, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Remove);

                        if (action == Properties.Resources.Details)
                        {
                            var spareinv = DBManager.realm.All<Spare>().Where(x => x.Code == eq.InventoryCode).First();
                            if (spareinv != null)
                                await Navigation.PushAsync(new SparePage(spareinv));
                            else
                            {
                                await page.DisplayAlert(Properties.Resources.Info, eq.InventoryCode + Properties.Resources.NotFound, Properties.Resources.Ok);
                            }
                        }

                        else if (action == Properties.Resources.Remove)
                        {
                            RemoveItem<SparePart>(eq);
                            (obj as ItemHoldingEventArgs).Handled = true;
                            await page.DisplayAlert(Properties.Resources.Info, eq.InventoryCode + Properties.Resources.Deleted, Properties.Resources.Ok);
                        }
                    }
                    else
                        await page.DisplayAlert(Properties.Resources.Info, Properties.Resources.SparePart + Properties.Resources.NotAvailable, Properties.Resources.Ok);
                }
                else if ((obj as Syncfusion.ListView.XForms.ItemHoldingEventArgs).ItemData.GetType() == typeof(Motor))
                {
                    Motor mtr = (Motor)(obj as ItemHoldingEventArgs).ItemData;
                    string action = await page.DisplayActionSheet(mtr.Code, Properties.Resources.Cancel, null, Properties.Resources.Details, Properties.Resources.Edit, Properties.Resources.Remove);

                    if (action == Properties.Resources.Edit)
                    {
                        await Navigation.PushAsync(new MotorPage(mtr, true));
                    }
                    else if (action == Properties.Resources.Details)
                    {
                        await Navigation.PushAsync(new MotorPage(mtr));
                    }
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
