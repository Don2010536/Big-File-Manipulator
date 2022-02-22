using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Big_File_Manipulator
{
    class BulkOperator
    {
        public static void MassFileOp(string loc, bool perserve, Action<string, string> action)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(loc);
                string[] files = Directory.GetFiles(loc);

                foreach (string dir in dirs)
                {
                    MassFileOp(dir, perserve, action);
                    foreach (string file in files)
                    {
                        string to = $"{LocationManager.selectedDir}{file.Split('\\')[^1]}";
                        if (perserve)
                        {
                            to = $"{LocationManager.selectedDir}{file.Replace(LocationManager.location, "\\")}";

                            _ = Directory.CreateDirectory(string.Join('\\', to.Split('\\')[..^1]));
                        } else if (File.Exists(to))
                        {
                            _ = Directory.CreateDirectory(string.Join('\\', $"{LocationManager.selectedDir}{file.Replace(LocationManager.location, "\\")}".Split('\\')[..^1]));
                        }

                        action(file, to);
                    }
                }
            }
            catch { }
        }

    }
}
