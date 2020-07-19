using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dssmegoldas
{
    public static class LogSystem
    {
        private static FileStream fs;
        private static StreamWriter sw;

        public static void StartLogging()
        {
            fs = new FileStream($"logfile{DateTime.Now:yyyyMMddHHmm}.txt", FileMode.Create);
            sw = new StreamWriter(fs);

            string s = $"[{DateTime.Now.ToString()}] Log file created";
            sw.WriteLine(s);
            sw.Flush();
            Console.WriteLine(s);

        }


        public static void LogDataInput(string inputString)
        {
            string s = $@"[{DateTime.Now.ToString()}] Input line ""{inputString}"" read and processed as:";
            sw.WriteLine(s);
            sw.Flush();
            Console.WriteLine(s);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Null if processing is not successful</param>
        public static void LogDataProcess(Data data)
        {
            string s;

            if (data == null)
                s = "Constructing Data type from input string failed";
            else
                s = $@"ID: {data.id} - Product Type: {data.product} - Quantity: {data.quantity} - Due time: {data.dueTime} - Profit (/piece): {data.profit} - Penalty For Delay (/day): {data.penaltyForDelay}";

            sw.WriteLine(s);
            sw.Flush();
            Console.WriteLine(s);
        }

        public static void SearchStarted()
        {
            string s = $"[{DateTime.Now.ToString()}] Searching for low loss started";
            sw.WriteLine(s);
            sw.Flush();
            Console.WriteLine(s);
        }

        public static void SearchFinished(int lowLoss)
        {
            string s = $"[{DateTime.Now.ToString()}] Searching for low loss completed. Lowest loss found: {lowLoss}";
            sw.WriteLine(s);
            Console.WriteLine(s);
            sw.Close();
        }
    }
}
