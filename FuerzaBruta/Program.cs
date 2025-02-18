using FuerzaBruta;

public class Program
{
    public static void Main()
    {
        string fileName = "2151220-passwords.txt";

        // Subir tres niveles desde la carpeta bin para llegar a la raíz del proyecto
        string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;

        // Construir la ruta final al archivo
        string filePath = Path.Combine(projectDirectory, fileName);
        
        HashDirectory.HashArguments(filePath);
        
    }
}