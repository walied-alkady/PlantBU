// ***********************************************************************
// Assembly         : PlantBU
// Author           : WaliedAlkady
// Created          : 04-12-2021
//
// Last Modified By : WaliedAlkady
// Last Modified On : 04-06-2021
// ***********************************************************************
// <copyright file="BooleanToStarConverter .cs" company="PlantBU">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using PlantBU.DataModel;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace PlantBU.Utilities
{
    /// <summary>
    /// Class BooleanToStarConverter.
    /// </summary>
    public class EmployeeCodeToEmployeeLastNameConverter : IValueConverter
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeCodeToEmployeeLastNameConverter.Convert(object, Type, object, CultureInfo)'
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeCodeToEmployeeLastNameConverter.Convert(object, Type, object, CultureInfo)'
        {
            var employeeCode = value as string;
            if (!string.IsNullOrEmpty(employeeCode))
                return DBManager.realm.All<Employee>().Where(x => x.CompanyCode == employeeCode).FirstOrDefault()?.LastName;
            else
                return null;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeCodeToEmployeeLastNameConverter.ConvertBack(object, Type, object, CultureInfo)'
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeCodeToEmployeeLastNameConverter.ConvertBack(object, Type, object, CultureInfo)'
        {
            return "";
        }
    }
}
