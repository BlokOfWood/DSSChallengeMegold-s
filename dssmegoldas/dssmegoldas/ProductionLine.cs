using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dssmegoldas
{
    /// <summary>
    /// The data structure that holds information about the various details of the completion of an order.
    /// </summary>
    public class CompletionData
    {
        //Each array has a length of 6 for each step.

        /// <summary>
        /// The order the completion details belong to.
        /// </summary>
        public Data OrderData;
        /// <summary>
        /// The array that contains how long each step takes for this order, based on the amount of products and the type of product ordered.
        /// </summary>
        public TimeSpan[] TimeToCompleteSteps = new TimeSpan[6];
        /// <summary>
        /// The time at which the completion of each step started.
        /// </summary>
        public DateTime[] StepStartedAt = new DateTime[6];
        /// <summary>
        /// The time at which each step completed at.
        /// </summary>
        public DateTime[] StepCompletedAt = new DateTime[6];
        /// <summary>
        /// The names of the machines each step was done on.
        /// </summary>
        public string[] StepDoneOn = new string[6];

        public CompletionData(Data orderData)
        {
            OrderData = orderData;

            for (int i = 0; i < 6; i++)
            {
                //Calculates the time required for completing a step based on the product and the quantity that was ordered.
                TimeToCompleteSteps[i] = TimeSpan.FromMinutes(Program.productionStepDurations[i][orderData.product] * orderData.quantity);
            }
        }
    }

    /// <summary>
    /// The data structure that holds the information about each work order that needs to be given.
    /// </summary>
    public class WorkOrderInstruction
    {
        /// <summary>
        /// The date and time of the work order.
        /// </summary>
        public DateTime instructionDate;
        /// <summary>
        /// The time at which the work order will be completed.
        /// </summary>
        public DateTime instructionEnd;
        /// <summary>
        /// The identifier of the machine that the work order was given for.
        /// </summary>
        public string machineName;
        /// <summary>
        /// The ID of the order the machine should work for.
        /// </summary>
        public string orderID;

        public WorkOrderInstruction(DateTime _instructionDate, DateTime _instructionEnd, string _machineName, string _orderID)
        {
            instructionDate = _instructionDate;
            instructionEnd = _instructionEnd;
            machineName = _machineName;
            orderID = _orderID;
        }
    }

    public class ProductionLine
    {
        /// <summary>
        /// The data for each of the orders expanded into a completion data class, that contains additional info on how and when it was completed.
        /// </summary>
        public CompletionData[] OrderCompletionData;
        /// <summary>
        /// The time at which the production line is currently at.
        /// </summary>
        public DateTime CurrentTime;
        /*
         * First component: The time when that step of the order is complete.
         * Second component: The index of the order that the date belongs to.
        */
        /// <summary>
        /// The array that holds which orders are being worked on in which step, with the length of 6, a tuple for each step.
        /// </summary>
        public (DateTime, int)[] OrderQueue = new (DateTime, int)[6];
        /// <summary>
        /// The orders that are currently not being worked on.
        /// </summary>
        public Queue<int>[] IdleOrders = new Queue<int>[5];
        /// <summary>
        /// The index of the next order that should be passed into the first step of machines.
        /// </summary>
        int nextOrderIndex = 0;

        /// <summary>
        /// Creates a new production line object, that calculates the various required data when the constructor is called.
        /// </summary>
        /// <param name="startDate">The date from which the production starts.</param>
        /// <param name="productionLineCapacity">The amount of orders that can be worked on in each step.</param>
        public ProductionLine(DateTime startDate, int[] productionLineCapacity)
        {
            CurrentTime = startDate;

            OrderCompletionData = new CompletionData[Program.data.Count];
            for (int i = 0; i < Program.data.Count; i++)
            {
                OrderCompletionData[i] = new CompletionData(Program.data[i]);
            }
            for (int i = 0; i < 6; i++)
            {
                //Creates the arrays for each step of the machines. 
                for (int x = 0; x < OrderCompletionData.Length; x++)
                {
                    //Adjusts the previously calculated numbers according to the number of machines at each step.
                    OrderCompletionData[x].TimeToCompleteSteps[i] = TimeSpan.FromMinutes(OrderCompletionData[x].TimeToCompleteSteps[i].TotalMinutes / productionLineCapacity[i]);
                }
                if (i != 5)
                {
                    IdleOrders[i] = new Queue<int>();
                }
            }


            while (PickUpNextOrder())
            {

            }
            while (OrderCompletionData.ToList().Exists(x => x.StepCompletedAt[5].Ticks == 0))
            {
                TimeStep();
            }
        }

        /// <summary>
        /// Places the next order into the first step of machines.
        /// </summary>
        /// <returns></returns>
        public bool PickUpNextOrder()
        {
            bool placedInQueue = false;
            if (OrderQueue[0].Item2 == -1 || OrderQueue[0] == default)
            {
                OrderQueue[0] = (CalculateTimeToFinish(OrderCompletionData[nextOrderIndex], 1), nextOrderIndex);
                OrderCompletionData[nextOrderIndex].StepStartedAt[0] = CurrentTime;
                nextOrderIndex++;
                placedInQueue = true;
            }

            return placedInQueue;
        }

        /// <summary>
        /// Steps to the time of the next completion event.
        /// </summary>
        public void TimeStep()
        {
            //First two components same as before, third one is the step it was found in.
            (DateTime, int, int) doneOrderStep = (DateTime.MaxValue, -1, -1);
            //Finds the first 
            for (int i = 0; i < 6; i++)
            {
                if (OrderQueue[i].Item2 != -1 && OrderQueue[i].Item1.Ticks != 0 && doneOrderStep.Item1.Ticks > OrderQueue[i].Item1.Ticks)
                {
                    doneOrderStep = (OrderQueue[i].Item1, OrderQueue[i].Item2, i);
                }

            }
            CurrentTime = doneOrderStep.Item1;
            if (doneOrderStep.Item3 == 5)
            {
                OrderCompletionData[doneOrderStep.Item2].StepCompletedAt[5] = CurrentTime;
                OrderQueue[doneOrderStep.Item3].Item2 = -1;
            }
            else if (doneOrderStep.Item2 != -1 && doneOrderStep.Item3 != -1)
            {
                OrderCompletionData[doneOrderStep.Item2].StepCompletedAt[doneOrderStep.Item3] = CurrentTime;
                IdleOrders[doneOrderStep.Item3].Enqueue(doneOrderStep.Item2);
                OrderQueue[doneOrderStep.Item3].Item2 = -1;
            }


            for (int i = 5; i > 0; i--)
            {
                if (IdleOrders[i - 1].Count == 0) continue;
                if (OrderQueue[i].Item2 == -1 || OrderQueue[i].Item1.Ticks == 0)
                {
                    int idleIndex = IdleOrders[i - 1].Dequeue();
                    OrderCompletionData[idleIndex].StepStartedAt[i] = CurrentTime;
                    OrderQueue[i] = (CalculateTimeToFinish(OrderCompletionData[idleIndex], i), idleIndex);
                }
            }
            if (nextOrderIndex != OrderCompletionData.Length)
            {
                PickUpNextOrder();
            }
        }

        public DateTime CalculateTimeToFinish(CompletionData nextOrder, int stepInProduction)
        {
            TimeSpan timetoCompleteStep = nextOrder.TimeToCompleteSteps[stepInProduction];
            TimeSpan noShiftTime = TimeSpan.FromHours(8 * ((CurrentTime + timetoCompleteStep).DayOfYear - CurrentTime.DayOfYear));

            DateTime timeOfFinish = CurrentTime + timetoCompleteStep + noShiftTime;

            TimeSpan timeTillNextWorkHour = new TimeSpan(0);
            if (timeOfFinish.Hour > 22 || (timeOfFinish.Hour == 22 && timeOfFinish.Minute > 0) || timeOfFinish.Hour < 6)
            {
                timeOfFinish += new TimeSpan(08, 00, 00);
            }
            return timeOfFinish;
        }
    }
}