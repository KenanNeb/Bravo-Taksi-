using Bravo_Taksi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bravo_Taksi.Auxiliary
{
    public class FileFolder
    {
        private static string[] FileName = new string[3] { "Admin.bin", "User.bin" ,"Driver.bin"};
        public static void BinWrite(User user, int n)
        {
            string Filename = FileName[n];
            using (FileStream fs = new FileStream(Filename, FileMode.Append, FileAccess.Write))
            {
                using (BinaryWriter br = new BinaryWriter(fs))
                {
                    br.Write(user.Email); br.Write(user.Password);
                }
            }
        }
        public static void BinDriverWrite(Driver driver, int n)
        {
            string Filename = FileName[n];
            using (FileStream fs = new FileStream(Filename, FileMode.Append, FileAccess.Write))
            {
                using (BinaryWriter br = new BinaryWriter(fs))
                {
                    br.Write(driver.Name);
                    br.Write(driver.Surname);
                    br.Write(driver.DriverNumber);
                    br.Write(driver.Email);
                    br.Write(driver.CarVendor);
                    br.Write(driver.CarModel);
                    br.Write(driver.CarNumber);
                    br.Write(driver.CarColor);
                }
            }
        }

        public static void NewPassword(string Email, string Password)
        {

            int index = 1;
            using (FileStream fs = new FileStream("CopyBin.bin", FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter br = new BinaryWriter(fs))
                {
                    using (FileStream fs2 = new FileStream(FileName[index], FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br2 = new BinaryReader(fs2))
                        {
                            for (int i = 0; i < fs2.Length;)
                            {
                                string text = br2.ReadString();
                                i += text.Length + 1;
                                if (text == Email)
                                {
                                    br.Write(Email);
                                    br.Write(Password);
                                    i += br2.ReadString().Length + 1;
                                }
                                else
                                {
                                    string pass = br2.ReadString();
                                    i += pass.Length + 1;
                                    br.Write(text); br.Write(pass);
                                }
                            }
                        }
                    }
                }
            }
            using (FileStream fs2 = new FileStream(FileName[index], FileMode.Create, FileAccess.Write)) { }
            File_Save("CopyBin.bin");
        }

        private static void File_Save(string filename)
        {

            using (FileStream fs = new FileStream(FileName[1], FileMode.Append, FileAccess.Write))
            {
                using (BinaryWriter br = new BinaryWriter(fs))
                {
                    using (FileStream fs2 = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (BinaryReader br2 = new BinaryReader(fs2))
                        {
                            for (int i = 0; i < fs2.Length;)
                            {
                                string text = br2.ReadString();
                                i += text.Length + 1;
                                string pass = br2.ReadString();
                                i += pass.Length + 1;
                                br.Write(text); br.Write(pass);
                            }
                        }
                    }
                }
            }
        }

        public static bool BinEmailRead(string Email, int n)
        {

            string Filename = FileName[n];
            using (FileStream fs = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    for (int i = 0; i < fs.Length;)
                    {
                        string text = br.ReadString();
                        i += text.Length + 1;
                        if (text == Email) return true;
                    }
                }
            }
            return false;
        }


        public static bool BinRead(string Name, string Password, int n)
        {

            string Filename = FileName[n];
            using (FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    for (int i = 0; i < fs.Length;)
                    {
                        string text = br.ReadString();
                        i += text.Length + 1;

                        if (text == Name)
                        {
                            string text2 = br.ReadString();

                            if (text2 == Password)
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }

            return false;
        }
    }
}
