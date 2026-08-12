using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TravelWithCode.Infrastructure;

public class Ciper
{
    private readonly byte[] _key;

    public Ciper()
    {
        string privateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY");
    
        using (var sha256 = SHA256.Create())
        {
            _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(privateKey));
        }
    }

    public (string encrypted, string IV) Encrypt(string text)
    {
        if (string.IsNullOrEmpty(text))
            return (string.Empty, string.Empty);

        using (Aes aes = Aes.Create())
        {
            aes.Key = _key;
            aes.GenerateIV();

            using (MemoryStream ms = new MemoryStream())
            {
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs, Encoding.UTF8))
                    {
                        sw.Write(text);
                    }
                }

                return (Convert.ToBase64String(ms.ToArray()), Convert.ToBase64String(aes.IV));
            }
        }
    }

    public string Decrypt(string encryptedText, string ivText)
    {
        if (string.IsNullOrEmpty(encryptedText) || string.IsNullOrEmpty(ivText))
            return string.Empty;

        byte[] encrypted = Convert.FromBase64String(encryptedText);
        byte[] iv = Convert.FromBase64String(ivText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = _key;
            aes.IV = iv;

            using (MemoryStream ms = new MemoryStream(encrypted))
            using (ICryptoTransform decryptor = aes.CreateDecryptor())
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
    }
}