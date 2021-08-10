// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 05-26-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-09-2021
// ***********************************************************************
// <copyright file="BakRestPage.xaml.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    /// <summary>
    /// Class BakRestPage.
    /// Implements the <see cref="Xamarin.Forms.ContentPage" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BakRestPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BakRestPage"/> class.
        /// </summary>
        public BakRestPage()
        {
            InitializeComponent();
            (BindingContext as BakRestsViewModel).page = this;
            (BindingContext as BakRestsViewModel).Navigation = Navigation;
        }
        /// <summary>
        /// Handles the Clicked event of the Bakup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Bakup_Clicked(object sender, EventArgs e)
        {

            if (sender is ToolbarItem && (sender as ToolbarItem).StyleId == "Bakup")
            {
                try
                {
                    DBManager.BackUp();
                    (BindingContext as BakRestsViewModel).BackUpsReLoad();
                    DisplayAlert("PlantBU", Properties.Resources.Successfull, Properties.Resources.Ok);
                }
                catch (Exception ex)
                { DisplayAlert("PlantBU", ex.Message, Properties.Resources.Ok); }
            }
            else if (sender is ToolbarItem && (sender as ToolbarItem).StyleId == "Refresh")
            {
                try
                {
                    (BindingContext as BakRestsViewModel).BackUpsReLoad();

                }
                catch (Exception ex)
                { DisplayAlert("PlantBU", ex.Message, Properties.Resources.Ok); }
            }
        }
        /// <summary>
        /// Handles the Clicked event of the Defaults control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Defaults_Clicked(object sender, EventArgs e)
        {
            DBManager.ConnectLocalDefaults();
            Navigation.PopAsync();
        }
    }
}