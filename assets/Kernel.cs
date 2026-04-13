using System;
using System.IO;
using Sys = Cosmos.System;

namespace CosmosKernel4
{
    public class Kernel : Sys.Kernel
    {
        private string currentDirectory = "/";

        private string NormalizePath(string dir)
        {
            if (string.IsNullOrWhiteSpace(dir))
            {
                return currentDirectory;
            }

            if (dir.StartsWith("/"))
            {
                return dir;
            }

            if (currentDirectory == "/")
            {
                return "/" + dir;
            }

            return currentDirectory.TrimEnd('/') + "/" + dir;
        }

        private string ToVfsPath(string normPath)
        {
            if (string.IsNullOrWhiteSpace(normPath)) normPath = currentDirectory;
            if (normPath == "/") return "0:\\";
            if (normPath.StartsWith("/"))
            {
                var rest = normPath.TrimStart('/').Replace('/', '\\');
                return "0:\\" + rest;
            }

            if (normPath.Contains(":")) return normPath;

            // relative to currentDirectory
            var cur = ToVfsPath(currentDirectory);
            if (!cur.EndsWith("\\")) cur += "\\";
            return cur + normPath.Replace('/', '\\');
        }

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("F1OS ha arrancat correctament. Benvingut al Pit Lane.");
            Console.WriteLine("Escriu 'briefing' per veure les comandes disponibles.");
            Sys.KeyboardManager.SetKeyLayout(new Sys.ScanMaps.ESStandardLayout());
            Console.WriteLine("Debug: Registrant VFS...");
            try
            {
                Sys.FileSystem.CosmosVFS fs = new Cosmos.System.FileSystem.CosmosVFS();
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
                Console.WriteLine("Debug: VFS registrat correctament.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Debug: Error registrant VFS: {ex.Message}");
            }
        }

        protected override void Run()
        {
            Console.Write("\nComanda (briefing per veure tot!!!): ");
            var rawInput = Console.ReadLine();
            var input = (rawInput ?? string.Empty).ToLower().Trim();

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

                case "grid":
                    grid();
                    break;

                case "drs":
                    drs();
                    break;

                case "build":
                    build();
                    break;

                case "crash":
                    crash();
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
            Console.WriteLine("grid: Llista contingut");
            Console.WriteLine("drs: Cambia directori");
            Console.WriteLine("build: Crea directori");
            Console.WriteLine("crash: Elimina directori");
            Console.WriteLine("radio: monstrar contingut d'un fitxer");
            Console.WriteLine("team: Info del sistema");
            Console.WriteLine("telemetry: Memoria del sistema");
            Console.WriteLine("lap: Temps del funcionament del sistema");
            Console.WriteLine("pitstop: Neteja de pantalla");
            Console.WriteLine("engine: Escriure text");
            Console.WriteLine("retire: Apagar sistema");
            Console.WriteLine("restart: Reiniciar sistema");
            Console.WriteLine("briefing: Llista comandes");
        }

        private void grid()
        {
            var files_list = Directory.GetFiles(@"0:\");
            var directory_list = Directory.GetDirectories(@"0:\");

            foreach (var file in files_list) 
            {
                Console.WriteLine(file);
            }
            foreach (var directory in directory_list)
            {
                Console.WriteLine(directory);
            }
        }

        private void drs()
        {
            Console.Write("Introdueix el directori a canviar: ");
            string dir = Console.ReadLine() ?? string.Empty;
            var newDir = NormalizePath(dir);
            try
            {
                var files = Sys.FileSystem.VFS.VFSManager.GetDirectoryListing(ToVfsPath(newDir));
                if (files == null)
                {
                    Console.WriteLine("Error: Directori no trobat o buit.");
                    return;
                }

                currentDirectory = newDir;
                Console.WriteLine($"Directori canviat a: {currentDirectory}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Directori no trobat. {ex.Message}");
            }
        }

        private void build()
        {
            Console.Write("Introdueix el nom del directori a crear: ");
            string dir = Console.ReadLine() ?? string.Empty;
            var path = NormalizePath(dir);
            try
            {
                Sys.FileSystem.VFS.VFSManager.CreateDirectory(ToVfsPath(path));
                Console.WriteLine($"Directori '{path}' creat correctament.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: No s'ha pogut crear el directori. {ex.Message}");
            }
        }

        private void crash()
        {
            Console.Write("Introdueix el nom del directori a eliminar: ");
            string dir = Console.ReadLine() ?? string.Empty;
            var path = NormalizePath(dir);
            try
            {
                // DeleteDirectory requires a recursive flag in this Cosmos version
                Sys.FileSystem.VFS.VFSManager.DeleteDirectory(ToVfsPath(path), true);
                Console.WriteLine($"Directori '{path}' eliminat correctament.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: No s'ha pogut eliminar el directori. ({ex.GetType().Name}) {ex.Message}");
            }
        }


        private void EjecutarCalculadora()
        {
            Console.Write("Introdueix l'operacio (ex: suma, resta, mult, div, mod, sqrt): ");
            var opRaw = Console.ReadLine();
            var op = (opRaw ?? string.Empty).ToLower().Trim();

            Console.Write("Primer numero: ");
            var s1 = Console.ReadLine();
            if (!double.TryParse(s1, out double n1))
            {
                Console.WriteLine("Error: Format de numero incorrecte.");
                return;
            }

            if (op == "sqrt")
            {
                if (n1 < 0)
                {
                    Console.WriteLine("Error: No es pot calcular la arrel quadrada d'un nombre negatiu.");
                    return;
                }

                Console.WriteLine($"Resultat: {Math.Sqrt(n1)}");
                return;
            }

            Console.Write("Segon numero: ");
            var s2 = Console.ReadLine();
            if (!double.TryParse(s2, out double n2))
            {
                Console.WriteLine("Error: Format de numero incorrecte.");
                return;
            }

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
}