using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dssmegoldas
{
    public static class Methods
    {

        public static ProductionLine CheckIfBetterSwapped(int idx1, int idx2, int[] prodLineCap, ProductionLine oriProdLine)
        {
            Data[] oriData = Program.data;

            // Swap in Program.data
            Data[] tmp = (Data[])Program.data.Clone();
            tmp[idx1] = Program.data[idx2];
            tmp[idx2] = Program.data[idx1];

            Program.data = tmp;

            // Make new production line
            ProductionLine newProductionLine = new ProductionLine(new DateTime(2020, 07, 20, 06, 00, 00), prodLineCap);

            if (oriProdLine.OrderCompletionData.TotalLoss() < newProductionLine.OrderCompletionData.TotalLoss())
            {
                // Not better
                // Restore Program.data
                Program.data = oriData;
                return oriProdLine;
            }

            // Better
            // Restore Program.data
            Program.data = oriData;
            return newProductionLine;

        }

        public static int TotalLoss(this CompletionData[] cDatas)
        {
            int loss = 0;

            foreach (var item in cDatas)
            {
                // Add each order's loss
                loss += LossFromOrder(item);
            }

            return loss;
        }

        public static int LossFromOrder(CompletionData completionData)
        {
            int loss = 0;

            // Calculate the time from due time to completion time
            TimeSpan tmp = completionData.StepCompletedAt[5] - completionData.OrderData.dueTime;
            
            // If it's less than zero, it completed in time
            if (tmp > TimeSpan.Zero)
            {
                // Lost for every started 24 hours
                loss += (int)(Math.Ceiling(tmp.TotalHours) / 24) * completionData.OrderData.penaltyForDelay;
            }

            return loss;
        }

        public static ProductionLine GetBetterOrder(int[] prodLineCap, ProductionLine firstProdLine)
        {
            ProductionLine newProdLine = firstProdLine;

            // Goes trough the full data array
            for (int i = 0; i < Program.data.Length; i++)
            {
                while (true)
                {
                    // Reset the best so far value to current
                    // First int is loss, last 2 int is "i" and "j"
                    (int, ProductionLine, int, int) bestSoFarProdLine = (newProdLine.OrderCompletionData.TotalLoss(), newProdLine, 0, 0);
                    int shouldBeBetterValue = bestSoFarProdLine.Item1;

                    // Swap with a data that gives the lowest loss
                    for (int j = 0; j < Program.data.Length; j++)
                    {
                        // No point swapping with itself
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

                    // Swap doesn't give better result, move on to next "i"
                    if (bestSoFarProdLine.Item1 == shouldBeBetterValue)
                    {
                        break;
                    }
                    // Spaw gives better result, swap it and check again with same "i" (different data)
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
