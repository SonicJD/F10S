using System;
using CosmosKernel4.Commands;
using CosmosKernel4.Utils;

namespace CosmosKernel4.Core
{
    public class CommandProcessor
    {
        private CommandHistory history = new CommandHistory();
        private string currentDirectory = "/";

        public void Execute(string input)
        {
            // ❗ NO guardar "again" en el historial
            if (input != "again")
                history.Save(input);

            switch (input)
            {
                case "history":
                    history.Show();
                    break;

                case "again":
                    var cmd = history.Get();
                    if (!string.IsNullOrEmpty(cmd))
                    {
                        Console.WriteLine($"Executant: {cmd}");
                        Execute(cmd);
                    }
                    break;

                case "clear":
                case "pitstop":
                    Console.Clear();
                    break;

                case "grid":
                    DirectoryCommands.List(currentDirectory);
                    break;

                case "drs":
                    currentDirectory = DirectoryCommands.Change(currentDirectory);
                    break;

                case "build":
                    DirectoryCommands.Create(currentDirectory);
                    break;

                case "crash":
                    DirectoryCommands.Delete(currentDirectory);
                    break;

                case "radio":
                    FileCommands.Read(currentDirectory);
                    break;

                case "engine":
                    FileCommands.Write(currentDirectory);
                    break;

                case "rm":
                    FileCommands.Delete(currentDirectory);
                    break;

                case "calc":
                    CalculatorCommands.Run();
                    break;

                case "retire":
                    Cosmos.System.Power.Shutdown();
                    break;

                case "restart":
                    Cosmos.System.Power.Reboot();
                    break;

                case "briefing":
                    ShowHelp();
                    break;

                case "team":
                    SystemCommands.Info();
                    break;

                case "telemetry":
                    SystemCommands.Memory();
                    break;

                case "lap":
                    SystemCommands.Uptime();
                    break;

                default:
                    Console.WriteLine("Error: Comanda no reconeguda. Revisa el briefing.");
                    break;
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine("--- PIT WALL BRIEFING ---");
            Console.WriteLine("calc: Operacions aritmetiques (suma, resta, mult, div, mod, sqrt)");
            Console.WriteLine("grid: Llista contingut");
            Console.WriteLine("drs: Cambia directori");
            Console.WriteLine("build: Crea directori");
            Console.WriteLine("crash: Elimina directori");
            Console.WriteLine("radio: monstrar contingut d'un fitxer");
            Console.WriteLine("rm: Elimina fitxer");
            Console.WriteLine("team: Info del sistema");
            Console.WriteLine("telemetry: Memoria del sistema");
            Console.WriteLine("lap: Temps del funcionament del sistema");
            Console.WriteLine("pitstop: Neteja de pantalla");
            Console.WriteLine("engine: Escriure text");
            Console.WriteLine("retire: Apagar sistema");
            Console.WriteLine("restart: Reiniciar sistema");
            Console.WriteLine("briefing: Llista comandes");
        }
    }
}