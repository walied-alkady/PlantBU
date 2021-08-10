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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisPage'
    public partial class SafetyReportAnalysisPage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisPage'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisPage.SafetyReportAnalysisPage(Employee)'
        public SafetyReportAnalysisPage(Employee emp)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisPage.SafetyReportAnalysisPage(Employee)'
        {
            InitializeComponent();
            (BindingContext as SafetyReportAnalysisViewModel).Navigation = Navigation;
            (BindingContext as SafetyReportAnalysisViewModel).page = this;
            (BindingContext as SafetyReportAnalysisViewModel).Employee = emp;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisPage.OnAppearing()'
        protected override void OnAppearing()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'SafetyReportAnalysisPage.OnAppearing()'
        {
            base.OnAppearing();
            List<SafetyReportAnalysisChartDataUnit> xx = (BindingContext as SafetyReportAnalysisViewModel).ChartData;
            (BindingContext as SafetyReportAnalysisViewModel).PopulateData();
        }
    }
}