namespace CosmosKernel4.Utils
{
    public static class PathUtils
    {
        public static string Normalize(string current, string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return current;

            if (input.StartsWith("/")) return input;

            if (current == "/") return "/" + input;

            return current.TrimEnd('/') + "/" + input;
        }

        public static string ToVfs(string path)
        {
            if (path == "/") return "0:\\";

            return "0:\\" + path.TrimStart('/').Replace('/', '\\');
        }
    }
}