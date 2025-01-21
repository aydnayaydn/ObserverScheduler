using System.Security.Cryptography;
using System.Text;

namespace ObserverScheduler.Helper;

public class EncryptionHelper
{   
    private static byte[] GetCipherKey()
    {
        return File.ReadAllBytes("cipher.txt"); // Must be 32 bytes
    }

    public static string Encrypt(string plaintext, out string tag)
    {
        byte[] key = GetCipherKey();
        byte[] nonce = new byte[12]; // GCM standard nonce size
        byte[] tagBytes = new byte[16]; // GCM standard tag size
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

        byte[] ciphertext = new byte[plaintextBytes.Length];
        using (var cipher = new AesGcm(key))
        {
            cipher.Encrypt(nonce, plaintextBytes, ciphertext, tagBytes);
            tag = Convert.ToBase64String(tagBytes);
        }

        return Convert.ToBase64String(ciphertext);
    }

    public static string Decrypt(string encryptedText, string tag)
    {
        byte[] key = GetCipherKey();
        byte[] nonce = new byte[12]; // GCM standard nonce size 
        byte[] ciphertext = Convert.FromBase64String(encryptedText);

        byte[] decryptedData = new byte[ciphertext.Length];
        using (var cipher = new AesGcm(key))
        {
            cipher.Decrypt(nonce, ciphertext, Convert.FromBase64String(tag), decryptedData);
        }

        return Encoding.UTF8.GetString(decryptedData);
    }
}