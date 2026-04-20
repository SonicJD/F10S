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

        private void ShowFile()
        {
            Console.Write("Introdueix el nom del fitxer a mostrar: ");
            var name = Console.ReadLine() ?? string.Empty;
            var path = ToVfsPath(NormalizePath(name));
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("Error: fitxer no existent.");
                    PlayErrorSound();
                    return;
                }

                var text = File.ReadAllText(path);
                Console.WriteLine(text);
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error llegint fitxer: ({ex.GetType().Name}) {ex.Message}");
                PlayErrorSound();
            }
        }

        private void EditFile()
        {
            Console.Write("Introdueix el nom del fitxer a editar/crear: ");
            var name = Console.ReadLine() ?? string.Empty;
            var path = ToVfsPath(NormalizePath(name));
            Console.WriteLine("Escriu el contingut (finalitza amb una línia que contingui només '.end'):");
            var sb = new System.Text.StringBuilder();
            while (true)
            {
                var line = Console.ReadLine();
                if (line == ".end") break;
                sb.AppendLine(line);
            }
            try
            {
                File.WriteAllText(path, sb.ToString());
                Console.WriteLine("Fitxer guardat.");
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error escrivint fitxer: ({ex.GetType().Name}) {ex.Message}");
                PlayErrorSound();
            }
        }

        private void DeleteFile()
        {
            Console.Write("Introdueix el nom del fitxer a eliminar: ");
            var name = Console.ReadLine() ?? string.Empty;
            var path = ToVfsPath(NormalizePath(name));
            try
            {
                if (!File.Exists(path))
                {
                    Console.WriteLine("Error: fitxer no existent.");
                    PlayErrorSound();
                    return;
                }

                File.Delete(path);
                Console.WriteLine("Fitxer eliminat.");
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error eliminant fitxer: ({ex.GetType().Name}) {ex.Message}");
                PlayErrorSound();
            }
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

        private void PlayStartupSound()
        {
            Console.Beep(600, 100);
            System.Threading.Thread.Sleep(50);
            Console.Beep(700, 100);
            System.Threading.Thread.Sleep(50);
            Console.Beep(800, 150);
        }

        private void PlaySuccessSound()
        {
            Console.Beep(800, 150);
            System.Threading.Thread.Sleep(50);
            Console.Beep(1000, 150);
        }

        private void PlayErrorSound()
        {
            Console.Beep(400, 200);
            System.Threading.Thread.Sleep(50);
            Console.Beep(300, 200);
        }

        private void DisplayLogo()
        {
            Console.WriteLine(@"
+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
+                                                             +
+                          F1OS                               +
+                 FORMULA 1 OPERATING SYSTEM                  +
+                                                             +
+                  #####  ###   ###    ####                   +      
+                  #     # #   #   #  #                       +            
+                  #####   #   #   #   ###                    +        
+                  #       #   #   #      #                   +           
+                  #     #####  ###   ####                    +
+                                                             +
+         +-----------------------------------------+         +
+         |         BIENVENIDO AL PIT LANE          |         +
+         |   Escribe 'briefing' para ver comandos  |         +
+         +-----------------------------------------+         +
+                                                             +
+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
");
        }

        protected override void BeforeRun()
        {
            Console.Clear();
            DisplayLogo();
            PlayStartupSound();
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

                case "radio":
                    ShowFile();
                    break;

                case "engine":
                    EditFile();
                    break;

                case "rm":
                    DeleteFile();
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

        private void grid()
        {
            var vfsPath = ToVfsPath(currentDirectory);
            var dirPath = vfsPath.EndsWith("\\") ? vfsPath : vfsPath + "\\";
            Console.WriteLine($"Llistant: {dirPath}");
            try
            {
                var directory_list = Directory.GetDirectories(dirPath);
                var files_list = Directory.GetFiles(dirPath);

                foreach (var directory in directory_list)
                {
                    var rel = directory.StartsWith(dirPath) ? directory.Substring(dirPath.Length) : directory;
                    rel = rel.TrimEnd('\\');
                    var display = currentDirectory == "/" ? "/" + rel.Replace('\\','/') : currentDirectory.TrimEnd('/') + "/" + rel.Replace('\\','/');
                    Console.WriteLine(display);
                }

                foreach (var file in files_list)
                {
                    var rel = file.StartsWith(dirPath) ? file.Substring(dirPath.Length) : file;
                    rel = rel.TrimEnd('\\');
                    var display = currentDirectory == "/" ? "/" + rel.Replace('\\','/') : currentDirectory.TrimEnd('/') + "/" + rel.Replace('\\','/');
                    Console.WriteLine(display);
                }
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error llegint directori: ({ex.GetType().Name}) {ex.Message}");
                PlayErrorSound();
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
                    PlayErrorSound();
                    return;
                }

                currentDirectory = newDir;
                Console.WriteLine($"Directori canviat a: {currentDirectory}");
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Directori no trobat. {ex.Message}");
                PlayErrorSound();
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
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: No s'ha pogut crear el directori. {ex.Message}");
                PlayErrorSound();
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
                PlaySuccessSound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: No s'ha pogut eliminar el directori. ({ex.GetType().Name}) {ex.Message}");
                PlayErrorSound();
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
                PlayErrorSound();
                return;
            }

            if (op == "sqrt")
            {
                if (n1 < 0)
                {
                    Console.WriteLine("Error: No es pot calcular la arrel quadrada d'un nombre negatiu.");
                    PlayErrorSound();
                    return;
                }

                Console.WriteLine($"Resultat: {Math.Sqrt(n1)}");
                PlaySuccessSound();
                return;
            }

            Console.Write("Segon numero: ");
            var s2 = Console.ReadLine();
            if (!double.TryParse(s2, out double n2))
            {
                Console.WriteLine("Error: Format de numero incorrecte.");
                PlayErrorSound();
                return;
            }

            switch (op)
            {
                case "suma": Console.WriteLine($"Resultat: {n1 + n2}"); PlaySuccessSound(); break;
                case "resta": Console.WriteLine($"Resultat: {n1 - n2}"); PlaySuccessSound(); break;
                case "mult": Console.WriteLine($"Resultat: {n1 * n2}"); PlaySuccessSound(); break;
                case "div":
                    if (n2 != 0) { Console.WriteLine($"Resultat: {n1 / n2}"); PlaySuccessSound(); }
                    else { Console.WriteLine("Error: Divisio per zero."); PlayErrorSound(); }
                    break;
                case "mod": Console.WriteLine($"Resultat: {n1 % n2}"); PlaySuccessSound(); break;
                default: Console.WriteLine("Operacio no valida."); PlayErrorSound(); break;
            }
        }
    }
}
