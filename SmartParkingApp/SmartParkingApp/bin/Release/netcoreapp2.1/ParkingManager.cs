using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ParkingApp
{
    [Serializable]
    class ParkingManager
    {
        private static List<ParkingSession> ListSessionOpen = new List<ParkingSession>();
        private static List<ParkingSession> ListSessionClosed = new List<ParkingSession>();
        private static Dictionary<int, decimal> DictTariff = new Dictionary<int, decimal>();
        private static List<User> LoyaltyProgram = new List<User>();

        public int ParkingCapacity
        {
            get;
            private set;
        }

        public int TicketId
        {
            get;
            private set;
        }

        internal static int FreeLeavePeriod = 15;

        /// <summary>
        /// Конструктор, вызываемый при первом запуске
        /// </summary>
        /// <param name="parkingCapacity">Вместимость парковки</param>
        public ParkingManager(int parkingCapacity, Dictionary<int, decimal> dTariff)
        {
            ParkingCapacity = parkingCapacity;
            TicketId = 0;
            DictTariff = dTariff;

            var binform = new BinaryFormatter();
            using (var file = new FileStream("../../../info_Parking_Manager.dat", FileMode.OpenOrCreate))
            {
                binform.Serialize(file, parkingCapacity);
                binform.Serialize(file, dTariff);
            }
        }

        /// <summary>
        /// Конструктор, вызываемый при повторных запусках
        /// </summary>
        public ParkingManager()
        {
            var binform = new BinaryFormatter();
            using (var file = new FileStream("../../../open_session.dat", FileMode.OpenOrCreate))
            {
                ListSessionOpen = (List<ParkingSession>)binform.Deserialize(file);
            }

            var binform2 = new BinaryFormatter();
            using (var file = new FileStream("../../../closed_session.dat", FileMode.OpenOrCreate))
            {
                ListSessionClosed = (List<ParkingSession>)binform2.Deserialize(file);
            }
            var binform3 = new BinaryFormatter();
            using (var file = new FileStream("../../../info_Parking_Manager.dat", FileMode.Open))
            {
                ParkingCapacity = (int)binform3.Deserialize(file);
                DictTariff = (Dictionary<int, decimal>)binform3.Deserialize(file);
            }
            try
            {
                var binform4 = new BinaryFormatter();
                using (var file = new FileStream("../../../loyalty_progam.dat", FileMode.OpenOrCreate))
                {
                    LoyaltyProgram = (List<User>)binform4.Deserialize(file);
                }
            }
            catch { }
            TicketId = ListSessionClosed.Count + ListSessionOpen.Count;
        }

        public ParkingSession EnterParking(string carPlateNumber, DateTime dateForDebug)
        {
            foreach (ParkingSession session in ListSessionOpen)
            {
                if (session.CarPlateNumber == carPlateNumber)
                {
                    return null;
                }
            }
            if (ParkingCapacity - ListSessionOpen.Count == 0)
            {
                return null;
            }
            ParkingSession newParkingSession = new ParkingSession(/*DateTime.Now*/
                dateForDebug, null, null, null, carPlateNumber, ++TicketId);
            User userEntering = FindLoyalUser(carPlateNumber);
            if (userEntering != null)
            {
                newParkingSession.StatusLoyaltyProgram = userEntering;
            }
            ListSessionOpen.Add(newParkingSession);
            SerializeChangesOfListSessionOpen();
            return newParkingSession;

        }

        public bool TryLeaveParkingWithTicket(int ticketNumber, out ParkingSession session, DateTime dateForDebug) // TODO: убрать dateForDebug
        {
            #region
            /*
             * Check that the car leaves parking within the free leave period
             * from the PaymentDt (or if there was no payment made, from the EntryDt)
             * 1. If yes:
             *   1.1 Complete the parking session by setting the ExitDt property
             *   1.2 Move the session from the list of active sessions to the list of past sessions 
             *   1.3 return true and the completed parking session object in the out parameter
             * 
             * 2. Otherwise, return false, session = null
             * _____________________
             * 
             * Убедитесь, что автомобиль покидает парковку в период бесплатного отпуска
             * из PaymentDt (или, если не было сделано платежа, из EntryDt)
             * 1. Если да:
             * 1.1 Завершите сессию парковки, установив свойство ExitDt
             * 1.2 Переместите сессию из списка активных сессий в список прошлых сессий
             * 1.3 возвращает true и завершенный объект парковки в параметре out
             *
             * 2. В противном случае вернуть false, session = null
             * (можно написать не налл, а что такое другое, но написать почему)
             */
            #endregion

            bool flagOfAccess = false;
            session = FindOpenSessionByTicket(ticketNumber);

            if (GetRemainingCost(ticketNumber, dateForDebug) == 0)
            {
                flagOfAccess = true;
            }

            if (session == null)
            {
                flagOfAccess = false;
            }

            #region
            //double minAfterLastPayment;
            //if (session.PaymentDt != null)
            //{
            //    minAfterLastPayment = (/*DateTime.Now*/dateForDebug - (DateTime)session.PaymentDt).TotalMinutes;
            //}
            //else
            //{
            //    minAfterLastPayment = 0;
            //}


            //if ((/*DateTime.Now*/dateForDebug - session.EntryDt).TotalMinutes <= FreeLeavePeriod) 
            //{

            //    flagOfAccess = true;
            //}
            //else if (session.PaymentDt == null)
            //{
            //    flagOfAccess = false;
            //}
            //else if (minAfterLastPayment <= FreeLeavePeriod)
            //{
            //    flagOfAccess = true;
            //}
            //else if ()
            //{
            //    flagOfAccess = false;
            //}
            //else if (session.TotalPayment != null)
            //{
            //    flagOfAccess = true;
            //}
            //else
            //{
            //    Console.WriteLine("непредвиденный case???");
            //    flagOfAccess = false;
            //}
            #endregion

            switch (flagOfAccess)
            {
                case true:
                    session.ExitDt = /*DateTime.Now*/dateForDebug;
                    ListSessionOpen.Remove(session);
                    SerializeChangesOfListSessionOpen();
                    ListSessionClosed.Add(session);
                    SerializeListSessionClosed();
                    break;

                case false:
                    session = null;
                    break;
            }
            return flagOfAccess;

        }

        public decimal GetRemainingCost(int ticketNumber, DateTime dateForDebug)
        {            
            decimal amount = 0;
            ParkingSession session = FindOpenSessionByTicket(ticketNumber);

            if (session.PaymentDt != null)
            {
                
                Tariff tf = new Tariff((System.DateTime)session.PaymentDt, /*DateTime.Now*/dateForDebug, DictTariff);
                amount = tf.Rate;

            }
            else
            {
                Tariff tf = new Tariff(session.EntryDt, /*DateTime.Now*/dateForDebug, DictTariff);
                amount = tf.Rate;
            }
            return amount;
        }

        public void PayForParking(int ticketNumber, decimal amount, DateTime dateForDebug)
        {
            ParkingSession session = FindOpenSessionByTicket(ticketNumber);
            if (session.PaymentDt == null)
            {
                session.TotalPayment = amount;
            }
            else
            {
                session.TotalPayment += amount;
            }
            session.PaymentDt = /*DateTime.Now*/dateForDebug;
            SerializeChangesOfListSessionOpen();
        }

        public bool TryLeaveParkingByCarPlateNumber(string carPlateNumber, out ParkingSession session, DateTime dateForDebug)
        {
            

            bool flagOfAccessExit;
            session = FindOpenSessionByCarPlateNumber(carPlateNumber);
            flagOfAccessExit = ( session == null || (FindLoyalUser(carPlateNumber) != null ? false : true) ) ? false : true;
            if (flagOfAccessExit == true)
            {
                if ((/*DateTime.Now*/dateForDebug - session.EntryDt).TotalMinutes <= FreeLeavePeriod * 2)
                {
                    flagOfAccessExit = true;
                }
                else if (session.PaymentDt != null)
                {
                    if ((/*DateTime.Now*/dateForDebug - (System.DateTime)session.PaymentDt).TotalMinutes <= FreeLeavePeriod)
                    {
                        flagOfAccessExit = true;
                    }
                    else
                    {
                        flagOfAccessExit = false;
                    }
                }
                else if (session.PaymentDt == null)
                {
                    flagOfAccessExit = false;
                }
            }

            switch (flagOfAccessExit)
            {
                case true:
                    session.ExitDt = /*DateTime.Now*/dateForDebug;
                    ListSessionOpen.Remove(session);
                    SerializeChangesOfListSessionOpen();
                    ListSessionClosed.Add(session);
                    SerializeListSessionClosed();
                    break;

                case false:
                    session = null;
                    break;
            }
            return flagOfAccessExit;
        }

        /// <summary>
        /// Нахождение Открытой сессии по номеру билета
        /// </summary>
        /// <param name="ticketNum">номер билета</param>
        /// <returns>Класс ParkingSession</returns>
        private ParkingSession FindOpenSessionByTicket(int ticketNum)
        {
            foreach (ParkingSession session in ListSessionOpen)
            {
                if (session.TicketNumber == ticketNum)
                {
                    return session;
                }
            }
            return null;
        }

        /// <summary>
        /// Поиск Открытой сессии по госзнаку
        /// </summary>
        /// <param name="carPlateNum">госзнак</param>
        /// <returns>Класс ParkingSession</returns>
        public ParkingSession FindOpenSessionByCarPlateNumber(string carPlateNum)
        {
            foreach (ParkingSession session in ListSessionOpen)
            {
                if (session.CarPlateNumber == carPlateNum)
                {
                    return session;
                }
            }
            return null;
        }

        /// <summary>
        /// Сериализация изменений в листе с окрытыми сессиями
        /// </summary>
        public void SerializeChangesOfListSessionOpen()
        {
            var binform = new BinaryFormatter();
            using (var file = new FileStream("../../../open_session.dat", FileMode.OpenOrCreate))
            {
                binform.Serialize(file, ListSessionOpen);
            }
        }

        public void PrintAllOpenSession()
        {
            Console.WriteLine("_____________________\n" +
                "Open Sessions\n_____________________");
            foreach (var session in ListSessionOpen)
            {
                Console.WriteLine($"session {session.TicketNumber}   EntryDt:{session.EntryDt}\n   " +
                    $"    PaymentDt:{session.PaymentDt}\n       ExitDt:{session.ExitDt}" +
                $"\n       TotalPayment:{session.TotalPayment}\n       CarPlateNumber;{session.CarPlateNumber}\n" +
                $"       Статус в программе лояльности: {session.StatusLoyaltyProgram}");
            }
        }

        /// <summary>
        /// Сериализация закрытых сессий
        /// </summary>
        public void SerializeListSessionClosed()
        {
            var binform = new BinaryFormatter();
            using (var file = new FileStream("../../../closed_session.dat", FileMode.OpenOrCreate))
            {
                binform.Serialize(file, ListSessionClosed);
            }
        }

        public void PrintAllClosedSession()
        {
            Console.WriteLine("_____________________\n" +
                "Closed Sessions\n_____________________");
            foreach (var session in ListSessionClosed)
            {
                Console.WriteLine($"session {session.TicketNumber}   EntryDt: {session.EntryDt}\n " +
                    $"      PaymentDt: {session.PaymentDt}\n       ExitDt: {session.ExitDt}" +
                $"\n       TotalPayment: {session.TotalPayment}\n       CarPlateNumber:{session.CarPlateNumber}\n" +
                $"       Статус в программе лояльности: {session.StatusLoyaltyProgram}");
            }
        }

        public void AddLoyalUser(string name, string carPlateNum, string phone)
        {
            LoyaltyProgram.Add(new User(name, carPlateNum, phone));

            var binform = new BinaryFormatter();
            using (var file = new FileStream("../../../loyalty_progam.dat", FileMode.OpenOrCreate))
            {
                binform.Serialize(file, LoyaltyProgram);
            }
        }


        public User FindLoyalUser(string carPlateNum)
        {
            try
            {
                foreach (var user in LoyaltyProgram)
                {
                    if (user.CarPlateNumber == carPlateNum)
                    {
                        return user;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public void PrintAllUserinLoyalty()
        {
            Console.WriteLine("_____________________\n" +
                "Users in Loyalty Program\n_____________________");
            foreach(var user in LoyaltyProgram)
            {
                Console.WriteLine($"user {user.Name}\n" +
                    $"    carplatenumber: {user.CarPlateNumber}\n" +
                    $"    his phone: {user.Phone}");
            }
        }

    }
}
