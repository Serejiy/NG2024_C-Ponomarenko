using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Console.Write("Your input = ");
        string input = Console.ReadLine();
        string hash = ComputeSha256Hash(input);
        Console.WriteLine($"SHA-256 hash of '{input}' is: {hash}");
    }

    static string ComputeSha256Hash(string rawData)
    {
        // Создаем SHA256 объект
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Вычисляем хэш - возвращает байтовый массив
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Конвертируем байтовый массив в строку
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
