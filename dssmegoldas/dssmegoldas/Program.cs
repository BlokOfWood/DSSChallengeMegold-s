using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;

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
        public static Dictionary<string, int>[] productionStepDurations = new Dictionary<string, int>[] {
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
        public static Dictionary<int, string> stepMachineNames = new Dictionary<int, string>
            {
                { 0, "Vágó" },
                { 1, "Hajlító" },
                { 2, "Hegesztő" },
                { 3, "Tesztelő" },
                { 4, "Festő" },
                { 5, "Csomagoló" }
            };
        public static int[] productionLineStepCapacities = new int[] { 6, 2, 3, 1, 4, 3 };
        public static DateTime startTime = new DateTime(2020, 07, 20, 06, 00, 00);

        static void Main(string[] args)
        {
            string path = "";
            if(args.Length == 0)
            {
                Console.WriteLine("Nem adott meg utat a bemeneti fájlhoz");
                return;
            }
            else
            {
                path = args.Aggregate((x, y) => x + y);
                try
                {
                    data = File.ReadAllLines(path).Select(x => new Data(x.Split(','), startTime)).ToArray();
                }
                catch(FileNotFoundException)
                {
                    Console.WriteLine("A fájl nem található! \ndssmegoldas.exe [út a fájlhoz]");
                    return;
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Ismeretlen hiba. {e.Message}");
                    return;
                }
            }
            data = data.OrderBy(x => x.priority).ToArray();

            ProductionLine productionLine = new ProductionLine(startTime, productionLineStepCapacities);

            Methods.GetBetterOrder(productionLineStepCapacities, productionLine);
            
            foreach (var item in data)
            {
                Console.WriteLine($"{item.id} - {item.product} - {item.quantity} - {item.dueTime} - {item.profit} - {item.penaltyForDelay}");
            }

            productionLine = new ProductionLine(startTime, productionLineStepCapacities);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(productionLine.OrderCompletionData.TotalLoss());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(data.Sum(x => x.profit * x.quantity));

            Output.OrderDataOutput(productionLine.OrderCompletionData);
            Output.WorkOrderDataOutput(productionLine.OrderCompletionData);
            Console.ReadKey();
            
        }
    }
}
