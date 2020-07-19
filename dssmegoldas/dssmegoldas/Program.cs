using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dssmegoldas
{
    class Program
    {
        public static Data[] data;
        /*
         * An array of product type and production duration pairs.
         * The various elements of the array pertain to the various steps of making the product.
         * For Example
         * The 0th element's "GYB"th element provides us with the
         * duration of the first step of making a "GYB" type product.  
         */
        public static Dictionary<string, int>[] productionStepDurations;
        public static Dictionary<int, string> stepMachineNames;
        public static DateTime startTime;

        static void Main(string[] args)
        {
            int[] prodLineCap = { 6, 2, 3, 1, 4, 3 };
            productionStepDurations = new Dictionary<string, int>[] {
                new Dictionary<string, int>
                {
                    { "GYB", 5 },
                    { "FB", 8 },
                    { "SB", 6 }
                },
                new Dictionary<string, int>
                {
                    { "GYB", 10 },
                    { "FB", 16 },
                    { "SB", 15 }
                },
                new Dictionary<string, int>
                {
                    { "GYB", 8 },
                    { "FB", 12 },
                    { "SB", 10 }
                },
                new Dictionary<string, int>
                {
                    { "GYB", 5 },
                    { "FB", 5 },
                    { "SB", 5 }
                },
                new Dictionary<string, int>
                {
                    { "GYB", 12 },
                    { "FB", 20 },
                    { "SB", 15 }
                },
                new Dictionary<string, int>
                {
                    { "GYB", 10 },
                    { "FB", 15 },
                    { "SB", 12 }
                },
            };
            stepMachineNames = new Dictionary<int, string>
            {
                { 0, "Vágó" },
                { 1, "Hajlító" },
                { 2, "Hegesztő" },
                { 3, "Tesztelő" },
                { 4, "Festő" },
                { 5, "Csomagoló" }
            };

            startTime = new DateTime(2020, 07, 20, 06, 00, 00);


            string[] lines = File.ReadAllLines("output.csv");
            data = lines.Select(x => new Data(x.Split(','), startTime)).ToArray();
            LogSystem.StartLogging(lines, data);


            data = data.OrderBy(x => x.priority).ToArray();
            ProductionLine productionLine = new ProductionLine(startTime, prodLineCap);

            LogSystem.SearchStarted();
            Methods.GetBetterOrder(prodLineCap, productionLine);
            LogSystem.SearchFinished(productionLine.OrderCompletionData.TotalLoss());

            /*foreach (var item in data)
            {
                Console.WriteLine($"{item.id} - {item.product} - {item.quantity} - {item.dueTime} - {item.profit} - {item.penaltyForDelay}");
            }

            productionLine = new ProductionLine(startTime, prodLineCap);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(productionLine.OrderCompletionData.TotalLoss());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(data.Sum(x => x.profit * x.quantity));

            Output.OrderDataOutput(productionLine.OrderCompletionData);
            Output.WorkOrderDataOutput(productionLine.OrderCompletionData);*/
            Console.ReadKey();
            
        }
    }
}
