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


        public Data(string[] csvRowSplit)
        {
            id = csvRowSplit[0];
            product = csvRowSplit[1];
            
            if (!int.TryParse(csvRowSplit[2], out quantity))
                return;

            if (!DateTime.TryParse(csvRowSplit[3], out dueTime))
                return;

            if (!int.TryParse(csvRowSplit[4], out profit))
                return;

            if (!int.TryParse(csvRowSplit[5], out penaltyForDelay))
                return;
        }
    }
}
