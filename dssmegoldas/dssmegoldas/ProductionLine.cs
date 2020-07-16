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
        public DateTime[] StageCompletedAt = new DateTime[6];

        public CompletionData(Data orderData)
        {
            OrderData = orderData;

            for(int i = 0; i < 6; i++)
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
        public (DateTime, int)[][] OrderQueue = new(DateTime, int)[6][];
        int nextOrderIndex = 0;
        
        public ProductionLine(DateTime startDate, int[] productionLineCapacity)
        {
            OrderCompletionData = new CompletionData[Program.data.Length];
            for (int i = 0; i < Program.data.Length; i++)
            {
                OrderCompletionData[i] = new CompletionData(Program.data[i]);
            }
            for(int i = 0; i < 6; i++)
            {
                OrderQueue[i] = new (DateTime, int)[productionLineCapacity[i]];
            }
        }

        public void PickUpNextOrder()
        {
            for (int i = 0; i < OrderQueue[0].Length; i++)
            {
                if (OrderQueue[0][i].Item2 == -1)
                {
                    OrderQueue[0][i] = (CalculateTimeToFinish(OrderCompletionData[nextOrderIndex], 1), nextOrderIndex);
                }
            }
            nextOrderIndex++;
            SortTimeDestination();
        }

        public void TimeStep()
        {
            for (int i = 0; i < 6; i++)
            {

            }
        }

        public DateTime CalculateTimeToFinish(CompletionData nextOrder, int stepInProduction)
        {
            TimeSpan timetoCompleteStep = nextOrder.TimeToCompleteSteps[stepInProduction];
            TimeSpan noShiftTime = TimeSpan.FromHours(8 * ((CurrentTime + timetoCompleteStep).DayOfYear - CurrentTime.DayOfYear));

            Console.WriteLine($"{CurrentTime} - {timetoCompleteStep} - {noShiftTime} - {(CurrentTime + timetoCompleteStep).DayOfYear - CurrentTime.DayOfYear}");

            return CurrentTime + timetoCompleteStep + noShiftTime;
        }

        public void SortTimeDestination()
        {
        }
    }
}
