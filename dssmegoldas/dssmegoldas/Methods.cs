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
                loss += LossFromOrder(item);
            }

            //Console.WriteLine(loss);
            return loss;
        }

        public static int LossFromOrder(CompletionData completionData)
        {
            int loss = 0;

            TimeSpan tmp = completionData.StepCompletedAt[5] - completionData.OrderData.dueTime;
            
            if (tmp > TimeSpan.Zero)
            {
                loss += (int)Math.Floor(Math.Ceiling(tmp.TotalHours) / 24) * completionData.OrderData.penaltyForDelay;
            }

            return loss;
        }

        public static ProductionLine GetBetterOrder(int[] prodLineCap, ProductionLine firstProdLine)
        {
            ProductionLine newProdLine = firstProdLine;
            newProdLine.OrderCompletionData.OrderByDescending(x => (int)Math.Floor(Math.Ceiling((x.StepCompletedAt[5] - x.OrderData.dueTime).TotalHours) / 24) * x.OrderData.penaltyForDelay);
            Program.data = newProdLine.OrderCompletionData.ToList().ConvertAll(x => x.OrderData).ToArray();
            
            int counter = 0;
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

                        counter++;
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
