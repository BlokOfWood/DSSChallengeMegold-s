using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dssmegoldas
{
    public class Data
    {
        public readonly string id;
        public readonly string product;
        public readonly int quantity;
        public readonly DateTime dueTime;
        public readonly int profit;
        public readonly int penaltyForDelay;
        public readonly double? priority;
        
        public Data(string[] csvRowSplit, DateTime startDate)
        {
            try
            {
                id = csvRowSplit[0];
                product = csvRowSplit[1];

                quantity = int.Parse(csvRowSplit[2]);

                string[] tmp = csvRowSplit[3].Split('.');
                dueTime = new DateTime(2020, int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2].Split(':')[0]), int.Parse(tmp[2].Split(':')[1]), 00);

                profit = int.Parse(csvRowSplit[4]);

                penaltyForDelay = int.Parse(csvRowSplit[5]);

                int timetocompleteperproduct = 0;
                for (int i = 0; i < 6; i++)
                {
                    timetocompleteperproduct += Program.productionStepDurations[i][product];
                }
                priority = (TimeSpan.FromMinutes(timetocompleteperproduct * quantity) - (dueTime - startDate)).TotalHours - penaltyForDelay / 100;
            }catch (Exception)
            {
                priority = null;
                return;
            }
        }
    }
}
