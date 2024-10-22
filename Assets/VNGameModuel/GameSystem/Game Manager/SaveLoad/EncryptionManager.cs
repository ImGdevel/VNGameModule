using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionManager
{
    private readonly static string key = "GameSecretKey"; // 16, 24, or 32 bytes key for AES-128, AES-192, or AES-256
    private readonly static string iv = "GameInitializationVector"; // 16 bytes IV for AES

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
        // SHA-256 �ؽ� �˰��� ���
        using (SHA256 sha256 = SHA256.Create()) {
            // ���ڿ��� ����Ʈ �迭�� ��ȯ
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // �ؽ� ���
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            // ����Ʈ �迭�� ���ڿ��� ��ȯ
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++) {
                builder.Append(hashBytes[i].ToString("x2")); // x2�� 16���� ǥ���� �ǹ�
            }

            return builder.ToString();
        }
    }
}
