using Syncfusion.DataSource;
using Syncfusion.DataSource.Extensions;
using System;
using System.Collections.Generic;

namespace PlantBU.Utilities
{
    class CustomGroupComparerYearMonth : IComparer<GroupResult>, ISortDirection
    {
        public CustomGroupComparerYearMonth()
        {
            this.SortDirection = ListSortDirection.Ascending;
        }

        public ListSortDirection SortDirection
        {
            get;
            set;
        }

        public int Compare(GroupResult x, GroupResult y)
        {
            DateTime xvalue = Convert.ToDateTime(x.Key);
            DateTime yvalue = Convert.ToDateTime(y.Key);
            // Group results are compared and return the SortDirection
            if (xvalue.CompareTo(yvalue) > 0)
                return SortDirection == ListSortDirection.Ascending ? 1 : -1;
            else if (xvalue.CompareTo(yvalue) == -1)
                return SortDirection == ListSortDirection.Ascending ? -1 : 1;
            else
                return 0;
        }
    }
}
