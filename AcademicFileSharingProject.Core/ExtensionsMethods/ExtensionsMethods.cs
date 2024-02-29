using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Core.ExtensionsMethods
{
    public class ExtensionsMethods
    {
        public static string CalculateMD5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] bytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static string GenerateRandomString(int length)
        {
            // Rastgele karakterleri içeren bir karakter dizisi
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            Random random = new Random();
            char[] randomArray = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomArray[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomArray);
        }

        public static string GenerateRandomPassword(int length)
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digitChars = "0123456789";
            const string symbolChars = "!@#$%^&*()-_+=<>?";

            Random random = new Random();

            // Her kategoriden en az bir karakter seçmek için
            char[] password = new char[length];
            password[0] = uppercaseChars[random.Next(uppercaseChars.Length)];
            password[1] = lowercaseChars[random.Next(lowercaseChars.Length)];
            password[2] = digitChars[random.Next(digitChars.Length)];
            password[3] = symbolChars[random.Next(symbolChars.Length)];

            // Geri kalan karakterleri rastgele seçmek için
            const string allChars = uppercaseChars + lowercaseChars + digitChars + symbolChars;
            for (int i = 4; i < length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }

            // Karakterleri karıştırmak için
            password = password.OrderBy(c => random.Next()).ToArray();

            return new string(password);
        }

    }
}
