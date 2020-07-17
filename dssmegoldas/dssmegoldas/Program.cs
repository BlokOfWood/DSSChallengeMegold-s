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

        static void Main(string[] args)
        {
            data = File.ReadAllLines("output.csv").Select(x => new Data(x.Split(','), new DateTime(2020, 07, 20, 06, 00, 00))).ToArray();

            foreach (var item in data)
            {
                Console.WriteLine(item.id);
            }

            /*var lol = data.GroupBy(x => x.dueTime.DayOfYear / 10);

            List<List<Data>> list = new List<List<Data>>();

            foreach (var group in lol)
            {
                Console.WriteLine(group.Key);
                var l = group.OrderByDescending(x => x.penaltyForDelay);
                foreach (var item in l)
                {
                    Console.Write(item.dueTime);
                    Console.Write("     ");

                    Console.WriteLine(item.penaltyForDelay);

                }
                list.Add(l.ToList());
            }

            data = list.SelectMany(x => x).ToArray();
            */
            
            /*foreach (var item in data)
            {
                Console.WriteLine($"{item.id} - {item.product} - {item.quantity} - {item.dueTime} - {item.profit} - {item.penaltyForDelay} ----- {item.priority}");
            }*/

            int[] prodLineCap = { 6, 2, 3, 1, 4, 3 };
            Dictionary<string, int>[] prodStepDur =
            { 
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
            productionStepDurations = prodStepDur;

            ProductionLine productionLine = new ProductionLine(new DateTime(2020, 07, 20, 06, 00, 00), prodLineCap);


            for (int i = 0; i < 30; i++)
            {
                productionLine = Methods.GetBestOrder(prodLineCap, productionLine);

            }

            //Console.WriteLine("\n\nNew:\n");

            foreach (var item in data)
            {
                Console.WriteLine($"{item.id} - {item.product} - {item.quantity} - {item.dueTime} - {item.profit} - {item.penaltyForDelay} ----- {item.priority}");
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(productionLine.OrderCompletionData.TotalLoss());
            Console.ForegroundColor = ConsoleColor.White;

            productionLine = new ProductionLine(new DateTime(2020, 07, 20, 06, 00, 00), prodLineCap);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(productionLine.OrderCompletionData.TotalLoss());

            Console.WriteLine(data.Sum(x => x.profit * x.quantity));

            Console.ForegroundColor = ConsoleColor.White;

            Output.OrderDataOutput(productionLine.OrderCompletionData);
            Console.ReadKey();
            
        }
    }
}
