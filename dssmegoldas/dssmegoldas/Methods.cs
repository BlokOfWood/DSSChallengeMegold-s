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
            //Console.WriteLine($"\nBetter ({idx1} - {idx2})\n");
            Program.data = oriData;
            return newProductionLine;

        }

        public static int TotalLoss(this CompletionData[] cDatas)
        {
            int loss = 0;

            foreach (var item in cDatas)
            {
                TimeSpan tmp = item.StepCompletedAt[5] - item.OrderData.dueTime;

                if (tmp > TimeSpan.Zero)
                {
                    loss += (int)Math.Floor(Math.Ceiling(tmp.TotalHours) / 24) * item.OrderData.penaltyForDelay;
                }
            }
            //Console.WriteLine(loss);
            return loss;
        }



        public static ProductionLine GetBestOrder(int[] prodLineCap, ProductionLine oriProdLine)
        {
            ProductionLine newProdLine = oriProdLine;

            for (int i = 0; i < Program.data.Length; i++)
            {
                while (true)
                {

                    (int, ProductionLine, int, int) bestSoFarProdLine = (newProdLine.OrderCompletionData.TotalLoss(), newProdLine, 0, 0);
                    int gotBetter = bestSoFarProdLine.Item1;

                    for (int j = 0; j < Program.data.Length; j++)
                    {
                        if (j == i)
                            continue;

                        ProductionLine tmpProdLine = CheckIfBetterSwapped(i, j, prodLineCap, newProdLine);

                        if (tmpProdLine.OrderCompletionData.TotalLoss() < bestSoFarProdLine.Item1)
                        {
                            bestSoFarProdLine.Item1 = tmpProdLine.OrderCompletionData.TotalLoss();
                            bestSoFarProdLine.Item2 = tmpProdLine;
                            bestSoFarProdLine.Item3 = i;
                            bestSoFarProdLine.Item4 = j;
                        }

                    }

                    // Not better, next
                    if (bestSoFarProdLine.Item1 == gotBetter)
                    {
                        break;
                    }
                    else
                    {
                        newProdLine = bestSoFarProdLine.Item2;
                        Data tmpDat = Program.data[bestSoFarProdLine.Item3];
                        Program.data[bestSoFarProdLine.Item3] = Program.data[bestSoFarProdLine.Item4];
                        Program.data[bestSoFarProdLine.Item4] = tmpDat;
                    }

                }
            }

            return newProdLine;

        }
    }
}
