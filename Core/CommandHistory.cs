using System;

namespace CosmosKernel4.Core
{
    public class CommandHistory
    {
        private string[] history = new string[5];
        private int index = 0;
        private int count = 0;

        public void Save(string cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd)) return;

            history[index] = cmd;
            index = (index + 1) % 5;

            if (count < 5) count++;
        }

        public void Show()
        {
            Console.WriteLine("--- HISTORIAL ---");

            for (int i = 0; i < count; i++)
            {
                int idx = (index - count + i + 5) % 5;
                Console.WriteLine($"{i}: {history[idx]}");
            }
        }

        public string Get()
        {
            Console.Write("Index: ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int i) || i < 0 || i >= count)
            {
                Console.WriteLine("Index invalid.");
                return null;
            }

            int idx = (index - count + i + 5) % 5;
            return history[idx];
        }
    }
}