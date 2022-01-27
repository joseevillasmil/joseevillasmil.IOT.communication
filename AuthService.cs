using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace joseevillasmil.IOT.Communication
{
    public static class AuthService
    {
        private static string sharedSecret = "49c4fc41489762ee70567c8414e0019cb2140cf58f3d2ba829a37405c1f9c9b0";
        private static byte[] _salt = Encoding.UTF8.GetBytes("81eeecf7c42ed43928e359f0fffa2432");
        public static string EncryptStringAES(string plainText)
        {

            string outStr = null;
            RijndaelManaged aesAlg = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return outStr;
        }

        public static string DecryptStringAES(string cipherText)
        {

            RijndaelManaged aesAlg = null;
            string plaintext = null;

            try
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {

                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    
        public static string GenerateToken( string role)
        {
            Objects.AuthToken token = new Objects.AuthToken()
            {
                startAt = DateTime.UtcNow,
                endsAt =  DateTime.UtcNow.AddMinutes(60),
                role = role
            };

            token.Signature = EncryptStringAES(token.ToString());
            string json = JsonSerializer.Serialize(token);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        public static bool ValidateToken(string _token)
        {
            Objects.AuthToken token = JsonSerializer.Deserialize<Objects.AuthToken>(Encoding.UTF8.GetString( Convert.FromBase64String(_token) ));
            if (token.startAt <= DateTime.UtcNow && token.endsAt >= DateTime.UtcNow)
            {
                string comp1 = DecryptStringAES(token.Signature);
                string comp2 = token.ToString();
                if (String.Equals(comp1, comp2))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
