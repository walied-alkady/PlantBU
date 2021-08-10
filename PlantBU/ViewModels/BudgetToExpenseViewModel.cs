using PlantBU.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlantBU.ViewModels
{
    class BudgetToExpenseViewModel :BaseViewModel
    {
        public List<Budgetitem> Budgetitems { get { return _Budgetitems; } set { _Budgetitems = value; OnPropertyChanged("Budgetitems"); } }
        List<Budgetitem> _Budgetitems;
        public List<ExpensItem> Expenses { get { return _Expenses; } set { _Expenses = value; OnPropertyChanged("Expenses"); } }
        List<ExpensItem> _Expenses;
        public List<BudgetToExpenseChartDataUnit> Data { get; set; }
        public string ExpensesSum
        {
            get
            {
                if (Expenses != null)
                    return Expenses.Sum(e => e.Value).ToString();
                else
                    return "";
            }
        }
        public string BudgeSum
        {
            get
            {
                if (Budgetitems != null)
                    return Budgetitems.Sum(e => e.Value).ToString();
                else
                    return "";
            }
        }
        public BudgetToExpenseViewModel()
        {
            PopulateData();
        }
        private void PopulateData()
        {
            IsBusy = true;
            Budgetitems = GetItems<Budgetitem>().ToList(); 
            Expenses    = GetItems<ExpensItem>().ToList();
            Data = new List<BudgetToExpenseChartDataUnit>()
            {
                new BudgetToExpenseChartDataUnit ("Jan" ,(from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==1 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==1 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Feb",(from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==2 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==2 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Mar", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==3 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==3 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Apr", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==4 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==4 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "May", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==5 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==5 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Jun", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==6 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==6 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Jul", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==7 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==7 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Aug", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==8 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==8 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Sep", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==9 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==9 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Oct", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==10 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==10 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Nov", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==11 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==11 select emp).Sum(e => e.Value)),
                new BudgetToExpenseChartDataUnit ( "Dec", (from emp in Budgetitems where emp.DateBudgetItem.LocalDateTime.Month==12 select emp).Sum(e => e.Value),(from emp in Expenses where emp.DateExpense.LocalDateTime.Month==12 select emp).Sum(e => e.Value))
            };
            IsBusy = false;
        }
       
    }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit'
    public class BudgetToExpenseChartDataUnit
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit'
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit.BudgetToExpenseChartDataUnit(string, double, double)'
        public BudgetToExpenseChartDataUnit(string month, double budgetvalue, double expensevalue)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit.BudgetToExpenseChartDataUnit(string, double, double)'
        {
            Month = month;
            BudgetValue = budgetvalue; ExpenseValue = expensevalue;
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit.Month'
        public string Month { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit.Month'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit.BudgetValue'
        public double BudgetValue { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit.BudgetValue'
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit.ExpenseValue'
        public double ExpenseValue { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'BudgetToExpenseChartDataUnit.ExpenseValue'


    }
}
