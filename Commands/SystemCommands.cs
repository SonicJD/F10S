namespace CosmosKernel4.Commands
{
    public static class SystemCommands
    {
        public static void Info()
        {
            Kernel.Instance.Print("F1OS System v1.0");
        }

        public static void Memory()
        {
            Kernel.Instance.Print("RAM: OK");
        }

        public static void Uptime()
        {
            Kernel.Instance.Print("Sistema activo");
        }
    }
}