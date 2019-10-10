using System;
using System.Collections.Generic;


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
            /* для проверки активировать dateForDebug в ParkingManager */

            ParkingManager PM = new ParkingManager();
            ParkingSession ps1 = new ParkingSession(new DateTime(2019, 10, 10, 11, 40, 00), null, null, null, "gngfn", 0);

            // Проверка работы EntryParking:
            ParkingSession ps2 = PM.EnterParking("dsfhkwhg", new DateTime(2019, 10, 10, 11, 40, 00));
            Console.WriteLine("ps2 {0} null null null {1} {2}", ps2.EntryDt, ps2.CarPlateNumber, ps2.TicketNumber);
            // проверка на вьезд при номере автомобиля, который есть в ListSessionOpen
            ParkingSession ps3 = PM.EnterParking("dsfhkwhg", new DateTime(2019, 10, 10, 11, 50, 00));
            if (ps3 == null)
            {
                Console.WriteLine("Right"); //метод реализован верно
            }
            else
            {
                Console.WriteLine("Wrong");
            }
            //проверка при заполненом паркинге. Нужно выставить ParkingCapacity = 1
            ParkingSession ps4 = PM.EnterParking("оооооо", new DateTime(2019, 10, 10, 11, 55, 00));
            if (ps4 == null)
            {
                Console.WriteLine("Right");  //метод реализован верно
            }
            else
            {
                Console.WriteLine("Wrong");
            }

            //добавим еще сессий:
            ParkingSession ps5 = PM.EnterParking("fggnbvm", new DateTime(2019, 10, 10, 11, 59, 00));
            ParkingSession ps6 = PM.EnterParking("k.,j,l.mn,", new DateTime(2019, 10, 10, 12, 10, 00));
            ParkingSession ps7 = PM.EnterParking("fsfsdgdf", new DateTime(2019, 10, 10, 12, 30, 00));

            // проверка GetRemainingCost:
            // при первой оплате выезда
            // бесплатный выезд
            decimal amount1 = PM.GetRemainingCost(1, new DateTime(2019, 10, 10, 11, 54, 00) );
            Console.WriteLine(amount1); //0
            // другая цена
            DateTime DataTimeOfPS2 = new DateTime(2019, 10, 10, 11, 40, 00);
            decimal amount2 = PM.GetRemainingCost(1, DataTimeOfPS2.AddMinutes(190.0) ); // стоял 190 минут
            Console.WriteLine(amount2); //300

            // проверка работоспособности TryLeaveParkingWithTicke
            // 1. если ммашина стояла меньше 15 минут:
            //Console.WriteLine("ps2 {0} {1} {3} {4} {5} {6}", ps2.EntryDt, ps2.PaymentDt, ps2.ExitDt, ps2.TotalPayment, ps2.CarPlateNumber, ps2.TicketNumber);
        }
    }
}
  