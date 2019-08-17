using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace JSparkerEngine
{
    public class npm_loader
    {
        /// <summary>
        /// the ability to add npm modules to your project (windows only)
        /// </summary>
        /// <param name="name">the npm names as a string List</param>
        /// <param name="global">boolean for setting npm global</param>
        /// <returns>the success or failure of the npm install process</returns>
        public bool downloadNPM(List<string> name, bool global = false)
        {
            try
            {
                var dir = Environment.CurrentDirectory;
                var g = "";
                if (global) g = "-g";
                var args = new List<string>();
                args.Add("@echo off");
                args.Add("cd \"" + dir + "\"");
                args.Add("npm i " + g + " " + name);


                if (File.Exists(dir + "\\run_npm.bat")) File.WriteAllLines(dir + "\\run_npm.bat", args);

                var proc1 = Process.Start(dir + "\\run_npm.bat");
                proc1.WaitForExit();

                return true;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return false;
            }
        }
    }
}