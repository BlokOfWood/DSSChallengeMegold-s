using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dssmegoldas
{
    public static class Methods
    {
        public static double CalculatePriority(this Data data, DateTime startDate)
        {
            double prio = ((data.dueTime - startDate).TotalMinutes) * (1d / data.penaltyForDelay);

            return prio;
        }

        public static Data[] SortByPriority(this Data[] unsortedData, DateTime startDate)
        {
            return unsortedData.OrderBy(x => x.CalculatePriority(startDate)).ToArray();
        }
    }
}
