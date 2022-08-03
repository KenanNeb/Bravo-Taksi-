using System;

namespace Bravo_Taksi.Models
{
    public class Driver
    {
        public Driver(string name, string surname, string carVendor, string carModel, string carNumber, string carColor, string email, string driverNumber)
        {
            Name = name;
            Surname = surname;
            CarVendor = carVendor;
            CarModel = carModel;
            CarNumber = carNumber;
            CarColor = carColor;
            Email = email;
            DriverNumber = driverNumber;
            //Balance = balance;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string CarVendor { get; set; }
        public string CarModel { get; set; }
        public string CarNumber { get; set; }
        public string CarColor { get; set; }
        public string Email { get; set; }
        public string DriverNumber { get; set; }
        public double Balance { get; set; } = -5;


    }
}
