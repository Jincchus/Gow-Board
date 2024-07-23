using System;
using System.Linq;

namespace GowBoard.Utility
{
    public static class AuthNumberGenerator
    {
        private static readonly Random random = new Random();

        public static string GenerateAuthNumber()
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}