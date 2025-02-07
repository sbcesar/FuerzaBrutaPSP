using System.Text;

namespace FuerzaBruta;

using System;
using System.IO;
using System.Security.Cryptography;

public class HashDirectory
{
    public static void HashArguments(string filePath)
    {

        // Verificar si existe el archivo
        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found");
            return;
        }

        List<string> passwords = new List<string>(File.ReadAllLines(filePath));

        // Escoge una contraseña aleatoria y la hashea
        Random random = new Random();
        string chosenPassword = passwords[random.Next(passwords.Count)];
        string hashedChosenPassword = HashPassword(chosenPassword);
        
        Console.WriteLine($"Contraseña seleccionada: {chosenPassword}");
        Console.WriteLine($"Hash de la contraseña: {hashedChosenPassword}\n");
        
        string crackedPassword = BruteForceAttack(passwords, hashedChosenPassword);

        if (crackedPassword != null)
        {
            Console.WriteLine($"¡Contraseña encontrada!: {crackedPassword}");
        }
        else
        {
            Console.WriteLine("No se pudo encontrar la contraseña en el diccionario.");
        }

        // Método que genera el hash
        // Crea una instancia del algoritmo -> Convierte la contraseña en bytes -> La hashea -> La devuelve en hex
        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        // Compara los hashes
        static string BruteForceAttack(List<string> passwordList, string targetHash)
        {
            foreach (string password in passwordList)
            {
                string hashedPassword = HashPassword(password);

                if (hashedPassword == targetHash)
                {
                    return password;
                }
            }

            return null;
        }
    }
}