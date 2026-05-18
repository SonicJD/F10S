using Cosmos.HAL;
using Cosmos.System;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Cosmos.System.Network;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using CosmosFtpServer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Sys = Cosmos.System;
using SysFileSystem = Cosmos.System.FileSystem.CosmosVFS;

namespace CosmosKernel12
{
    public class Kernel : Sys.Kernel
    {
        private Canvas canvas;
        private List<string> messages = new List<string>();
        private List<string> commandHistory = new List<string>();
        private string input = "";
        private string currentDir = @"0:\";
        private SysFileSystem fs;



        // =========================
        // BOOT SCREEN
        // =========================
        private bool bootScreen = true;

        // =========================
        // NETWORK
        // =========================
        private string systemIP = "0.0.0.0";

        // =========================
        // FTP
        // =========================
        private bool ftpEnabled = false;
        private bool ftpListening = false;   // true = estem dins del Listen()
        private string ftpSharedFolder = @"0:\ftp";
        private FtpServer ftpServer = null;

        // =========================
        // START
        // =========================
        protected override void BeforeRun()
        {
            fs = new SysFileSystem();
            VFSManager.RegisterVFS(fs);

            canvas = FullScreenCanvas.GetFullScreenCanvas(
                new Mode(1024, 768, ColorDepth.ColorDepth32)
            );

            MouseManager.ScreenWidth = 0;
            MouseManager.ScreenHeight = 0;

            NetworkInit();
        }

        protected override void Run()
        {
            // =========================
            // BOOT SCREEN
            // =========================
            if (bootScreen)
            {
                DrawBootScreen();
                if (KeyboardManager.TryReadKey(out KeyEvent key))
                {
                    if (key.Key == ConsoleKeyEx.Enter)
                    {
                        bootScreen = false;
                        AddMessage("F1OS iniciado.");
                        AddMessage("");
                        ShowCommands();
                        SoundUtils.PlayStartup();
                    }
                }
                return;
            }

            // =========================
            // MODE FTP ACTIU
            // Si el FTP esta en mode listen, mostrem pantalla d'espera
            // i bloquejem aqui fins que el client es desconnecti
            // =========================
            if (ftpListening && ftpServer != null)
            {
                DrawFTPScreen();

                // Listen() es bloquejant: espera connexio, la processa, i retorna
                try
                {
                    ftpServer.Listen();
                    AddMessage("[FTP] Client desconnectat.");
                }
                catch (Exception ex)
                {
                    AddMessage("[FTP] Error: " + ex.Message);
                }

                // Quan Listen() retorna, el servidor ha acabat
                ftpListening = false;
                ftpEnabled = false;
                ftpServer.Dispose();
                ftpServer = null;

                AddMessage("[FTP] Servidor aturat.");
                AddMessage("Pots tornar a fer ftpstart.");
                return;
            }

            // =========================
            // NORMAL SYSTEM
            // =========================
            HandleKeyboard();
            DrawUI();
        }

        // =========================
        // PANTALLA FTP
        // =========================
        private void DrawFTPScreen()
        {
            canvas.Clear(Color.Black);

            canvas.DrawFilledRectangle(Color.DarkRed, 0, 0, 1024, 60);
            canvas.DrawString(
                "F1OS - SERVIDOR FTP ACTIU",
                PCScreenFont.Default, Color.Yellow, 20, 20
            );

            int y = 80;
            foreach (string msg in messages)
            {
                canvas.DrawString(msg, PCScreenFont.Default, Color.White, 20, y);
                y += 22;
            }

            canvas.DrawFilledRectangle(Color.DarkGray, 0, 720, 1024, 48);
            canvas.DrawString(
                "Esperant connexio Filezilla... IP: " + systemIP + ":21",
                PCScreenFont.Default, Color.Black, 20, 736
            );

            canvas.Display();
        }

        // =========================
        // BOOT SCREEN
        // =========================
        private void DrawBootScreen()
        {
            canvas.Clear(Color.Black);

            string[] logo =
            {
                "                                                               ",
                "                                                               ",
                "                           F1OS                                ",
                "                  FORMULA 1 OPERATING SYSTEM                   ",
                "                                                               ",
                "                   #####  ###   ###    ####                    ",
                "                   #     # #   #   #  #                        ",
                "                   #####   #   #   #   ###                     ",
                "                   #       #   #   #      #                    ",
                "                   #     #####  ###   ####                     ",
                "                                                               ",
                "          +-----------------------------------------+          ",
                "          |         BIENVENIDO AL PIT LANE          |          ",
                "          |   Escribe 'briefing' para ver comandos  |          ",
                "          +-----------------------------------------+          ",
                "                                                               ",
                "                                                               "
            };

            int charWidth = 8;
            int lineHeight = 22;
            int maxLen = 0;
            foreach (var line in logo) if (line.Length > maxLen) maxLen = line.Length;
            int blockWidth = maxLen * charWidth;
            int blockHeight = logo.Length * lineHeight;
            int startX = (1024 - blockWidth) / 2;
            int startY = (768 - blockHeight) / 2 - 30;
            int y = startY;

            foreach (string line in logo)
            {
                canvas.DrawString(line, PCScreenFont.Default, Color.Red, startX, y);
                y += lineHeight;
            }

            canvas.DrawString(
                "PULSA ENTER PARA CONTINUAR",
                PCScreenFont.Default,
                Color.Yellow,
                (1024 - ("PULSA ENTER PARA CONTINUAR".Length * charWidth)) / 2,
                startY + blockHeight + 40
            );

            canvas.Display();
        }

