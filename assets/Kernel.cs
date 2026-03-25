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
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
        }

        protected override void Run()
        {
            Console.Write("Comanda (briefing per veure tot): ");
            var input = Console.ReadLine();
            Console.WriteLine(input);
            if (input== "briefing")
            {
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
        }
    }
}
