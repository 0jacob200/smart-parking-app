using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ParkingApp
{
    /* TODO при окончании работы:
     * 1. закоментировать все DateTime dateForDebug
     * 2. ParkingCapacity = 500
     * 3. удалить дефолтные комменты
     */
    [Serializable]
    class ParkingManager
    {
        private static List<ParkingSession> ListSessionOpen = new List<ParkingSession>();
        private static List<ParkingSession> ListSessionClosed = new List<ParkingSession>();
        private static Dictionary<int, decimal> DictTariff = new Dictionary<int, decimal>();

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
            using (var file = new FileStream("../../../infoParkingManager.dat", FileMode.OpenOrCreate))
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
            using (var file = new FileStream("../../../opensession.dat", FileMode.OpenOrCreate))
            {
                ListSessionOpen = (List<ParkingSession>)binform.Deserialize(file);
            }

            var binform2 = new BinaryFormatter();
            using (var file = new FileStream("../../../closedsession.dat", FileMode.OpenOrCreate))
            {
                ListSessionClosed = (List<ParkingSession>)binform2.Deserialize(file);
            }
            var binform3 = new BinaryFormatter();
            using (var file = new FileStream("../../../infoParkingManager.dat", FileMode.Open))
            {
                ParkingCapacity = (int)binform3.Deserialize(file);
                DictTariff = (Dictionary<int, decimal>)binform3.Deserialize(file);
            }

            TicketId = ListSessionClosed.Count + ListSessionOpen.Count;
        }

        /* BASIC PART */
        public ParkingSession EnterParking(string carPlateNumber, DateTime dateForDebug) // TODO: убрать dateForDebug
        {
            #region
            /* Check that there is a free parking place (by comparing the parking capacity 
            with the number of active parking sessions). If there are no free places, return null
            
            Also check that there are no existing active sessions with the same car plate number,
            if such session exists, also return null
            
            Otherwise:
            Create a new Parking session, fill the following properties:
            EntryDt = current date time
            CarPlateNumber = carPlateNumber (from parameter)
            TicketNumber = unused parking ticket number = generate this programmatically
            
            Add the newly created session to the list of active sessions
            
            Advanced task:
            Link the new parking session to an existing user by car plate number (if such user exists)
            _____________________________

            Убедитесь, что есть свободное место на парковке (сравнив парковочную емкость
            с количеством активных парковочных сессий). Если свободных мест нет, верните null - DONE
            
            Также убедитесь, что нет активных сессий с таким же номером автомобиля,
            если такая сессия существует, также вернуть null - DONE
            
            В противном случае: - DONE
            Создайте новый сеанс парковки, заполните следующие свойства:
            EntryDt = текущая дата и время
            CarPlateNumber = carPlateNumber (из параметра)
            TicketNumber = номер неиспользованного парковочного билета = сгенерировать это программно
            
            Добавить вновь созданную сессию в список активных сессий -DONE

            TODO: 
            Расширенное задание:
            Свяжите новый сеанс парковки с существующим пользователем по номеру 
            автомобильной таблички (если такой пользователь существует)*/
            #endregion

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

        public decimal GetRemainingCost(int ticketNumber, DateTime dateForDebug) // TODO: убрать dateForDebug
        {
            #region
            /* Return the amount to be paid for the parking
             * If a payment had already been made but additional charge was then given
             * because of a late exit, this method should return the amount 
             * that is yet to be paid (not the total charge)
             *
             * вывести сумму, подлежащую оплате за парковку - DONE
             * Если оплата уже была произведена, но была произведена дополнительная оплата
             * из-за позднего выхода этот метод должен вернуть amount
             * это еще не оплачено (не общая сумма) _ DONE
             */
            #endregion

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

        public void PayForParking(int ticketNumber, decimal amount, DateTime dateForDebug) // TODO: убрать dateForDebug
        {
            #region
            /*
             * Save the payment details in the corresponding parking session
             * Set PaymentDt to current date and time
             * 
             * For simplicity we won't make any additional validation here and always
             * assume that the parking charge is paid in full
             *
             * Сохраните реквизиты платежа в соответствующей сессии парковки - DONE
             * Установите PaymentDt на текущую дату и время - DONE
             *
             * Для простоты мы не будем делать дополнительную проверку здесь и всегда
             * Предположим, что плата за парковку полностью оплачена
             */
            #endregion
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

        /* ADDITIONAL TASK 2 */ // TODO
        public bool TryLeaveParkingByCarPlateNumber(string carPlateNumber, out ParkingSession session, DateTime dateForDebug) 
        {
            #region
            /* There are 3 scenarios for this method:
            
            1. The user has not made any payments but leaves the parking within the free leave period
            from EntryDt:
               1.1 Complete the parking session by setting the ExitDt property
               1.2 Move the session from the list of active sessions to the list of past sessions
               1.3 return true and the completed parking session object in the out parameter
            
            2. The user has already paid for the parking session (session.PaymentDt != null):
            Check that the current time is within the free leave period from session.PaymentDt
               2.1. If yes, complete the session in the same way as in the previous scenario
               2.2. If no, return false, session = null

            3. The user has not paid for the parking session:            
            3a) If the session has a connected user (see advanced task from the EnterParking method):
            ExitDt = PaymentDt = current date time; 
            TotalPayment according to the tariff table:            
            
            IMPORTANT: before calculating the parking charge, subtract FreeLeavePeriod 
            from the total number of minutes passed since entry
            i.e. if the registered visitor enters the parking at 10:05
            and attempts to leave at 10:25, no charge should be made, otherwise it would be unfair
            to loyal customers, because an ordinary printed ticket could be inserted in the payment
            kiosk at 10:15 (no charge) and another 15 free minutes would be given (up to 10:30)

            return the completed session in the out parameter and true in the main return value

            3b) If there is no connected user, set session = null, return false (the visitor
            has to insert the parking ticket and pay at the kiosk)
            ___________________________________

            Есть 3 сценария для этого метода:
            
            1. Пользователь не сделал никаких платежей, но покидает парковку в течение периода бесплатного пропуска.
            от EntryDt:
               1.1 Завершите сессию парковки, установив свойство ExitDt
               1.2 Переместить сессию из списка активных сессий в список прошлых сессий
               1.3 вернуть true и завершенный объект парковки в параметре out
            
            2. Пользователь уже оплатил парковку (session.PaymentDt! = Null):
            Убедитесь, что текущее время находится в пределах периода бесплатного пропуска из сессии. PaymentDt
               2.1. Если да, завершите сеанс так же, как в предыдущем сценарии
               2.2. Если нет, вернуть false, session = null

            3. Пользователь не оплатил парковку:
            3a) Если у сеанса есть подключенный пользователь (см. Расширенную задачу из метода EnterParking):
            ExitDt = PaymentDt = текущая дата и время;
            TotalPayment согласно тарифной таблице:
            
            ВАЖНО: перед расчетом платы за парковку вычтите FreeLeavePeriod
            от общего количества минут, прошедших с момента входа
            т.е. если зарегистрированный посетитель заходит на парковку в 10:05
            и попытки уйти в 10:25 не должны быть предъявлены обвинения, иначе это было бы несправедливо
            постоянным клиентам, поскольку в оплату можно вставить обычный распечатанный билет
            киоск в 10:15 (бесплатно) и еще 15 бесплатных минут (до 10:30)

            (тоесть программа лояльности == без билета????) 

            вернуть завершенный сеанс в параметре out и true в основном возвращаемом значении

            3b) Если нет подключенного пользователя, установите session = null, верните false (посетитель
            должен вставить парковочный билет и оплатить в киоске)
            */
            #endregion

            session = FindOpenSessionByCarPlateNumber(carPlateNumber);
            bool flagOfAccessExit = false;


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
            using (var file = new FileStream("../../../opensession.dat", FileMode.OpenOrCreate))
            {
                binform.Serialize(file, ListSessionOpen);
            }
        }

        public void PrintAllOpenSessionForDebug()
        {
            Console.WriteLine("_____________________\n" +
                "Open Sessions\n_____________________");
            foreach (var session in ListSessionOpen)
            {
                Console.WriteLine($"session {session.TicketNumber}   EntryDt:{session.EntryDt}\n   " +
                    $"    PaymentDt:{session.PaymentDt}\n       ExitDt:{session.ExitDt}" +
                $"\n       TotalPayment:{session.TotalPayment}\n       CarPlateNumber;{session.CarPlateNumber}");
            }
        }

        /// <summary>
        /// Сериализация закрытых сессий
        /// </summary>
        public void SerializeListSessionClosed()
        {
            var binform = new BinaryFormatter();
            using (var file = new FileStream("../../../closedsession.dat", FileMode.OpenOrCreate))
            {
                binform.Serialize(file, ListSessionClosed);
            }
        }

        public void PrintAllClosedSessionForDebug()
        {
            Console.WriteLine("_____________________\n" +
                "Closed Sessions\n_____________________");
            foreach (var session in ListSessionClosed)
            {
                Console.WriteLine($"session {session.TicketNumber}   EntryDt:{session.EntryDt}\n " +
                    $"      PaymentDt:{session.PaymentDt}\n       ExitDt:{session.ExitDt}" +
                $"\n       TotalPayment:{session.TotalPayment}\n       CarPlateNumber;{session.CarPlateNumber}");
            }
        }

    }
}
