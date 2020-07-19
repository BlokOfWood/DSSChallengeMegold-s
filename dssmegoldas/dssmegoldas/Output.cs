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
                outputFileContents.Add(
                    $"{i.OrderData.id}," +
                    $"{i.OrderData.quantity * i.OrderData.profit} Ft," +
                    $"{(int)Math.Ceiling((i.StepCompletedAt[5]-i.OrderData.dueTime).TotalDays)*i.OrderData.penaltyForDelay} Ft," +
                    $"{i.StepStartedAt:MM.dd.HH:mm}," +
                    $"{i.StepCompletedAt:MM.dd.HH:mm}," +
                    $"{i.OrderData.dueTime:MM.dd.HH:mm}");
            }

            System.IO.File.WriteAllLines("megrendelések.csv", outputFileContents.ToArray());
        }

        public static void WorkOrderDataOutput(CompletionData[] completionData)
        {
            List<string> outputFileContents = new List<string>();

            outputFileContents.Add("Dátum,Gép,Kezdő időpont,Záró időpont,Megrendelésszám");

            List<WorkOrderInstruction> workOrderData = new List<WorkOrderInstruction>();
            for(int i = 0; i < completionData.Length; i++)
            {
                for(int x = 0; x < 6; x++)
                {
                    int differenceInDays = (completionData[i].StepCompletedAt[x] - completionData[i].StepStartedAt[x]).Days;
                    if (differenceInDays > 0)
                    {
                        workOrderData.Add(
                            new WorkOrderInstruction(
                            completionData[i].StepStartedAt[x],
                            new DateTime(completionData[i].StepStartedAt[x].Year, completionData[i].StepStartedAt[x].Month, completionData[i].StepStartedAt[x].Day, 22, 00, 00),
                            completionData[i].StepDoneOn[x],
                            completionData[i].OrderData.id));
                        for (int y = 1; y < differenceInDays; y++)
                        {
                            workOrderData.Add(
                                new WorkOrderInstruction(
                                new DateTime(completionData[i].StepStartedAt[x].Year, completionData[i].StepStartedAt[x].Month, completionData[i].StepStartedAt[x].Day, 06, 00, 00) + TimeSpan.FromDays(y),
                                new DateTime(completionData[i].StepStartedAt[x].Year, completionData[i].StepStartedAt[x].Month, completionData[i].StepStartedAt[x].Day, 22, 00, 00) + TimeSpan.FromDays(y),
                                completionData[i].StepDoneOn[x],
                                completionData[i].OrderData.id));
                        }
                        workOrderData.Add(
                            new WorkOrderInstruction(
                            new DateTime(completionData[i].StepCompletedAt[x].Year, completionData[i].StepCompletedAt[x].Month, completionData[i].StepCompletedAt[x].Day, 06, 00, 00),
                            completionData[i].StepCompletedAt[x],
                            completionData[i].StepDoneOn[x],
                            completionData[i].OrderData.id));
                    }
                    else
                    {
                        workOrderData.Add(
                        new WorkOrderInstruction(
                        completionData[i].StepStartedAt[x],
                        completionData[i].StepCompletedAt[x],
                        completionData[i].StepDoneOn[x],
                        completionData[i].OrderData.id));
                    }
                }
            }

            foreach(WorkOrderInstruction i in workOrderData.OrderBy(x => x.instructionDate))
            {
                outputFileContents.Add(
                    $"{i.instructionDate:yyyy.MM.dd}," +
                    $"{i.machineName}," +
                    $"{i.instructionDate:H:mm}," +
                    $"{i.instructionEnd:H:mm}," +
                    $"{i.orderID}");
            }

            System.IO.File.WriteAllLines("munkarend.csv", outputFileContents.ToArray());
        }
    }
}
