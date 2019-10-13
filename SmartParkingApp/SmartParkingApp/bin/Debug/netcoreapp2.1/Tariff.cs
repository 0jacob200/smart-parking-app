using System;
using System.Collections.Generic;

namespace ParkingApp
{
    class Tariff
    {
        public int Minutes { get; set; }
        public decimal Rate { get; set; }

        public Tariff(DateTime firstDt, DateTime secondDt, Dictionary<int, decimal> dictTariff)
        {
            Minutes = Convert.ToInt32((secondDt - firstDt).TotalMinutes);
            if (Minutes <= ParkingManager.FreeLeavePeriod){
                Rate = 0;
                return;
            }
            int minbefore = 0;
            foreach (KeyValuePair<int, decimal> keyValue in dictTariff)
            {
                if (Minutes <= keyValue.Key && Minutes > minbefore)
                {
                    Rate = keyValue.Value;
                    break;
                }
                minbefore = keyValue.Key;
                if (keyValue.Key == FindMaxKeyInDict(dictTariff))
                {
                    Rate = keyValue.Value;
                }
            }
        }

        public int FindMaxKeyInDict(Dictionary<int, decimal> dict)
        {
            int maxkey = 0;
            foreach (int key in dict.Keys)
            {
                if (maxkey < key)
                {
                    maxkey = key;
                }
            }
            return maxkey;
        }
    }

}
