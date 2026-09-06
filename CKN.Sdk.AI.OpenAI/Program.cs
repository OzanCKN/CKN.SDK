using System;
using System.Linq;
using System.Reflection;

namespace CKN.Sdk.AI.OpenAI;

public class Program
{
    public static void Main()
    {
        // Find Microsoft.Extensions.AI.OpenAI assembly
        var asm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Microsoft.Extensions.AI.OpenAI");
        if (asm == null) {
            Console.WriteLine("Assembly not loaded in current domain.");
            // Load it explicitly
            asm = Assembly.Load("Microsoft.Extensions.AI.OpenAI");
        }
        
        foreach (var type in asm.GetExportedTypes())
        {
            Console.WriteLine($"Type: {type.FullName}");
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (method.Name.Contains("As") || method.Name.Contains("Client"))
                {
                    Console.WriteLine($"  Method: {method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name))})");
                }
            }
        }
    }
}
