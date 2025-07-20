using System.Security.Cryptography;
using System.Text;

namespace Pinnacle.Helpers
{
    public class UserEncryption
    {
        private static string encryptionKey = "PinnacleApi2025";

        //public static string Encrypt(string clearText)
        //{
        //    byte[] bytes = Encoding.Unicode.GetBytes(clearText);
        //    using (Aes aes = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(encryptionKey, new byte[13]
        //        {
        //        73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
        //        100, 101, 118
        //        });
        //        aes.Key = rfc2898DeriveBytes.GetBytes(32);
        //        aes.IV = rfc2898DeriveBytes.GetBytes(16);
        //        using MemoryStream memoryStream = new MemoryStream();
        //        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        //        {
        //            cryptoStream.Write(bytes, 0, bytes.Length);
        //            cryptoStream.Close();
        //        }

        //        clearText = Convert.ToBase64String(memoryStream.ToArray());
        //    }

        //    return clearText;
        //}

        //public static string Decrypt(string cipherText)
        //{
        //    byte[] array = Convert.FromBase64String(cipherText);
        //    using (Aes aes = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(encryptionKey, new byte[13]
        //        {
        //        73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
        //        100, 101, 118
        //        });
        //        aes.Key = rfc2898DeriveBytes.GetBytes(32);
        //        aes.IV = rfc2898DeriveBytes.GetBytes(16);
        //        using MemoryStream memoryStream = new MemoryStream();
        //        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
        //        {
        //            cryptoStream.Write(array, 0, array.Length);
        //            cryptoStream.Close();
        //        }

        //        cipherText = Encoding.Unicode.GetString(memoryStream.ToArray());
        //    }

        //    return cipherText;
        //}

        public static string Encrypt(string clearText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(clearText); // Use UTF8 instead of Unicode
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(encryptionKey, new byte[13]
                {
            73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
            100, 101, 118
                });
                aes.Key = rfc2898DeriveBytes.GetBytes(32);
                aes.IV = rfc2898DeriveBytes.GetBytes(16);
                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.Close();
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            byte[] array = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(encryptionKey, new byte[13]
                {
            73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
            100, 101, 118
                });
                aes.Key = rfc2898DeriveBytes.GetBytes(32);
                aes.IV = rfc2898DeriveBytes.GetBytes(16);
                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(array, 0, array.Length);
                    cryptoStream.Close();
                    return Encoding.UTF8.GetString(memoryStream.ToArray()); // Use UTF8 instead of Unicode
                }
            }
        }

    }
}
