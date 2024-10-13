namespace Assembler;

internal class Parser
{
    private List<string> instructions;
    public Parser()
    {
        instructions = [];
    }

    public Parser GetInstructions(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            var trimmedLine = line.Replace(" ", "");
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("//"))
                continue;

            if(trimmedLine.Contains("//"))
            {
                var temp = trimmedLine.Split("//");
                trimmedLine = temp[0];
            }

            instructions.Add(trimmedLine);
        }

        return this;
    }

    public Parser ParseLabels()
    {
        var newInstructions = new List<string>();
        var lineNumber = 0;
        foreach (string instruction in instructions)
        {
            if (GetInstructionType(instruction) != InstructionType.Label)
            {
                newInstructions.Add(instruction);
                lineNumber++;
                continue;
            }

            ParseLabel(instruction, lineNumber);
        }
        instructions = newInstructions;
        return this;
    }

    public IEnumerable<string> ParseInstructions()
    {
        var parsedInstructions = new List<string>();
        foreach (var instruction in instructions) {
            
            if (GetInstructionType(instruction) == InstructionType.Label)
            {
                continue;
            }


            parsedInstructions.Add(ParseInstruction(instruction));
        }
        instructions = parsedInstructions;
        return instructions;
    }

    private static string ParseInstruction(string instruction)
        => GetInstructionType(instruction) switch
        {
            InstructionType.A => ParseAInstruction(instruction),
            InstructionType.Label => throw new InvalidOperationException("This method doesn't parse Labels"),
            _ => ParseCInstruction(instruction),
        };

    private static string ParseLabel(string instruction, int lineNumber)
     {
        var label = instruction.Replace("(", string.Empty).Replace(")", string.Empty);
        var value = SymbolTable.Get(label);

        if (value is null)
        {
            SymbolTable.Add(label, lineNumber);
        }
        return string.Empty;
    }

    private static string ParseCInstruction(string instruction)
    {
        var dest = "null";
        var comp = string.Empty;
        var jump = "null";

        if(instruction.Contains('='))
        {
            var split = instruction.Split('=');
            dest = split[0];


            var furtherSplit = split[1].Split(';');
            comp = furtherSplit[0];
            jump = furtherSplit.Length > 1 ? furtherSplit[1] : "null";
        } 
        else
        {
            var split = instruction.Split(';');
            comp = split[0];
            jump = split.Length > 1 ? split[1] : "null";
        }

        
        
        return "111" + Code.Comp[comp] + Code.Dest[dest] + Code.Jump[jump]; 
    }

    private static string ParseAInstruction(string instruction)
    {
        var symbol = instruction.Replace("@", string.Empty);

        if (int.TryParse(symbol, out var code))
        {
            return Convert.ToString(code, 2).PadLeft(16, '0');
        }

        int? value = SymbolTable.Get(symbol);

        if(value is null)
        {
            value = SymbolTable.GetNextVarAddress();
            SymbolTable.Add(symbol, value.Value);
        }

        return Convert.ToString(value.Value, 2).PadLeft(16, '0');
    }

    private static InstructionType GetInstructionType(string instruction)
        => instruction switch
        {
            _ when instruction.StartsWith('@') => InstructionType.A,
            _ when instruction.StartsWith('(') => InstructionType.Label,
            _ => InstructionType.C
        };

    private enum InstructionType
    {
        A, 
        C, 
        Label
    }
}
