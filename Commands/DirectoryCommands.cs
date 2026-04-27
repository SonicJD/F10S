using System;
using System.IO;
using CosmosKernel4.Utils;
using Cosmos.System.FileSystem.VFS;

namespace CosmosKernel4.Commands
{
    public static class DirectoryCommands
    {
        // =========================
        // LISTAR DIRECTORIO (grid)
        // =========================
        public static void List(string currentDir)
        {
            var vfsPath = PathUtils.ToVfs(currentDir);
            var dirPath = vfsPath.EndsWith("\\") ? vfsPath : vfsPath + "\\";

            Console.WriteLine($"Llistant: {dirPath}");

            try
            {
                var directories = Directory.GetDirectories(dirPath);
                var files = Directory.GetFiles(dirPath);

                // Mostrar directorios
                foreach (var directory in directories)
                {
                    var rel = directory.StartsWith(dirPath)
                        ? directory.Substring(dirPath.Length)
                        : directory;

                    rel = rel.TrimEnd('\\');

                    var display = currentDir == "/"
                        ? "/" + rel.Replace('\\', '/')
                        : currentDir.TrimEnd('/') + "/" + rel.Replace('\\', '/');

                    Console.WriteLine(display);
                }

                // Mostrar archivos
                foreach (var file in files)
                {
                    var rel = file.StartsWith(dirPath)
                        ? file.Substring(dirPath.Length)
                        : file;

                    rel = rel.TrimEnd('\\');

                    var display = currentDir == "/"
                        ? "/" + rel.Replace('\\', '/')
                        : currentDir.TrimEnd('/') + "/" + rel.Replace('\\', '/');

                    Console.WriteLine(display);
                }

                SoundUtils.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error llegint directori: ({ex.GetType().Name}) {ex.Message}");
                SoundUtils.Error();
            }
        }

        // =========================
        // CANVIAR DIRECTORIO (drs)
        // =========================
        public static string Change(string currentDir)
        {
            Console.Write("Introdueix el directori a canviar: ");
            var input = Console.ReadLine() ?? string.Empty;

            var newDir = PathUtils.Normalize(currentDir, input);

            try
            {
                var files = VFSManager.GetDirectoryListing(PathUtils.ToVfs(newDir));

                if (files == null)
                {
                    Console.WriteLine("Error: Directori no trobat o buit.");
                    SoundUtils.Error();
                    return currentDir;
                }

                Console.WriteLine($"Directori canviat a: {newDir}");
                SoundUtils.Success();
                return newDir;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: Directori no trobat. {ex.Message}");
                SoundUtils.Error();
                return currentDir;
            }
        }

        // =========================
        // CREAR DIRECTORIO (build)
        // =========================
        public static void Create(string currentDir)
        {
            Console.Write("Introdueix el nom del directori a crear: ");
            var name = Console.ReadLine() ?? string.Empty;

            var path = PathUtils.Normalize(currentDir, name);

            try
            {
                VFSManager.CreateDirectory(PathUtils.ToVfs(path));

                Console.WriteLine($"Directori '{path}' creat correctament.");
                SoundUtils.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: No s'ha pogut crear el directori. {ex.Message}");
                SoundUtils.Error();
            }
        }

        // =========================
        // ELIMINAR DIRECTORIO (crash)
        // =========================
        public static void Delete(string currentDir)
        {
            Console.Write("Introdueix el nom del directori a eliminar: ");
            var name = Console.ReadLine() ?? string.Empty;

            var path = PathUtils.Normalize(currentDir, name);

            try
            {
                VFSManager.DeleteDirectory(PathUtils.ToVfs(path), true);

                Console.WriteLine($"Directori '{path}' eliminat correctament.");
                SoundUtils.Success();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: No s'ha pogut eliminar el directori. ({ex.GetType().Name}) {ex.Message}");
                SoundUtils.Error();
            }
        }
    }
}