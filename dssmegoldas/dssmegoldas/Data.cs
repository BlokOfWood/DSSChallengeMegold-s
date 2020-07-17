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
        public readonly double priority;

        public Data(string[] csvRowSplit, DateTime startDate)
        {
            id = csvRowSplit[0];
            product = csvRowSplit[1];
            
            if (!int.TryParse(csvRowSplit[2], out quantity))
                return;

            string[] tmp = csvRowSplit[3].Split('.');
            dueTime = new DateTime(2020, int.Parse(tmp[0]), int.Parse(tmp[1]), int.Parse(tmp[2].Split(':')[0]), int.Parse(tmp[2].Split(':')[1]), 00);

            if (!int.TryParse(csvRowSplit[4], out profit))
                return;

            if (!int.TryParse(csvRowSplit[5], out penaltyForDelay))
                return;

            priority = 1;
        }
    }
}
