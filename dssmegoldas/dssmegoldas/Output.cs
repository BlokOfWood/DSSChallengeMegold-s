using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dssmegoldas
{
    static class Output
    {
        public static void OrderDataOutput(CompletionData[] completionData)
        {
            List<string> outputFileContents = new List<string>();
            outputFileContents.Add("Megrendelésszám,Profit összesen,Levont kötbér,Munka megkezdése,Készre jelentés ideje,Megrendelés és eredeti határideje");
            foreach (CompletionData i in completionData.OrderBy(x => x.OrderData.id))
            {
                outputFileContents.Add($"{i.OrderData.id},{i.OrderData.quantity * i.OrderData.profit} Ft,{/*(int)Math.Ceiling((*/i.CompletedAt/*-i.OrderData.dueTime*//*).TotalDays)*//**i.OrderData.penaltyForDelay*/} Ft");

            }
            foreach(string i in outputFileContents)
            {
                Console.WriteLine(i);
            }
        }
    }
}
