using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dssmegoldas
{
    public static class Methods
    {
        public static Data[] SortByPriority(this Data[] unsortedData, DateTime startDate)
        {
            return unsortedData.OrderBy(x => x.priority).ToArray();
        }

        public static ProductionLine CheckIfBetterSwapped(int idx1, int idx2, int[] prodLineCap, ProductionLine oriProdLine)
        {
            Data[] oriData = Program.data;

            Data[] tmp = (Data[])Program.data.Clone();
            tmp[idx1] = Program.data[idx2];
            tmp[idx2] = Program.data[idx1];

            Program.data = tmp;

            ProductionLine newProductionLine = new ProductionLine(new DateTime(2020, 07, 20, 06, 00, 00), prodLineCap);

            if (oriProdLine.OrderCompletionData.TotalLoss() < newProductionLine.OrderCompletionData.TotalLoss())
            {
                // Not better
                Program.data = oriData;
                return oriProdLine;
            }

            //Better
            Console.WriteLine($"\nBetter ({idx1} - {idx2})\n");
            return newProductionLine;

        }

        public static int TotalLoss(this CompletionData[] cDatas)
        {
            int loss = 0;

            foreach (var item in cDatas)
            {
                TimeSpan tmp = item.CompletedAt - item.OrderData.dueTime;

                if (tmp > TimeSpan.Zero)
                {
                    loss += (int)Math.Floor(Math.Ceiling(tmp.TotalHours) / 24) * item.OrderData.penaltyForDelay;
                }
            }
            Console.WriteLine(loss);
            return loss;
        }



        public static void GetBestOrder(int[] prodLineCap, ProductionLine oriProdLine)
        {
            ProductionLine newProdLine = oriProdLine;


            for (int i = 0; i < Program.data.Length - 1; i++)
            {
                newProdLine = CheckIfBetterSwapped(i, i + 1, prodLineCap, newProdLine);
            }
        }
    }
}
