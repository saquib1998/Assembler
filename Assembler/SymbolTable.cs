namespace Assembler;

public static class SymbolTable
{
    // Static dictionaries for default symbols
    private static readonly Dictionary<string, int> registers;
    private static readonly Dictionary<string, int> symbols;

    private static int nextVariableAddress = 15; 

    static SymbolTable()
    {
        // Initialize registers (R0 -> 0, R1 -> 1, ..., R15 -> 15)
        registers = [];
        for (int n = 0; n <= 15; n++)
        {
            registers[$"R{n}"] = n;
        }

        // Initialize symbols
        symbols = new Dictionary<string, int>
        {
            { "SP", 0 },
            { "LCL", 1 },
            { "ARG", 2 },
            { "THIS", 3 },
            { "THAT", 4 },
            { "SCREEN", 16384 },
            { "KBD", 24576 }
        };

        // Merge registers into symbols
        foreach (var register in registers)
        {
            symbols[register.Key] = register.Value;
        }
    }

    public static void Add(string symbolName, int address)
    {
        if (Get(symbolName).HasValue)
        {
            throw new InvalidOperationException($"Symbol already defined: {symbolName}");
        }
        symbols[symbolName] = address;
    }

    public static int? Get(string symbolName)
    {
        if (!symbols.ContainsKey(symbolName)) return null;
        return symbols[symbolName]; 
    }

    public static int GetNextVarAddress()
    {
        nextVariableAddress++;
        return nextVariableAddress;
    }
}
