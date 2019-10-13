using System;
using System.Collections.Generic;


namespace ParkingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            /* для проверки активировать dateForDebug в ParkingManager */

            ParkingManager PM = new ParkingManager();
            // проверка ParkingSession
            ParkingSession parkingSession1 = new ParkingSession(new DateTime(2019, 10, 10, 11, 40, 00), null, null, null, "gngfn", 0);


            // Проверка работы EntryParking:
            ParkingSession parkingSession2 = PM.EnterParking("dsfhkwhg", new DateTime(2019, 10, 10, 11, 40, 00));
            Console.WriteLine("parkingSession2 {0} null null null {1} {2}", parkingSession2.EntryDt,
                parkingSession2.CarPlateNumber, parkingSession2.TicketNumber);
            // проверка на вьезд при номере автомобиля, который есть в ListSessionOpen
            DateTime DateTimeOfparkingSession3 = new DateTime(2019, 10, 10, 11, 50, 00);
            ParkingSession parkingSession3 = PM.EnterParking("dsfhkwhg", DateTimeOfparkingSession3);
            if (parkingSession3 == null)
            {
                Console.WriteLine("Right"); //метод реализован верно
            }
            else
            {
                Console.WriteLine("Wrong");
            }
            //проверка при заполненом паркинге. Нужно выставить ParkingCapacity = 1
            var DateTimeOfparkingSession4 = new DateTime(2019, 10, 10, 11, 55, 00);
            ParkingSession parkingSession4 = PM.EnterParking("оооооо", DateTimeOfparkingSession4);
            if (parkingSession4 == null)
            {
                Console.WriteLine("Right");  //метод реализован верно
            }
            else
            {
                Console.WriteLine("Wrong");
            }

            //добавим еще сессий:
            var DateTimeOfparkingSession5 = new DateTime(2019, 10, 10, 11, 59, 00);
            ParkingSession parkingSession5 = PM.EnterParking("fggnbvm", DateTimeOfparkingSession5);
            var DateTimeOfparkingSession6 = new DateTime(2019, 10, 10, 12, 10, 00);
            ParkingSession parkingSession6 = PM.EnterParking("kjlmnvv", DateTimeOfparkingSession6);
            var DateTimeOfparkingSession7 = new DateTime(2019, 10, 10, 12, 30, 00);
            ParkingSession parkingSession7 = PM.EnterParking("fsfsdgd", DateTimeOfparkingSession7);


            // проверка GetRemainingCost:
            // 1. при первой оплате выезда
            //    а. бесплатный выезд
            DateTime DataTimeOfparkingSession2 = new DateTime(2019, 10, 10, 11, 40, 00);
            decimal amount1 = PM.GetRemainingCost(1, DataTimeOfparkingSession2.AddMinutes(14.0)); // 14 минут
            Console.WriteLine(amount1); //0
            //    б. другая цена
            decimal amount2 = PM.GetRemainingCost(1, DataTimeOfparkingSession2.AddMinutes(190.0)); // стоял 190 минут
            Console.WriteLine(amount2); // 360
            // 2. при второй оплате выезда
            PM.PayForParking(1, amount2, DataTimeOfparkingSession2.AddMinutes(191.0)); // проверка PayForParking
            amount2 = PM.GetRemainingCost(1, DataTimeOfparkingSession2.AddMinutes(231.0)); // 40 минут с момента оплаты
            Console.WriteLine(amount2); // 120


            // проверка  TryLeaveParkingWithTicket
            // 1. если машина стояла не более 15 минут:
            bool ExitAggrement3 = PM.TryLeaveParkingWithTicket(2, out parkingSession3, DateTimeOfparkingSession3.AddMinutes(15));
            if (ExitAggrement3 == true)
            {
                Console.WriteLine("Доступ разрешен"); // метод верен
            }
            else
            {
                Console.WriteLine("доступ запрещен");
            }
            Console.WriteLine($"parkingSession3 {parkingSession3.EntryDt} {parkingSession3.PaymentDt} {parkingSession3.ExitDt}" +
                $" {parkingSession3.TotalPayment} {parkingSession3.CarPlateNumber} {parkingSession3.TicketNumber}");
            // 2. если машина стояла 700 минут и нет оплаты
            bool ExitAggrement4 = PM.TryLeaveParkingWithTicket(3, out parkingSession4, DateTimeOfparkingSession4.AddMinutes(700));
            if (ExitAggrement4 == true)
            {
                Console.WriteLine("Доступ разрешен");
            }
            else
            {
                Console.WriteLine("доступ запрещен"); // метод верен
            }
            // 3. если машина стояла 340 минут и оплата есть. выезд через 10 минут после оплаты
            decimal amountFor5 = PM.GetRemainingCost(4, DateTimeOfparkingSession5.AddMinutes(339));
            PM.PayForParking(4, amountFor5, DateTimeOfparkingSession5.AddMinutes(340));
            bool ExitAggrement5 = PM.TryLeaveParkingWithTicket(4, out parkingSession5, DateTimeOfparkingSession5.AddMinutes(350)); 
            if (ExitAggrement5 == true)
            {
                Console.WriteLine("Доступ разрешен"); // метод верен
            }
            else
            {
                Console.WriteLine("доступ запрещен");
            }
            Console.WriteLine($"parkingSession5 {parkingSession5.EntryDt} {parkingSession5.PaymentDt} {parkingSession5.ExitDt}" +
                $" {parkingSession5.TotalPayment} {parkingSession5.CarPlateNumber} {parkingSession5.TicketNumber}");
            // 4. если машина стояла 340 минут и оплата есть. выезд через 20 минут после оплаты
            decimal amountFor6 = PM.GetRemainingCost(5, DateTimeOfparkingSession6.AddMinutes(340));
            PM.PayForParking(5, amountFor6, DateTimeOfparkingSession6.AddMinutes(340));
            ParkingSession outsession;
            bool ExitAggrement6 = PM.TryLeaveParkingWithTicket(5, out outsession, DateTimeOfparkingSession6.AddMinutes(360));
            if (ExitAggrement6 == true)
            {
                Console.WriteLine("Доступ разрешен");
            }
            else
            {
                Console.WriteLine("доступ запрещен"); // метод верен
            }
            try
            {
                Console.WriteLine($"parkingSession6 {outsession.EntryDt} {outsession.PaymentDt} {outsession.ExitDt}" +
            $" {outsession.TotalPayment} {outsession.CarPlateNumber} {outsession.TicketNumber}");
            }
            catch
            {
                Console.WriteLine("session = null");
            }
        }

    }
}  