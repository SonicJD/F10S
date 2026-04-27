using System;
using Sys = Cosmos.System;
using CosmosKernel4.Core;
using CosmosKernel4.Utils;
using Cosmos.System.Graphics;

namespace CosmosKernel4
{
    public class Kernel : Sys.Kernel
    {
        public static Kernel Instance;

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
        private CommandProcessor processor;

        public void Print(string text)
        {
            Console.WriteLine(text);
        }

        protected override void BeforeRun()
        {
            Console.Clear();
            DisplayLogo();
            
            Instance = this;
            
            SoundUtils.PlayStartup();

            Console.WriteLine("F1OS ha arrancat correctament. Benvingut al Pit Lane.");
            Console.WriteLine("Escriu 'briefing' per veure les comandes disponibles.");

            Sys.KeyboardManager.SetKeyLayout(new Sys.ScanMaps.ESStandardLayout());

            Console.WriteLine("Debug: Registrant VFS...");
            try
            {
                Sys.FileSystem.VFS.VFSManager.RegisterVFS(
                    new Cosmos.System.FileSystem.CosmosVFS()
                );
                Console.WriteLine("Debug: VFS registrat correctament.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Debug: Error registrant VFS: {ex.Message}");
            }

            processor = new CommandProcessor();
        }

        protected override void Run()
        {
            Console.Write("\nComanda (briefing per veure tot!!!): ");
            var input = (Console.ReadLine() ?? "").ToLower().Trim();

            processor.Execute(input);
        }
    }
}
