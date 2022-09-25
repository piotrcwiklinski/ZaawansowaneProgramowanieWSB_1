using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ZaawansowaneProgramowanieWSB_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Users> UsersList = new List<Users>();

            foreach (string line in System.IO.File.ReadLines(@"C:\Temp\Users.txt"))
            {
                string[] parts = line.Split(',');

                string user = parts[0];
                string pass = parts[1];
                string email = parts[2];
                string decryptedPass = EncryptionHelper.Decrypt(pass);
                UsersList.Add(new Users(user, decryptedPass, email));
            }

        Start:
            Console.WriteLine("MENU:");
            Console.WriteLine("1. Logowanie");
            Console.WriteLine("2. Rejestracja Nowego Użytkownika");
            Console.WriteLine("0. Wyjście z Programu");
            Console.WriteLine("Wybierz numer opcji.");
            var input = Console.ReadLine();



            bool successfull = false;
            while (!successfull)
            {

                if (input == "1")
                {
                    Console.WriteLine("Podaj nazwę użytkownika:");
                    var username = Console.ReadLine();
                    Console.WriteLine("Podaj hasło:");
                    var password = Console.ReadLine();


                    foreach (Users user in UsersList)
                    {
                        if (username == user.username && password == user.password)
                        {
                            Console.WriteLine("Udało Ci się zalogować do systemu !!!");
                            Console.ReadLine();
                            successfull = true;
                            break;
                        }
                    }

                    if (!successfull)
                    {
                        Console.WriteLine("Podano niewłaściwą nazwę użytkownika i/lub hasło. Spróbuj ponownie !!!");
                    }

                }

                else if (input == "2")
                {

                    Console.WriteLine("Wprowadź swoją nazwę użytkownika:");
                    var username = Console.ReadLine();

                    Console.WriteLine("Wprowadź swoje hasło:");
                    var password = Console.ReadLine();

                    Console.WriteLine("Wprowadź swój adres e-mail:");
                    var email = Console.ReadLine();

                    UsersList.Add(new Users(username, password, email));
                    using (StreamWriter writer = new StreamWriter(@"C:\Temp\Users.txt", true))
                    {
                        string encryptedPass = EncryptionHelper.Encrypt(password);
                        writer.WriteLine("{0},{1},{2}", username, encryptedPass, email);
                    }
                    successfull = true;
                    goto Start;

                }
                else if (input == "0")
                {

                    Console.WriteLine("Zapraszamy ponownie !!!");
                    break;

                }
                else
                {
                    Console.WriteLine("Nie ma takiej opcji. Spróbuj ponownie !!!");
                    goto Start;


                }

            }

        }
    }

    public class Users
    {
        public string username;
        public string password;
        public string email;

        public Users(string username, string password, string email)
        {
            this.username = username;
            this.password = password;
            this.email = email;
            
        }
    }

    public static class EncryptionHelper
    {
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
