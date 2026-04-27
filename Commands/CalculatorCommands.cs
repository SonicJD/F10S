using System;

namespace CosmosKernel4.Commands
{
    public static class CalculatorCommands
    {
        public static void Run()
        {
            Console.Write("Introdueix l'operacio (ex: suma, resta, mult, div, mod, sqrt): ");
            var op = Console.ReadLine();

            Console.Write("Primer numero: ");
            double n1 = double.Parse(Console.ReadLine());

            if (op == "sqrt")
            {
                Console.WriteLine(Math.Sqrt(n1));
                return;
            }

            Console.Write("Segon numero: ");
            double n2 = double.Parse(Console.ReadLine());

            switch (op)
            {
                case "suma": Console.WriteLine(n1 + n2); break;
                case "resta": Console.WriteLine(n1 - n2); break;
                case "mult": Console.WriteLine(n1 * n2); break;
                case "div": Console.WriteLine(n1 / n2); break;
            }
        }
    }
}