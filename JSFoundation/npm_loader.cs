using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace JSparkerEngine
{
    public class npm_loader
    {
        public bool downloadNPM(List<string> name, bool global = false)
        {
            try
            {
                var dir = Environment.CurrentDirectory;
                var g = "";
                if(global)
                {
                    g = "-g";
                }
                List<string> args = new List<string>();
                args.Add("@echo off");
                args.Add("cd \"" + dir + "\"");
                args.Add("npm i " + g + " " + name);
                

                if(System.IO.File.Exists(dir + "\\run_npm.bat"))
                {
                    System.IO.File.WriteAllLines(dir + "\\run_npm.bat", args);
                }

                Process proc1 = Process.Start(dir + "\\run_npm.bat");
                proc1.WaitForExit();

                return true;
            }
            catch(Exception i)
            {
                Console.WriteLine(i);
                return false;
            }
        }
    }
}
