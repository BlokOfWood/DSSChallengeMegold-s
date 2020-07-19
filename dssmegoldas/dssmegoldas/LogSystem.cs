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
        private static FileStream fs = new FileStream("logfile.txt", FileMode.Create);
        private static StreamWriter sw = new StreamWriter(fs);

        public static void StartLogging(string[] fullCSVLines, Data[] datas)
        {
            

            string s = $"[{DateTime.Now.ToString()}] Log file created";
            sw.WriteLine(s);
            Console.WriteLine(s);

            for (int i = 0; i < fullCSVLines.Length; i++)
            {
                LogDataInput(fullCSVLines[i]);
                LogDataProcess(datas[i]);
            }

        }


        private static void LogDataInput(string inputString)
        {
            string s = $@"[{DateTime.Now.ToString()}] Input line ""{inputString}"" read and processed as:";
            sw.WriteLine(s);
            Console.WriteLine(s);
        }

        private static void LogDataProcess(Data data)
        {
            string s = $@"ID: {data.id} - Product Type: {data.product} - Quantity: {data.quantity} - Due time: {data.dueTime} - Profit (/piece): {data.profit} - Penalty For Delay (/day): {data.penaltyForDelay}";
            sw.WriteLine(s);
            Console.WriteLine(s);
        }

        public static void SearchStarted()
        {
            string s = $"[{DateTime.Now.ToString()}] Searching for low loss started";
            sw.WriteLine(s);
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
