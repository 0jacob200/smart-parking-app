﻿using System;

namespace ParkingApp
{
    [Serializable]
    class ParkingSession
    {
        // Date and time of arriving at the parking
        public DateTime EntryDt { get; set; }
        // Date and time of payment for the parking
        public DateTime? PaymentDt { get; set; }
        // Date and time of exiting the parking
        public DateTime? ExitDt { get; set; }
        // Total cost of parking
        public decimal? TotalPayment { get; set; }
        // Plate number of the visitor's car
        public string CarPlateNumber { get; set; }
        // Issued printed ticket
        public int TicketNumber { get; set; }

        public ParkingSession(DateTime entryDT, DateTime? paymentDT, DateTime? exitDT,
            decimal? totalPay, string carPlateNum, int ticketNum)
        {
            EntryDt = entryDT;
            PaymentDt = paymentDT;
            ExitDt = exitDT;
            TotalPayment = totalPay;
            CarPlateNumber = carPlateNum;
            TicketNumber = ticketNum;
        }

    }
}
