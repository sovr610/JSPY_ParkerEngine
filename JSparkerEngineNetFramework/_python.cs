using System;
using System.Threading;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace JSparkerEngine
{
    public class PythonClass
    {
        private readonly ScriptEngine _python;

        public PythonClass()
        {
            _python = Python.CreateEngine();
        }

        /// <summary>
        ///     a single line command of python to execute
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool ExecutePythonCommand(string cmd)
        {
            try
            {
                _python.Execute(cmd);
                return true;
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
                return false;
            }
        }

        /// <summary>
        ///     execute a whole python file
        /// </summary>
        /// <param name="file">filename</param>
        /// <param name="threaded">to run in a seperate thread if true</param>
        /// <returns>if successful or not</returns>
        public bool ExecutePythonFile(string file, bool threaded)
        {
            try
            {
                if (!threaded)
                {
                    _python.ExecuteFile(file);
                }
                else
                {
                    var tp = new threadPython(_python);
                    tp.setFile(file);
                    var t = new Thread(tp.runFile);
                    t.Start();
                }

                return true;
            }
            catch (Exception i)
            {
                Console.WriteLine(i.Message);
                return false;
            }
        }

        /// <summary>
        ///     same as running a python file, but treated as an app.
        /// </summary>
        /// <param name="file">app name</param>
        public void RunPythonApp(string file)
        {
            ExecutePythonFile(Environment.CurrentDirectory + "\\apps\\" + file + ".py", true);
        }
    }

    /// <summary>
    ///     for multi-threading the python file execution
    /// </summary>
    internal class threadPython
    {
        private readonly ScriptEngine _python;
        private string fileName;

        public threadPython(ScriptEngine _p)
        {
            _python = _p;
        }

        public void setFile(string filename)
        {
            fileName = filename;
        }

        public void runFile()
        {
            try
            {
                _python.ExecuteFile(fileName);
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
            }
        }
    }
}