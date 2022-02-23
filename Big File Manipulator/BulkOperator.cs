using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Big_File_Manipulator
{
    class BulkOperator
    {
        string selectedDir = LocationManager.selectedDir;
        string location = LocationManager.location;

        public void MassFileOp(string loc, bool perserve, Action<string, string> action)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(loc);
                string[] files = Directory.GetFiles(loc);

                foreach (string dir in dirs)
                {
                    MassFileOp(dir, perserve, action);
                    ManipulateFiles(files, perserve, action);
                }
            }
            catch { }
        }

        private void ManipulateFiles(string[] files, bool perserve, Action<string, string> action)
        {
            new Thread(() =>
            {
                foreach (string file in files)
                {
                    string to = $"{selectedDir}{file.Split('\\')[^1]}";
                    if (perserve)
                    {
                        to = $"{selectedDir}{file.Replace(location, "\\")}";

                        _ = Directory.CreateDirectory(string.Join('\\', to.Split('\\')[..^1]));
                    }
                    else if (File.Exists(to))
                    {
                        _ = Directory.CreateDirectory(string.Join('\\', $"{selectedDir}{file.Replace(location, "\\")}".Split('\\')[..^1]));
                    }

                    action(file, to);
                }

                Random rand = new();

                string path = $"{Environment.CurrentDirectory}\\Logging\\Progress.txt";
                bool updated = false;

                while (!updated)
                {
                    try
                    {
                        File.WriteAllText(path, (Convert.ToInt32(File.ReadAllLines(path)[0]) + files.Length).ToString());
                        updated = true;
                        Thread.Sleep(rand.Next(0, 200));
                    } catch { }
                }

                rand = null;
            });
        }
    }
}
