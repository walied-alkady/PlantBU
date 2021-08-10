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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpensePage'
	public partial class BudgetToExpensePage : ContentPage
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpensePage'
	{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpensePage.BudgetToExpensePage()'
		public BudgetToExpensePage ()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpensePage.BudgetToExpensePage()'
		{
			InitializeComponent ();
			(BindingContext as BudgetToExpenseViewModel).Navigation = Navigation;
			(BindingContext as BudgetToExpenseViewModel).page = this;
		}
	}
}