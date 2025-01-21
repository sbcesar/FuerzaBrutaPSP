namespace FuerzaBruta;

using System;
using System.IO;
using System.Security.Cryptography;

public class HashDirectory
{
    public static void HashArguments(string filePath)
    {
        if (File.Exists(filePath))
        {
            
            FileInfo file = new FileInfo(filePath);
            
            using (SHA256 mySHA256 = SHA256.Create())
            {
                
                using (FileStream fileStream = file.Open(FileMode.Open))
                {
                    try
                    {
                        fileStream.Position = 0;
                        
                        byte[] hashValue = mySHA256.ComputeHash(fileStream);
                        
                        Console.Write($"{file.Name}: ");
                        PrintByteArray(hashValue);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"I/O Exception: {e.Message}");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"Access Exception: {e.Message}");
                    }
                }
                
            }
        }
        else
        {
            Console.WriteLine("The directory specified could not be found.");
        }
    }
    
    public static void PrintByteArray(byte[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Console.Write($"{array[i]:X2}");
            if ((i % 4) == 3) Console.Write(" ");
        }
        Console.WriteLine();
    }
}