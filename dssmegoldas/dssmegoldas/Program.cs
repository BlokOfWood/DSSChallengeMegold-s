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
        static void Main(string[] args)
        {
            Data[] arr = File.ReadAllLines("output.csv").Select(x => new Data(x.Split(','))).ToArray();

            arr = arr.SortByPriority(new DateTime(2020, 07, 20, 06, 00, 00));

            foreach (var item in arr)
            {
                Console.WriteLine($"{item.id} - {item.product} - {item.quantity} - {item.dueTime} - {item.profit} - {item.penaltyForDelay} ----- {item.CalculatePriority(new DateTime(2020, 07, 20, 06, 00, 00))}");
            }
        }
    }
}
