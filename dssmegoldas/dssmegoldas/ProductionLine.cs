using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dssmegoldas
{
    class CompletionData
    {
        public Data OrderData;
        public TimeSpan[] TimeToCompleteSteps = new TimeSpan[6];
        public DateTime CompletedAt;

        public CompletionData(Data orderData)
        {
            OrderData = orderData;

            for (int i = 0; i < 6; i++)
            {
                TimeToCompleteSteps[i] = TimeSpan.FromMinutes(Program.productionStepDurations[i][orderData.product] * orderData.quantity);
            }
        }
    }

    class ProductionLine
    {
        public CompletionData[] OrderCompletionData;
        public DateTime CurrentTime;
        /*
         * First component: The next step in time.
         * Second component: The index of the order that the date belongs to.
        */
        public (DateTime, int)[][] OrderQueue = new (DateTime, int)[6][];
        public Queue<int>[] IdleOrders = new Queue<int>[5];
        int nextOrderIndex = 0;

        public ProductionLine(DateTime startDate, int[] productionLineCapacity)
        {
            CurrentTime = startDate;

            OrderCompletionData = new CompletionData[Program.data.Length];
            for (int i = 0; i < Program.data.Length; i++)
            {
                OrderCompletionData[i] = new CompletionData(Program.data[i]);
            }
            for (int i = 0; i < 6; i++)
            {
                OrderQueue[i] = new (DateTime, int)[productionLineCapacity[i]];
                if (i != 5)
                {
                    IdleOrders[i] = new Queue<int>();
                }
            }


            while (PickUpNextOrder())
            {

            }
            while (OrderCompletionData[OrderCompletionData.Length - 1].CompletedAt.Ticks == 0)
            {
                TimeStep();
                // OrderQueue.ToList().ForEach(x => x.ToList().ForEach(y => Console.WriteLine(y.Item1)));
            }
        }

        public bool PickUpNextOrder()
        {
            bool placedOrderInQueue = false;
            for (int i = 0; i < OrderQueue[0].Length; i++)
            {
                if (OrderQueue[0][i].Item2 == -1 || OrderQueue[0][i] == default)
                {
                    OrderQueue[0][i] = (CalculateTimeToFinish(OrderCompletionData[nextOrderIndex], 1), nextOrderIndex);
                    placedOrderInQueue = true;
                    break;
                }
            }
            if(placedOrderInQueue)
            nextOrderIndex++;
            return placedOrderInQueue;
        }

        public void TimeStep()
        {
            //First two components same as before, third one is the step it was found in, the fourth one is the index of which machine it was inside of in that step.
            (DateTime, int, int, int) doneOrderStep = (DateTime.MaxValue, -1, -1, -1);
            for (int i = 0; i < 6; i++)
            {
                for (int x = 0; x < OrderQueue[i].Length; x++)
                {
                    if (OrderQueue[i][x].Item2 != -1 && OrderQueue[i][x].Item1.Ticks != 0 && doneOrderStep.Item1.Ticks > OrderQueue[i][x].Item1.Ticks)
                    {
                        doneOrderStep = (OrderQueue[i][x].Item1, OrderQueue[i][x].Item2, i, x);
                    }
                }
            }
            CurrentTime = doneOrderStep.Item1;
            if (doneOrderStep.Item3 == 5)
            {
                OrderCompletionData[doneOrderStep.Item2].CompletedAt = CurrentTime;
                OrderQueue[doneOrderStep.Item3][doneOrderStep.Item4].Item2 = -1;
            }
            else if (doneOrderStep.Item2 != -1 && doneOrderStep.Item3 != -1 && doneOrderStep.Item4 != -1)
            {
                IdleOrders[doneOrderStep.Item3].Enqueue(doneOrderStep.Item2);
                OrderQueue[doneOrderStep.Item3][doneOrderStep.Item4].Item2 = -1;
            }
            if(OrderQueue[doneOrderStep.Item3][doneOrderStep.Item4].Item2 == -1)
            {
                
            }
            //Console.WriteLine($"{doneOrderStep.Item1} - {doneOrderStep.Item2} - {doneOrderStep.Item3} - {doneOrderStep.Item4}");


            for (int i = 5; i > 0; i--)
            {
                for (int x = 0; x < OrderQueue[i].Length; x++)
                {
                    if (OrderQueue[i][x].Item2 == -1 || OrderQueue[i][x].Item1.Ticks == 0)
                    {
                        if (IdleOrders[i - 1].Count > 0)
                        {
                            int idleIndex = IdleOrders[i - 1].Dequeue();
                            OrderQueue[i][x] = (CalculateTimeToFinish(OrderCompletionData[idleIndex], i), idleIndex);
                        }
                    }
                }
            }
            if (nextOrderIndex != OrderCompletionData.Length - 1)
            {
                PickUpNextOrder();
            }
        }

        public DateTime CalculateTimeToFinish(CompletionData nextOrder, int stepInProduction)
        {
            TimeSpan timetoCompleteStep = nextOrder.TimeToCompleteSteps[stepInProduction];
            TimeSpan noShiftTime = TimeSpan.FromHours(8 * ((CurrentTime + timetoCompleteStep).DayOfYear - CurrentTime.DayOfYear));

            return CurrentTime + timetoCompleteStep + noShiftTime;
        }
    }
}
