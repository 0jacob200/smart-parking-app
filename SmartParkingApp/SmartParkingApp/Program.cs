using System;
using System.Collections.Generic;

/* 
 */

namespace ParkingApp
{
    class Program
    {
        public const string Separator = "_____________________________________";
        public const string InquiryCarPlateNum = "Enter car plate number: ";
        public const string MainMenu  = "Menu\n" +
                    "1. Entrance of car\n" +
                    "2. Pay for the parking\n" +
                    "3. Leave parking\n" +
                    "4. Informaion\n" +
                    "5. Exit";
        public const string InquiryTicketNumber = "Enter insert your ticket id: ";
        //public const string  = "";
        //public const string  = "";


        static void Main(string[] args)
        {
            ParkingManager pm = new ParkingManager();
            Console.WriteLine("Smart parking application");
            Console.WriteLine(Separator);
            //do
            //{
            //    Console.WriteLine();

            //    //Console.WriteLine(InquiryCarPlateNum);
            //    //string inputCarPlateNum = Console.ReadLine();

            //}
            //while (true);
        }
    }
}
  