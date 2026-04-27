using System;

namespace CosmosKernel4.Utils
{
    public static class SoundUtils
    {
        public static void PlayStartup()
        {
            Console.Beep(600, 100);
            Console.Beep(700, 100);
            Console.Beep(800, 150);
        }

        public static void Success()
        {
            Console.Beep(800, 150);
        }

        public static void Error()
        {
            Console.Beep(400, 200);
        }
    }
}