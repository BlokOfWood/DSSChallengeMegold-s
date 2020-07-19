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

        public static void BruteForce(ref int counter, ref ProductionLine bestProdLine, ref int bestLoss, ref List<int> order, int lenght, int[] prodLineCap)
        {
            counter++;

            if(counter % 10000 == 0)
            {
                foreach (var item in order)
                {
                    Console.Write(item + "-");
                }
                Console.WriteLine("           " + counter + " - " + bestLoss);

            }

            if (order.Count == lenght)
            {
                Data[] progSave = Program.data;
                Data[] newData = new Data[lenght];

                for (int i = 0; i < lenght; i++)
                {
                    newData[i] = Program.data[order[i]];
                }

                Program.data = newData;

                ProductionLine prodLine = new ProductionLine(new DateTime(2020, 07, 20, 06, 00, 00), prodLineCap);

                if(bestLoss > TotalLoss(prodLine.OrderCompletionData))
                {
                    bestLoss = TotalLoss(prodLine.OrderCompletionData);
                    bestProdLine = prodLine;
                    return;
                }
                else
                {
                    return;
                }
            }


            for (int i = 0; i < lenght; i++)
            {
                if (!order.Contains(i))
                {
                    order.Add(i);
                    BruteForce(ref counter, ref bestProdLine, ref bestLoss, ref order, lenght, prodLineCap);
                    order.Remove(i);
                }
            }

            return;
        }

        public static ProductionLine GetBestOrder(int[] prodLineCap, ProductionLine firstProdLine)
        {
            ProductionLine newProdLine = firstProdLine;
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
            Console.WriteLine(counter);
            return newProdLine;
            
        }
    }
}
