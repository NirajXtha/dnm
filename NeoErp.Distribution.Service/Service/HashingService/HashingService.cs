using System;
using System.Security.Cryptography;
namespace NeoErp.Distribution.Service.Service.HashingService
{


    public static class PasswordHasher
    {
        // Recommended defaults (tune iterations upwards as CPU allows)
        private const int SaltSize = 16;        // 128 bit
        private const int HashSize = 32;        // 256 bit
        private const int DefaultIterations = 10000; // >= 10000 recommended for .NET4.5 era; increase if possible

        /// <summary>
        /// Hash a password with a generated salt. Returns a string formatted as:
        /// iterations:base64(salt):base64(hash)
        /// </summary>
        public static string HashPassword(string password, int iterations = DefaultIterations)
        {
            if (password == null) throw new ArgumentNullException("password");

            // Generate salt
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Derive hash
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            // Format: iterations:salt:hash (base64)
            string result = string.Format("{0}:{1}:{2}",
                iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash));

            return result;
        }

        /// <summary>
        /// Verifies a plaintext password against stored hashed value.
        /// Stored format must be iterations:base64(salt):base64(hash)
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (storedHash == null) throw new ArgumentNullException("storedHash");

            // Parse stored value
            var parts = storedHash.Split(':');
            if (parts.Length != 3) return false;

            int iterations;
            if (!int.TryParse(parts[0], out iterations)) return false;

            byte[] salt = Convert.FromBase64String(parts[1]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[2]);

            // Derive hash from provided password using same salt/iterations
            byte[] computedHash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                computedHash = pbkdf2.GetBytes(storedHashBytes.Length);
            }

            // Compare in constant time
            return AreByteArraysEqual(storedHashBytes, computedHash);
        }

        /// <summary>
        /// Constant-time comparison to prevent timing attacks.
        /// </summary>
        private static bool AreByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }

}
