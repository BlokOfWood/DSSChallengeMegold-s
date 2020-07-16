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
                Console.WriteLine(TimeToCompleteSteps[i]);
            }
            Console.ReadKey();
        }
    }

    class ProductionLine
    {
        public List<CompletionData[]> ProductionLineContents;
        public CompletionData[] OrderCompletionData;

        public ProductionLine(int[] productionLineCapacity)
        {
            OrderCompletionData = new CompletionData[Program.data.Length];
            for (int i = 0; i < Program.data.Length; i++)
            {
                OrderCompletionData[i] = new CompletionData(Program.data[i]);
            }
        }

        public void TimeStep()
        {

        }
    }
}
