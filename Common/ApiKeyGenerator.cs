using System.Security.Cryptography;

namespace ObserverScheduler.Common;

public class ApiKeyGenerator
{
    public static string GenerateAPIKey()
    {
        try
        {
            // Generate a random byte array with 32 bytes
            byte[] key = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            // Encode the byte array to a base64 string
            string apiKey = Convert.ToBase64String(key);

            return apiKey;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate API key", ex);
        }
    }
}