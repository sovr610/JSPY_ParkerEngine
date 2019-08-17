using System;
using System.Diagnostics;
using System.IO;
using Jint;

namespace JSparkerEngine
{
    internal class ProgramCore
    {
        private static void Main(string[] args)
        {
            var prm = new ProgramCore();
            Console.WriteLine(" _ -- Welcome To JSPY-Parker-Engine 1.0v -- _");
            Console.WriteLine("note: would have a nice gui, but .net core... you know. Might do it in electron.js");

            Console.WriteLine("Coomand Prompt Mode: /start to start engine");
            while (true)
            {
                Console.Write(":> ");
                string r = Console.ReadLine();

                if(r == "/start")
                {
                    break;
                }

                prm.ExecuteCommandSync(r);


            }


            Console.WriteLine("type /open to open a javascript/python file");
            Console.WriteLine("");
            Console.Write("Do you want python or javascript? or JSPY (javascript & python merged) (p, j or jspy): ");
            var type = Console.ReadLine().Trim();


            if (type.ToLower() == "jspy")
            {
                var core = new JScore();
                var main_engine = core.createEngine();
                var ee = prm.addModules(core.getJSengine(main_engine));
                core.setJSEngine(ee, main_engine);
                var python = new PythonClass();
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(":");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    var cmd = Console.ReadLine();

                    try
                    {
                        object ret_final = null;
                        try
                        {
                            var ret = core.executeJSprogramWithReturn(main_engine, cmd);
                            ret_final = ret;
                        }
                        catch (Exception ii)
                        {
                        }

                        var check = false;
                        try
                        {
                            check = python.ExecutePythonCommand(cmd);
                        }
                        catch (Exception ii)
                        {
                        }

                        Console.WriteLine("JS: " + ret_final + ", PY:" + Convert.ToString(check));
                    }
                    catch (Exception i)
                    {
                        Console.WriteLine(i);
                    }
                }
            }

            if (type.ToLower() == "j")
            {
                Console.WriteLine("");


                var core = new JScore();
                var main_engine = core.createEngine();

                Console.Write("Do you want to add modules to the javascript? and add C# functions? (Y/N): ");
                var check = Console.ReadLine().Trim().ToLower();
                if (check == "y")
                {
                    var ee = prm.addModules(core.getJSengine(main_engine));
                    core.setJSEngine(ee, main_engine);
                }

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(":> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    var cmd = Console.ReadLine();
                    if (cmd != "")
                    {
                        if (cmd.Trim() == "/open")
                        {
                            Console.WriteLine("Please enter or paste the directory:");
                            var dir = Console.ReadLine();
                            try
                            {
                                core.executeJSCode(main_engine, File.ReadAllText(dir));
                            }
                            catch (Exception i)
                            {
                                Console.WriteLine(i);
                            }
                        }
                        else
                        {
                            var ret = core.executeJSprogramWithReturn(main_engine, cmd);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(ret);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
            }

            {
                var python = new PythonClass();
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(":> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    var cmd = Console.ReadLine();
                    if (cmd != "")
                    {
                        if (cmd.Trim() == "/open")
                        {
                            Console.WriteLine("Please enter or paste the directory:");
                            var dir = Console.ReadLine();
                            try
                            {
                                python.ExecutePythonFile(dir, false);
                            }
                            catch (Exception i)
                            {
                                Console.WriteLine(i);
                            }
                        }
                        else
                        {
                            python.ExecutePythonCommand(cmd);
                        }
                    }
                }
            }
        }

        public Exception ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                var procStartInfo =
                    new ProcessStartInfo("cmd", "/c " + command)
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                // Do not create the black window.
                // Now we create a process, assign its ProcessStartInfo and start it
                var proc = new Process { StartInfo = procStartInfo };
                proc.Start();
                // Get the output into a string
                var result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
                return null;
            }
            catch (Exception objException)
            {
                return objException;
                // Log the exception
            }
        }

        public Engine addModules(Engine e)
        {
            Console.Clear();
            Console.Write("Add System.IO.File methods? (Y/N): ");
            if (Console.ReadLine().Trim().ToLower() == "y")
            {
                e.SetValue("writeFile", new Action<string, string>(File.WriteAllText));
                e.SetValue("deleteFile", new Action<string>(File.Delete));
                e.SetValue("fileExists", new Func<string, bool>(File.Exists));
                e.SetValue("readFile", new Func<string, string>(File.ReadAllText));
            }

            Console.WriteLine("added function: writeFile(string name, string text)");
            Console.WriteLine("added function: deleteFile(string name)");
            Console.WriteLine("added function: bool : fileExists(string name)");
            Console.WriteLine("added function: string : readFile(string name)");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();

            return e;
        }
    }
}