        // =========================
        // NETWORK
        // =========================
        private void NetworkInit()
        {
            try
            {
                NetworkDevice nic = null;
                foreach (var device in NetworkDevice.Devices)
                {
                    nic = device;
                    break;
                }

                if (nic == null)
                {
                    AddMessage("ERROR: No NIC trobat");
                    return;
                }

                var ip = new Address(192, 168, 93, 2);
                var mask = new Address(255, 255, 255, 0);
                var gw = new Address(192, 168, 93, 1);

                IPConfig.Enable(nic, ip, mask, gw);
                systemIP = "192.168.93.2";

                AddMessage("Red iniciada.");
                AddMessage("NIC: " + nic.Name);
                AddMessage("IP:  " + systemIP);
            }
            catch (Exception ex)
            {
                AddMessage("Error de red: " + ex.Message);
            }
        }

        // =========================
        // UI
        // =========================
        private void DrawUI()
        {
            canvas.Clear(Color.Black);
            DrawTerminal();
            DrawInput();
            canvas.Display();
        }

        private void DrawCategory(string text, int y)
            => canvas.DrawString(text, PCScreenFont.Default, Color.Yellow, 12, y);

        private void DrawCommand(string text, int y)
            => canvas.DrawString("- " + text, PCScreenFont.Default, Color.White, 28, y);

        private void DrawTerminal()
        {
            canvas.DrawFilledRectangle(Color.Black, 0, 0, 1024, 768);

            int y = 20;
            foreach (string msg in messages)
            {
                canvas.DrawString(msg, PCScreenFont.Default, Color.White, 20, y);
                y += 22;
            }
        }

        private void DrawInput()
        {
            canvas.DrawFilledRectangle(Color.DarkGray, 0, 720, 1024, 48);
            canvas.DrawString("> " + input, PCScreenFont.Default, Color.Black, 20, 736);
        }

        // =========================
        // KEYBOARD
        // =========================
        private void HandleKeyboard()
        {
            if (KeyboardManager.TryReadKey(out KeyEvent key))
            {
                if (key.Key == ConsoleKeyEx.Enter)
                {
                    ExecuteCommand(input);
                    input = "";
                    return;
                }
                if (key.Key == ConsoleKeyEx.Backspace)
                {
                    if (input.Length > 0)
                        input = input.Substring(0, input.Length - 1);
                    return;
                }
                char c = key.KeyChar;
                if (c != '\0') input += c;
            }
        }

        // =========================
        // COMMANDS
        // =========================
        private void ExecuteCommand(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd)) return;

            AddMessage("> " + cmd);

            // guardar historial (máximo 5)
            if (cmd.ToLower() != "history")
            {
                commandHistory.Add(cmd);

                if (commandHistory.Count > 5)
                    commandHistory.RemoveAt(0);
            }

            string[] parts = cmd.Trim().Split(' ');
            string command = parts[0].ToLower();
            string[] args = parts.Length > 1 ? parts.Skip(1).ToArray() : new string[0];

