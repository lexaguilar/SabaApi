namespace Saba.Application.Helpers;

internal static class PasswordHelper
{
    public static string GeneratePassword(int length, int upperCaseCount, int lowerCaseCount, int digitCount, int specialCharCount)
    {
        const string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
        
        var passwordChars = new List<char>();
        
        var random = new Random();
        
        for (int i = 0; i < upperCaseCount; i++)
        {
            passwordChars.Add(upperCaseChars[random.Next(upperCaseChars.Length)]);
        }
        
        for (int i = 0; i < lowerCaseCount; i++)
        {
            passwordChars.Add(lowerCaseChars[random.Next(lowerCaseChars.Length)]);
        }
        
        for (int i = 0; i < digitCount; i++)
        {
            passwordChars.Add(digits[random.Next(digits.Length)]);
        }
        
        for (int i = 0; i < specialCharCount; i++)
        {
            passwordChars.Add(specialChars[random.Next(specialChars.Length)]);
        }
        
        while (passwordChars.Count < length)
        {
            passwordChars.Add(upperCaseChars[random.Next(upperCaseChars.Length)]);
        }
        
        return new string(passwordChars.OrderBy(c => random.Next()).ToArray());
    }
}