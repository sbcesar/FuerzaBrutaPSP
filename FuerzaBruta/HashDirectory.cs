using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace FuerzaBruta
{
    public class HashDirectory
    {
        private static string targetHash; // Almacena el hash de la contraseña objetivo
        private static string crackedPassword = null; // Variable para almacenar la contraseña encontrada
        private static object lockObject = new object(); // Objeto para sincronización de hilos
        private static ManualResetEvent doneEvent; // Evento para coordinar finalización de hilos

        public static void HashArguments(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }

            List<string> passwords = new List<string>(File.ReadAllLines(filePath));
            
            Random random = new Random();
            string chosenPassword = passwords[random.Next(passwords.Count)];
            targetHash = HashPassword(chosenPassword);

            Console.WriteLine($"Contraseña seleccionada: {chosenPassword}");
            Console.WriteLine($"Hash de la contraseña: {targetHash}\n");
            
            Stopwatch stopwatch = Stopwatch.StartNew();

            int numThreads = 5; // Número de hilos a utilizar
            doneEvent = new ManualResetEvent(false); // Evento para señalizar la finalización
            int chunkSize = passwords.Count / numThreads; // Tamaño de cada bloque de contraseñas
            int remaining = passwords.Count % numThreads; // Resto de contraseñas que no caben en los bloques exactos
            int startIndex = 0;
            int threadsCompleted = 0;

            // Divide la lista de contraseñas en partes en funcion de los hilos que hay
            for (int i = 0; i < numThreads; i++)
            {
                int endIndex = startIndex + chunkSize + (i < remaining ? 1 : 0);
                List<string> chunk = passwords.GetRange(startIndex, endIndex - startIndex);
                startIndex = endIndex;

                ThreadPool.QueueUserWorkItem(state =>
                {
                    BruteForceAttack(chunk);
                    if (Interlocked.Increment(ref threadsCompleted) == numThreads)
                    {
                        doneEvent.Set();
                    }
                });
            }

            doneEvent.WaitOne();
            stopwatch.Stop();

            if (crackedPassword != null)
            {
                Console.WriteLine($"¡Contraseña encontrada!: {crackedPassword}");
            }
            else
            {
                Console.WriteLine("No se pudo encontrar la contraseña en el diccionario.");
            }
            
            Console.WriteLine($"Tiempo transcurrido: {stopwatch.ElapsedMilliseconds} ms");
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private static void BruteForceAttack(List<string> passwordList)
        {
            foreach (string password in passwordList)
            {
                if (crackedPassword != null) return;
                string hashedPassword = HashPassword(password);

                if (hashedPassword == targetHash)
                {
                    lock (lockObject)   // Bloquea para evitar condiciones de carrera (si varios hilos encuentran la contraseña e intentan asignarlo a la misma variable)
                    {
                        if (crackedPassword == null)
                        {
                            crackedPassword = password;
                        }
                    }
                    return;
                }
            }
        }
    }
}