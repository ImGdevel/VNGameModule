using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


namespace Utils
{
    public class Encryption : MonoBehaviour
    {
        private readonly static string key = "GameSecretKey"; 
        private readonly static string iv = "GameInitializationVector";

        public static string EncryptAES(string plainText)
        {
            using (AesManaged aesAlg = new AesManaged()) {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string DecryptAES(string cipherText)
        {
            using (AesManaged aesAlg = new AesManaged()) {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText))) {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string EncryptHash(string input)
        {
            // SHA-256 해시 알고리즘 사용
            using (SHA256 sha256 = SHA256.Create()) {
                // 문자열을 바이트 배열로 변환
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // 해시 계산
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // 바이트 배열을 문자열로 변환
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    builder.Append(hashBytes[i].ToString("x2")); // x2는 16진수 표현을 의미
                }

                return builder.ToString();
            }
        }
    }

}


