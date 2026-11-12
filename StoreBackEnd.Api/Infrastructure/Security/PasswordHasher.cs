using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace StoreBackEnd.Api.Infrastructure.Security
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int DegreeOfParallelism = 4;
        private const int Iterations = 4;
        private const int MemorySizeKb = 65536;  
        public static string Hasher(string motDePasse)
        {
            var sel = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = CalculerHash(motDePasse, sel);

            return $"{Convert.ToBase64String(sel)}:{Convert.ToBase64String(hash)}";
        }

        public static bool Verifier(string motDePasse, string motDePasseHash)
        {
            var parts = motDePasseHash.Split(':');
            if (parts.Length != 2) return false;

            var sel = Convert.FromBase64String(parts[0]);
            var hashAttendu = Convert.FromBase64String(parts[1]);
            var hashCalcule = CalculerHash(motDePasse, sel);

            return CryptographicOperations.FixedTimeEquals(hashCalcule, hashAttendu);
        }

        private static byte[] CalculerHash(string motDePasse, byte[] sel)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(motDePasse))
            {
                Salt = sel,
                DegreeOfParallelism = DegreeOfParallelism,
                Iterations = Iterations,
                MemorySize = MemorySizeKb
            };

            return argon2.GetBytes(HashSize);
        }
    }
}