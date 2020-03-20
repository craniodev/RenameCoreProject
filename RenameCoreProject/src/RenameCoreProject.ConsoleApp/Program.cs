using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RenameCoreProject.ConsoleApp
{
    class Program
    {
        private static List<FolderDto> Folders = new List<FolderDto>();
        private static List<FileDto> Files = new List<FileDto>();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var origin = @"C:\Users\rafae\source\repos\eattae_account\Original";
            var dest = @"C:\Users\rafae\source\repos\eattae_account";


            dest = Path.Combine(dest, "Renamed");

            Console.WriteLine("Limpando destio");
            if (Directory.Exists(dest))
            {
                Directory.Delete(dest, true);
            }

            //
            var oldName = "eattae.Admin";
            var newName = "eattae.Account";

            Console.WriteLine("Copiando arquivos"); // TODO: COlocar em uma classe
            DirectoryCopy(origin, dest, true);
            Console.WriteLine("Copiado");


            Console.WriteLine("Renomeando pastas");
            RenameFiles(dest, oldName, newName);
            RenameFolders(dest, oldName, newName);
            Console.WriteLine("Renomeado");

            Files = new List<FileDto>();
            FillFilesCopy(dest, oldName, true);

            Console.WriteLine("Replace internal file");
            ReplateFilesContent(oldName, newName);
            Console.WriteLine("Replaced");


        }

        private static void RenameFolders(string dest, string oldName, string newName)
        {

            foreach (var folder in Folders.OrderByDescending(f => f.Id))
            {

                var toName = GetNewName(folder.Path, oldName, newName);
                if (toName == folder.Path)
                {
                    continue;
                }

                Console.WriteLine($"    {folder.Id} {folder.Path}");
                Directory.Move(folder.Path, toName);
                folder.Path = toName;

            }

        }

        private static void RenameFiles(string dest, string oldName, string newName)
        {

            foreach (var file in Files.OrderByDescending(f => f.Id))
            {

                var toName = GetNewName(file.Path, oldName, newName);
                if (toName == file.Path)
                {
                    continue;
                }

                Console.WriteLine($"    {file.Id} {file.Path}");
                File.Move(file.Path, toName);
                file.Path = toName;

            }

        }

        private static void ReplateFilesContent(string oldName, string newName)
        {

            foreach (var file in Files.OrderByDescending(f => f.Id))
            {

                if (file.Path.IndexOf("\\bin\\") != -1)
                {
                    continue;
                }
                if (file.Path.IndexOf("\\wwwroot\\lib\\") != -1)
                {
                    continue;
                }




                Console.WriteLine($"    {file.Id} {file.Path}");
                string textOld = File.ReadAllText(file.Path);
                var textNew = textOld.Replace(oldName, newName);
                if (textNew == textOld)
                {
                    continue;
                }


                File.WriteAllText(file.Path, textNew);

            }

        }



        private static string GetNewName(string path, string oldName, string newName)
        {

            var partes = path.Split("\\");

            var n = partes[partes.Length - 1];

            partes[partes.Length - 1] = n.Replace(oldName, newName);

            var ret = "";

            foreach (var parte in partes)
            {
                ret = Path.Combine(ret, parte);
            }


            return ret;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            Folders.Add(new FolderDto
            {
                Id = Folders.Count,
                Path = destDirName
            });


            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
                Files.Add(new FileDto
                {
                    Id = Files.Count,
                    Path = temppath
                });
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }





        private static void FillFilesCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {



            Folders.Add(new FolderDto
            {
                Id = Folders.Count,
                Path = destDirName
            });


            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            //if (!Directory.Exists(destDirName))
            //{
            //    Directory.CreateDirectory(destDirName);
            //}

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                //string temppath = Path.Combine(destDirName, file.Name);
                //file.CopyTo(temppath, false);
                Files.Add(new FileDto
                {
                    Id = Files.Count,
                    Path = file.FullName
                });
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    FillFilesCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }













    }
}
