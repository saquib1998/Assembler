namespace Assembler;

internal class Program
{
    const string outputPath = "output.hack";
    static void Main(string[] args)
    {
        if (!IsValidArg(args)) return;

        var parser = new Parser();

        var instructions = parser.GetInstructions(args[0])
                                 .ParseLabels()
                                 .ParseInstructions();

        WriteToFile(outputPath, instructions);

    }

    static void WriteToFile(string filePath, IEnumerable<string> lines)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string line in lines)
            {
                writer.WriteLine(line); 
            }
        }
    }

    static bool IsValidArg(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Error: No arguments given");
            return false;
        }


        if (!args[0].EndsWith(".asm", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Error: The argument should be an .asm file");
            return false;
        }

        if (!File.Exists(args[0]))
        {
            Console.WriteLine($"The file {args[0]} does not exist!");
            return false;
        }

        return true;
    }
}
