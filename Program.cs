using System;
using System.IO;
using Jint;

namespace JSparkerEngine
{
    class ProgramCore
    {
        static void Main(string[] args)
        {
            ProgramCore prm = new ProgramCore();
            Console.WriteLine(" _ -- Welcome To JSPY-Parker-Engine 1.0v -- _");
            Console.WriteLine("note: would have a nice gui, but .net core... you know. Might do it in electron.js");
            Console.WriteLine("type /open to open a javascript/python file");

            Console.Write("Do you want python or javascript? (p or j)?");
            string type = Console.ReadLine().Trim();


            if (type.ToLower() == "j")
            {

                Console.WriteLine("");


                JScore core = new JScore();
                int main_engine = core.createEngine();

                Console.Write("Do you want to add modules to the javascript? and add C# functions? (Y/N): ");
                string check = Console.ReadLine().Trim().ToLower();
                if (check == "y")
                {
                    Engine ee = prm.addModules(core.getJSengine(main_engine));
                    core.setJSEngine(ee, main_engine);
                }

                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(":> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    string cmd = Console.ReadLine();
                    if (cmd != "")
                    {
                        if (cmd.Trim() == "/open")
                        {
                            Console.WriteLine("Please enter or paste the directory:");
                            string dir = Console.ReadLine();
                            try
                            {
                                core.executeJSCode(main_engine,System.IO.File.ReadAllText(dir));

                            }
                            catch (Exception i)
                            {
                                Console.WriteLine(i);
                            }
                        }
                        else
                        {
                            object ret = core.executeJSprogramWithReturn(main_engine, cmd);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(ret);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                }
            }
            else
            {

                PythonClass python = new PythonClass();
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(":> ");
                    Console.ForegroundColor = ConsoleColor.White;
                    string cmd = Console.ReadLine();
                    if (cmd != "")
                    {
                        if (cmd.Trim() == "/open")
                        {
                            Console.WriteLine("Please enter or paste the directory:");
                            string dir = Console.ReadLine();
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
