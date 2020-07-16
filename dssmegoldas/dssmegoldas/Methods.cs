using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dssmegoldas
{
    public static class Methods
    {
        public static Data[] SortByPriority(this Data[] unsortedData, DateTime startDate)
        {
            return unsortedData.OrderBy(x => x.priority).ToArray();
        }

        public static bool CheckIfBetterSwapped(this Data[] datas, int idx1, int idx2)
        {
            Data[] tmp = datas;
            tmp[idx1] = datas[idx2];
            tmp[idx2] = datas[idx1];

            if(tmp.TotalLoss() < datas.TotalLoss())
            {
                return true;
            }

            return false;
        }

        public static int TotalLoss(this Data[] datas)
        {
            return 0;
        }



        public static Data[] GetBestOrder(this Data[] datasByPriority)
        {
            Data[] result = datasByPriority;

            for (int i = 0; i < datasByPriority.Length - 1; i++)
            {
                if(datasByPriority.CheckIfBetterSwapped(i, i + 1))
                {
                    Data tmp = result[i];
                    result[i] = result[i + 1];
                    result[i + 1] = tmp;
                }
            }

            return result;
        }
        
    }
}
