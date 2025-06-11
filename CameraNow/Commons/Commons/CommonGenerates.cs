using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Commons.Commons
{
    public static class CommonGenerates
    {
        public static string GenerateDefaultPassword(IdentityOptions options)
        {
            var random = new Random();
            var password = new StringBuilder();

            password.Append(GetRandomCharacter("0123456789", random));
            password.Append(GetRandomCharacter("abcdefghijklmnopqrstuvwxyz", random));
            password.Append(GetRandomCharacter("ABCDEFGHIJKLMNOPQRSTUVWXYZ", random));
            password.Append(GetRandomCharacter("!@#$%^&*()-_+=<>?{}[]", random));

            string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+=<>?{}[]";

            for (int i = password.Length; i < options.Password.RequiredLength; i++)
            {
                password.Append(GetRandomCharacter(allChars, random));
            }

            return new string(password.ToString().OrderBy(c => random.Next()).ToArray());
        }

        private static char GetRandomCharacter (string chars, Random random)
        {
            return chars[random.Next (chars.Length)];
        }
    }
}
