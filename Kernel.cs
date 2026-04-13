using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace CosmosKernel1
{
    public class Kernel : Sys.Kernel
    {
        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("F1OS ha arrancat correctament. Benvingut al Pit Lane.");
            Console.WriteLine("Escriu 'briefing' per veure les comandes disponibles.");
        }

        protected override void Run()
        {
            Console.Write("\nComanda (briefing per veure tot): ");
            var input = Console.ReadLine().ToLower().Trim();

            switch (input)
            {
                case "briefing":
                    MostrarBriefing();
                    break;

                case "retire":
                    Console.WriteLine("Apagant el motor... Adéu!");
                    Sys.Power.Shutdown(); // Nota: En versions recents de Cosmos s'usa Sys.Power
                    break;

                case "restart":
                    Console.WriteLine("Reiniciant el monoplaça...");
                    Sys.Power.Reboot();
                    break;

                case "pitstop":
                    Console.Clear();
                    break;

                case "calc":
                    EjecutarCalculadora();
                    break;

                default:
                    if (!string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("Error: Comanda no reconeguda. Revisa el briefing.");
                    }
                    break;
            }
        }

        private void MostrarBriefing()
        {
            Console.WriteLine("--- PIT WALL BRIEFING ---");
            Console.WriteLine("calc: Operacions aritmetiques (suma, resta, mult, div, mod, sqrt)");
            Console.WriteLine("pitstop: Neteja de pantalla");
            Console.WriteLine("retire: Apagar sistema");
            Console.WriteLine("restart: Reiniciar sistema");
            Console.WriteLine("briefing: Llista comandes");
        }

        private void EjecutarCalculadora()
        {
            try
            {
                Console.Write("Introdueix l'operacio (ex: suma, resta, mult, div, mod, sqrt): ");
                string op = Console.ReadLine().ToLower();

                Console.Write("Primer numero: ");
                double n1 = double.Parse(Console.ReadLine());

                if (op == "sqrt")
                {
                    Console.WriteLine($"Resultat: {Math.Sqrt(n1)}");
                }
                else
                {
                    Console.Write("Segon numero: ");
                    double n2 = double.Parse(Console.ReadLine());

                    switch (op)
                    {
                        case "suma": Console.WriteLine($"Resultat: {n1 + n2}"); break;
                        case "resta": Console.WriteLine($"Resultat: {n1 - n2}"); break;
                        case "mult": Console.WriteLine($"Resultat: {n1 * n2}"); break;
                        case "div":
                            if (n2 != 0) Console.WriteLine($"Resultat: {n1 / n2}");
                            else Console.WriteLine("Error: Divisio per zero.");
                            break;
                        case "mod": Console.WriteLine($"Resultat: {n1 % n2}"); break;
                        default: Console.WriteLine("Operacio no valida."); break;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error: Format de numero incorrecte.");
            }
        }
    }
}