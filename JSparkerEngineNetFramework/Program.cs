using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Jint;
using Newtonsoft.Json;

namespace JSparkerEngine
{
    internal class ProgramCore
    {
        private static void Main(string[] args)
        {
            string type = "";
            var prm = new ProgramCore();
            if (args != null)
            {
                Console.WriteLine(" _ -- Welcome To JSPY-Parker-Engine 1.0v -- _");
                Console.WriteLine("note: would have a nice gui, but .net core... you know. Might do it in electron.js");
                Console.WriteLine("make sure to add me to the PATH :), type in cd for the directory");
                Console.WriteLine("Comand Prompt Mode: /start to start engine or enter windows command line commands");
                while (true)
                {
                    Console.Write(":> ");
                    string r = Console.ReadLine();

                    if (r == "/start")
                    {
                        break;
                    }

                    prm.ExecuteCommandSync(r);


                }


                Console.WriteLine("type /open to open a javascript/python file");
                Console.WriteLine("");
                Console.Write("Do you want python or javascript? or JSPY (javascript & python merged) (p, j or jspy): ");
                type = Console.ReadLine().Trim();

 

                if (type.ToLower() == "jspy")
                {
                    var core = new JScore();
                    var main_engine = core.createEngine();


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
                                if (System.IO.File.Exists(Environment.CurrentDirectory + "\\temp.js"))
                                {
                                    File.Delete(Environment.CurrentDirectory + "\\temp.js");
                                }
                                File.WriteAllText(Environment.CurrentDirectory + "\\temp.js", cmd);
                                var ret = core.executeJSCode(main_engine, "", "./temp");
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
                                    dir = dir.Substring(0, dir.Length - 3).Replace("\\", "/");
                                    core.executeJSCode(main_engine, "", dir);
                                }
                                catch (Exception i)
                                {
                                    Console.WriteLine(i);
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (System.IO.File.Exists(Environment.CurrentDirectory + "\\temp.js"))
                                    {
                                        File.Delete(Environment.CurrentDirectory + "\\temp.js");
                                    }
                                    string[] deps =
                                    {
                                        "var File = require(\"file\");",
                                        "var WebClient = require(\"webClient\");",
                                        "var Math = require(\"math\");",
                                        "var Environment = require(\"Environment\");",
                                        "var console = require(\"console\");",
                                        "var stringBuilder = require(\"stringBuilder\");",
                                        "var array = require(\"Array\");",
                                        "var directory = require(\"Directory\");",
                                        "var proccess = require(\"Process\");",
                                        "var driveInfo = require(\"DriveInfo\");",
                                        "var exception = require(\"Exception\");",
                                        "var thread = require(\"Thread\");",
                                        "var encoding = require(\"Encoding\");",
                                        "var smtpClient = require(\"SmtpClient\");",
                                        "var cultureInfo = require(\"CultureInfo\");"
                                    };

                                    string[] init =
                                    {
                                        //s"var file = new File();",
                                        //"var zip = new Zip();",
                                        //"var web = new WebClient();",
                                        //"var math = new math();"
                                        //"var env = new Environment();",
                                    };

                                    string[] code = { cmd };

                                    List<string> arr = new List<string>();
                                    arr.AddRange(deps);
                                    arr.AddRange(init);
                                    arr.AddRange(code);

                                    File.WriteAllLines(Environment.CurrentDirectory + "\\temp.js", arr.ToArray());
                                    var ret = core.executeJSCode(main_engine, "", "./temp");
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine(ret);
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                catch (Exception i)
                                {
                                    Console.WriteLine(i);
                                }
                            }
                        }
                    }
                }
                if(type == "p")
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
            else
            {
                try
                {
                    string file = args[0];
                    if (file.Contains(".js"))
                    {
                        var core = new JScore();
                        var main_engine = core.createEngine();
                        string module = file.Substring(0, file.Length - 3).Replace("\\", "/");
                        core.executeJSCode(main_engine, "", module);
                    }

                    if (file.Contains(".json"))
                    {
                        string json = System.IO.File.ReadAllText(file);
                        var dat = JsonConvert.DeserializeObject<jsonDat>(json);
                        var core = new JScore();
                        var main_engine = core.createEngine();
                        foreach (string j in dat.javaScript)
                        {
                            string module = j.Substring(0, file.Length - 3).Replace("\\", "/");
                            core.executeJSCode(main_engine, "", module);
                        }
                    }

                    if (file.Contains(".py"))
                    {
                        var python = new PythonClass();
                        string dat = File.ReadAllText(file);
                        python.ExecutePythonCommand(dat);
                    }
                }
                catch(Exception i)
                {
                    Console.WriteLine(i);
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


    }

    public class jsonDat
    {
        public string[] javaScript;
    }
}