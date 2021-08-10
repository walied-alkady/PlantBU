// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 06-01-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 06-02-2021
// ***********************************************************************
// <copyright file="SparePage.xaml.cs" company="PlantBU">
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
    /// Class SparePage.
    /// Implements the <see cref="Xamarin.Forms.ContentPage" />
    /// </summary>
    /// <seealso cref="Xamarin.Forms.ContentPage" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SparePage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SparePage"/> class.
        /// </summary>
        /// <param name="spr">The SPR.</param>
        /// <param name="IsEnabled">if set to <c>true</c> [is enabled].</param>
        public SparePage(Spare spr, bool IsEnabled = false)
        {
            InitializeComponent();
            try
            {
                (BindingContext as SpareViewModel).Navigation = Navigation;
                (BindingContext as SpareViewModel).page = this;
                (BindingContext as SpareViewModel).Spare = spr;
                (BindingContext as SpareViewModel).LoadRelated();
                (BindingContext as SpareViewModel).IsEnabled = IsEnabled;
            }
            catch (Exception ex)
            {
                DisplayAlert(Properties.Resources.Error, ex.Message, Properties.Resources.Ok);
            }
        }
    }
}