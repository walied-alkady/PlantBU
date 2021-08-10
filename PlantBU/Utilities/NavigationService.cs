// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-12-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 04-08-2021
// ***********************************************************************
// <copyright file="NavigationService.cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PlantBU.Utilities
{
    /// <summary>
    /// Class NavigationService.
    /// </summary>
    class NavigationService
    {
        /// <summary>
        /// Gets the navigation.
        /// </summary>
        /// <value>The navigation.</value>
        private static INavigation Navigation => Application.Current.MainPage?.Navigation;

        /// <summary>
        /// Navigates the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotSupportedException">Set navigatable main page before calling this.</exception>
        public static Task Navigate(object viewModel)
        {
            if (Navigation == null)
            {
                throw new NotSupportedException("Set navigatable main page before calling this.");
            }
            var page = GetPage(viewModel);
            page.BindingContext = viewModel;
            return Navigation.PushModalAsync(page, animated: true);
        }
        // All pages should follow the convention of being named the same way as their respective
        // View Models, except that the ViewModel suffix is replaced by Page.
        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <returns>Page.</returns>
        private static Page GetPage(object viewModel)
        {
            string snamespace = "";
            var pageType = viewModel.GetType().Name.Replace("ViewModel", "Page");
            if (pageType.Contains("ViewModel"))
                snamespace = "PlantBU.ViewModels";
            if (pageType.Contains("Page"))
                snamespace = "PlantBU.Views";
            string st1 = $"{snamespace}.{pageType}";
            string st = Type.GetType(st1).ToString(); //PlantBU.Views.EquipmentEditPage

            return (Page)Activator.CreateInstance(Type.GetType($"{snamespace}.{pageType}"));
            /*return (Page)Activator.CreateInstance(Assembly.GetEntryAssembly().CodeBase,
                                         typeof(pageType).FullName);*/
        }
    }
}
