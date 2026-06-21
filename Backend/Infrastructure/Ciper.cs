using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TravelWithCode.Infrastructure;

public class Ciper
{
    private readonly string _privateKey;

    public Ciper()
    {
        _privateKey = Environment.GetEnvironmentVariable("PRIVATE_KEY");
    }

    public (string encrypted, string IV) Encrypt(string text)
    {   
        using(Aes aes = Aes.Create())
        {
            aes.Key = _privateKey;

            using(MemoryStream ms = new MemoryStream())
            {   
                using(ICryptoTransform encryptor = aes.CreateEncryptor())
                using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using(StreamWriter sw = new StreamWriter(cs))
                {
                    sw.Write(text);
                }
            }

            return (Convert.ToBase64String(ms.ToArray()), Convert.ToBase64String(aes.IV));
        }
    }

    public string Decrypt(string encryptedText, string ivText)
    {
        byte[] encrypted = Convert.FromBase64String(encrypted);
        byte[] iv = Convert.FromBase64String(encrypted);

        using(Aes aes = Aes.Create())
        {
            aes.Key = _privateKey;
            aes.IV = iv;

            using(MemoryStream ms = new MemoryStream(encrypted))
            using(ICryptoTransform decryptor = aes.CreateDecryptor())
            using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using(StreamReader sr = new StreamReader(cs))

            return sr.ReadToEnd();
        }
    }
}