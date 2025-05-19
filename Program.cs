namespace atx2img;

class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: atx2img <input_atx_file> <output_directory>");
            return;
        }

        string inputFilePath = args[0];
        string outputDirectory = args[1];

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine($"Error: Input file not found at '{inputFilePath}'");
            return;
        }

        if (!Directory.Exists(outputDirectory))
        {
            try
            {
                Directory.CreateDirectory(outputDirectory);
                Console.WriteLine($"Output directory created at '{outputDirectory}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating output directory: {ex.Message}");
                return;
            }
        }

        Console.WriteLine($"Converting '{inputFilePath}' to images in '{outputDirectory}'...");

        try
        {
            AtxConverter converter = new AtxConverter(inputFilePath, outputDirectory);
            converter.Convert();

            Console.WriteLine("Conversion complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during conversion: {ex.Message}");
        }
    }
}