            switch (command)
            {
                case "team":
                    AddMessage("F1OS - Version 1.0");
                    SoundUtils.Success();
                    break;

                case "telemetry":
                    AddMessage("RAM OK - System Stable");
                    AddMessage("FTP: " + (ftpEnabled ? "ACTIVO" : "OFFLINE"));
                    SoundUtils.Success();
                    break;

                case "lap":
                    AddMessage(DateTime.Now.ToString());
                    SoundUtils.Success();
                    break;

                case "ip":
                    AddMessage("IP: " + systemIP);
                    SoundUtils.Success();
                    break;

                // =========================
                // FTP START
                // Prepara el servidor i activa el mode FTP.
                // El Run() detectara ftpListening=true i cridara Listen().
                // =========================
                case "ftpstart":
                    try
                    {
                        if (ftpEnabled)
                        {
                            AddMessage("FTP ja actiu.");
                            SoundUtils.Error();
                            break;
                        }

                        if (!Directory.Exists(ftpSharedFolder))
                            Directory.CreateDirectory(ftpSharedFolder);

                        // Crea el servidor FTP oficial de Cosmos
                        ftpServer = new FtpServer(fs, ftpSharedFolder);

                        ftpEnabled = true;
                        ftpListening = true;   // Run() entrara al mode FTP

                        AddMessage("FTP STARTED");
                        AddMessage("IP:   " + systemIP);
                        AddMessage("PORT: 21");
                        AddMessage("DIR:  " + ftpSharedFolder);
                        AddMessage("Filezilla: mode ACTIU obligatori!");
                        AddMessage("Connecta ara des de Filezilla...");
                        SoundUtils.Success();
                    }
                    catch (Exception ex)
                    {
                        AddMessage("FTP ERROR: " + ex.Message);
                        SoundUtils.Error();
                    }
                    break;

                // =========================
                // FTP STOP
                // =========================
                case "ftpstop":
                    ftpEnabled = false;
                    ftpListening = false;
                    if (ftpServer != null)
                    {
                        try { ftpServer.Dispose(); } catch { }
                        ftpServer = null;
                    }
                    AddMessage("FTP detenido.");
                    SoundUtils.Success();
                    break;

                // =========================
                // FTP STATUS
                // =========================
                case "ftpstatus":
                    AddMessage("FTP: " + (ftpEnabled ? "ACTIVO" : "OFFLINE"));
                    if (ftpEnabled)
                    {
                        AddMessage("IP:  " + systemIP);
                        AddMessage("DIR: " + ftpSharedFolder);
                    }
                    SoundUtils.Success();
                    break;

                case "grid":
                    try
                    {
                        string dirToList = args.Length > 0 ? args[0] : currentDir;
                        AddMessage("DIR: " + dirToList);
                        foreach (string d in Directory.GetDirectories(dirToList))
                            AddMessage("[D] " + Path.GetFileName(d));
                        foreach (string f in Directory.GetFiles(dirToList))
                            AddMessage("[F] " + Path.GetFileName(f));
                        SoundUtils.Success();
                    }
                    catch (Exception ex) { AddMessage("Error: " + ex.Message); SoundUtils.Error(); }
                    break;

                case "drs":
                    try
                    {
                        if (args.Length == 0)
                        {
                            AddMessage("Dir actual: " + currentDir);
                            SoundUtils.Success();
                            break;
                        }

                        string newDir = args[0];

                        // soporta ".."
                        if (newDir == "..")
                        {
                            if (currentDir != @"0:\")
                            {
                                string temp = currentDir.TrimEnd('\\');
                                int last = temp.LastIndexOf('\\');

                                if (last > 1)
                                    currentDir = temp.Substring(0, last + 1);
                                else
                                    currentDir = @"0:\";
                            }

                            AddMessage("Dir actual: " + currentDir);
                            SoundUtils.Success();
                            break;
                        }

                        // ruta relativa
                        if (!newDir.Contains(@":\"))
                        {
                            if (!currentDir.EndsWith("\\"))
                                currentDir += "\\";

                            newDir = currentDir + newDir;
                        }

                        if (!newDir.EndsWith("\\"))
                            newDir += "\\";

                        if (Directory.Exists(newDir))
                        {
                            currentDir = newDir;
                            AddMessage("Dir actual: " + currentDir);
                            SoundUtils.Success();
                        }
                        else
                        {
                            AddMessage("No existe: " + newDir);
                            SoundUtils.Error();
                        }
                    }
                    catch (Exception ex)
                    {
                        AddMessage("Error drs: " + ex.Message);
                        SoundUtils.Error();
                    }
                    break;

                case "build":
                    try
                    {
                        string folder = Path.Combine(currentDir, args.Length > 0 ? args[0] : "newfolder");
                        Directory.CreateDirectory(folder);
                        AddMessage("Creado: " + folder);
                        SoundUtils.Success();
                    }
                    catch (Exception ex) { AddMessage("Error: " + ex.Message); SoundUtils.Error(); }
                    break;

                case "crash":
                    try
                    {
                        string folder = Path.Combine(currentDir, args.Length > 0 ? args[0] : "newfolder");
                        if (Directory.Exists(folder))
                        {
                            Directory.Delete(folder, true);
                            AddMessage("Eliminado: " + folder);
                            SoundUtils.Success();
                        }
                        else { AddMessage("No existe: " + folder); SoundUtils.Error(); }
                    }
                    catch (Exception ex) { AddMessage("Error: " + ex.Message); SoundUtils.Error(); }
                    break;

                case "radio":
                    try
                    {
                        string file = Path.Combine(currentDir, args.Length > 0 ? args[0] : "test.txt");
                        if (File.Exists(file)) { AddMessage(File.ReadAllText(file)); SoundUtils.Success(); }
                        else { AddMessage("No encontrado: " + file); SoundUtils.Error(); }
                    }
                    catch (Exception ex) { AddMessage("Error: " + ex.Message); SoundUtils.Error(); }
                    break;

                case "engine":
                    try
                    {
                        string file = Path.Combine(currentDir, args.Length > 0 ? args[0] : "test.txt");
                        string content = args.Length > 1 ? string.Join(' ', args.Skip(1)) : "F1OS";
                        File.WriteAllText(file, content);
                        AddMessage("Escrito: " + file);
                        SoundUtils.Success();
                    }
                    catch (Exception ex) { AddMessage("Error: " + ex.Message); SoundUtils.Error(); }
                    break;

                case "rm":
                    try
                    {
                        string file = Path.Combine(currentDir, args.Length > 0 ? args[0] : "test.txt");
                        if (File.Exists(file)) { File.Delete(file); AddMessage("Eliminado: " + file); SoundUtils.Success(); }
                        else { AddMessage("No existe: " + file); SoundUtils.Error(); }
                    }
                    catch (Exception ex) { AddMessage("Error: " + ex.Message); SoundUtils.Error(); }
                    break;

                case "calc":
                    try
                    {
                        if (args.Length < 3) { AddMessage("Uso: calc <sum|sub|mult|div> n1 n2"); SoundUtils.Error(); break; }
                        double n1 = double.Parse(args[1]);
                        double n2 = double.Parse(args[2]);
                        double r = args[0] == "sum" ? n1 + n2
                                 : args[0] == "sub" ? n1 - n2
                                 : args[0] == "mult" ? n1 * n2
                                 : n1 / n2;
                        AddMessage("= " + r);
                        SoundUtils.Success();
                    }
                    catch { AddMessage("Error en calculo."); SoundUtils.Error(); }
                    break;

                case "pitstop":
                    messages.Clear();
                    SoundUtils.Success();
                    break;

                case "briefing":
                    ShowCommands();
                    SoundUtils.Success();
                    break;

                case "restart":
                    AddMessage("Restarting...");
                    SoundUtils.Success();
                    Cosmos.System.Power.Reboot();
                    break;

                case "retire":
                    AddMessage("Shutting down...");
                    SoundUtils.Success();
                    Cosmos.System.Power.Shutdown();
                    break;

                case "history":
                    if (commandHistory.Count == 0)
                    {
                        AddMessage("No hi ha historial.");
                    }
                    else
                    {
                        AddMessage("Ultimes 5 comandes:");

                        for (int i = 0; i < commandHistory.Count; i++)
                        {
                            AddMessage(i + ": " + commandHistory[i]);
                        }
                    }

                    SoundUtils.Success();
                    break;

                case "repeat":
                    try
                    {
                        if (args.Length == 0)
                        {
                            AddMessage("Uso: repeat <numero>");
                            SoundUtils.Error();
                            break;
                        }

                        int index = int.Parse(args[0]);

                        if (index >= 0 && index < commandHistory.Count)
                        {
                            string oldCmd = commandHistory[index];
                            AddMessage("Executant: " + oldCmd);
                            ExecuteCommand(oldCmd);
                        }
                        else
                        {
                            AddMessage("Index invalid.");
                            SoundUtils.Error();
                        }
                    }
                    catch
                    {
                        AddMessage("Error repeat.");
                        SoundUtils.Error();
                    }
                    break;
                default:
                    AddMessage("Unknown: " + command);
                    SoundUtils.Error();
                    break;
            }
        }

        private void ShowCommands()
        {
            AddMessage("SYSTEM:  team / telemetry / lap / ip");
            AddMessage("FILES:   grid / drs / build / crash");
            AddMessage("         radio / engine / rm");
            AddMessage("NETWORK: ftpstart / ftpstop / ftpstatus");
            AddMessage("UTILS:   calc / pitstop / briefing");
            AddMessage("POWER:   restart / retire");
            AddMessage("MEM:     history / repeat");
        }

        private void AddMessage(string msg)
        {
            messages.Add(msg);
            if (messages.Count > 28) messages.RemoveAt(0);
        }
    }

    // =========================
    // SOUND
    // =========================
    public static class SoundUtils
    {
        public static void PlayStartup()
        {
            System.Console.Beep(600, 100);
            System.Console.Beep(700, 100);
            System.Console.Beep(800, 150);
        }
        public static void Success() => System.Console.Beep(800, 150);
        public static void Error() => System.Console.Beep(400, 200);
    }
}