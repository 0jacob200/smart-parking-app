using System;

namespace ParkingApp
{
    [Serializable]
    class User
    {
        public string Name { get; set; }
        public string CarPlateNumber { get; set; }
        public string Phone { get; set; }

        public User(string name, string carPlateNum, string phone)
        {
            Name = name;
            CarPlateNumber = carPlateNum;
            Phone = phone;
        }
    }
}
