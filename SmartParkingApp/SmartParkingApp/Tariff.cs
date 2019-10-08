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
            Minutes = Convert.ToInt32(secondDt.Subtract(firstDt).Minutes);
            int minbefore = 0;
            foreach (KeyValuePair<int, decimal> keyValue in dictTariff)
            {
                if (Minutes < keyValue.Key && Minutes < minbefore)
                {
                    Rate = keyValue.Value;
                    break;
                }
                minbefore = keyValue.Key;
            }
        }
    }

}
