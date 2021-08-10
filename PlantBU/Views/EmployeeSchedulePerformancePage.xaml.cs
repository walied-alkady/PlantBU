using PlantBU.DataModel;
using PlantBU.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlantBU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformancePage'
    public partial class EmployeeSchedulePerformancePage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformancePage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformancePage.EmployeeSchedulePerformancePage()'
        public EmployeeSchedulePerformancePage()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'EmployeeSchedulePerformancePage.EmployeeSchedulePerformancePage()'
        {
            InitializeComponent();
            (BindingContext as EmployeeSchedulePerformanceViewModel).Navigation = Navigation;
            (BindingContext as EmployeeSchedulePerformanceViewModel).page = this;
            (BindingContext as EmployeeSchedulePerformanceViewModel).PopulateData();

        }

       
    }
}