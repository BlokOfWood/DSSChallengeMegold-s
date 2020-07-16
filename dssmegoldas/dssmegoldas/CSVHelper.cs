using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dssmegoldas
{
    class CSVHelper
    {
        //Parses the copy pasted google docs table text into a .csv file.
        public static void csvifyToFile()
        {
            List<string> input = new List<string>();

            string newLine = "";
            do
            {
                newLine = Console.ReadLine();
                if(newLine.Replace(" ","").Length != 0)
                input.Add(newLine);
            } while (newLine != "");

            int rowLength = 6;
            string[] output = new string[input.Count / rowLength];

            for (int y = 0; y < output.Length; y++)
            {
                for (int x = 0; x < rowLength; x++)
                {
                    output[y] += input[y * rowLength + x] + ',';
                }
                output[y] = output[y].Replace(" ", "");
                output[y] = output[y].Remove(output[y].Length - 1);
            }

            File.WriteAllLines("output.csv", output.ToArray());

            Console.ReadLine();
        }
    }
}