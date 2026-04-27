using CosmosKernel4.Utils;

namespace CosmosKernel4.Commands
{
    public static class FileCommands
    {
        public static void Read(string dir)
        {
            Kernel.Instance.Print("radio ejecutado");
        }

        public static void Write(string dir)
        {
            Kernel.Instance.Print("engine ejecutado");
        }

        public static void Delete(string dir)
        {
            Kernel.Instance.Print("rm ejecutado");
        }
    }
}