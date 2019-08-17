using System;
using System.Collections.Generic;
using System.IO;
using Jint;
using Jint.Native;
using Jint.Parser;
using Jint.Parser.Ast;

namespace JSparkerEngine
{
    public class JScore
    {
        private readonly List<Engine> engine = new List<Engine>();
        private List<Program> JSprograms = new List<Program>();

        private static void AddScript(Engine jintEngine, string ravenDatabaseJsonMapJs)
        {
            jintEngine.Execute(GetFromResources(ravenDatabaseJsonMapJs), new ParserOptions
            {
                Source = ravenDatabaseJsonMapJs
            });
        }

        private static string GetFromResources(string resourceName)
        {
            //Assembly assem = typeof(ScriptedJsonPatcher).Assembly;
            using (Stream stream = new FileStream(resourceName, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// add a JavaScript Engine
        /// </summary>
        /// <returns>JS Engine ID integer</returns>
        public int createEngine()
        {
            try
            {
                var eng = new Engine(cfg =>
                {
                    cfg.AllowClr();
#if DEBUG
                    cfg.AllowDebuggerStatement();
#else
				    cfg.AllowDebuggerStatement(false);
#endif
                });
                var addon = new C_sharp_addon(eng);
                eng = addon.getEngine();

                engine.Add(eng);
                return engine.Count - 1;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return -1;
            }
        }

        /// <summary>
        /// get the raw JavaScript Engine
        /// </summary>
        /// <param name="engineID">The Engine ID integer</param>
        /// <returns>the JS Engine Object</returns>
        public Engine getJSengine(int engineID)
        {
            return engine[engineID];
        }

        /// <summary>
        /// set the raw JS Engine, not for creating a new engine. Only replacing a JS Engine
        /// </summary>
        /// <param name="e">The JS Engine object</param>
        /// <param name="id">The Engine ID</param>
        public void setJSEngine(Engine e, int id)
        {
            engine[id] = e;
        }

        /// <summary>
        /// add a new JS Engine to the engine List
        /// </summary>
        /// <param name="eng">JS Engine Object</param>
        /// <returns>the ID of the new JS Engine</returns>
        public int addJSEngine(Engine eng)
        {
            engine.Add(eng);
            return engine.Count - 1;
        }

        /// <summary>
        /// execute JavaScript Code
        /// </summary>
        /// <param name="engineID">engine ID to execute</param>
        /// <param name="code">The JavaScript Code</param>
        /// <returns>return success or failure to execute boolean</returns>
        public bool executeJSCode(int engineID, string code)
        {
            try
            {
                engine[engineID].Execute(code);
                return true;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return false;
            }
        }

        /// <summary>
        /// set a value as in a primitive, or delegate (custom js function as a C# method) 
        /// </summary>
        /// <param name="setValues">the value tuple. item1 = value name, item2 = type (in32, double...), item3 = actual value</param>
        /// <param name="id">the JS engine ID</param>
        public void setValues(List<Tuple<string, string, object>> setValues, int id)
        {
            foreach (var val in setValues)
                switch (val.Item2)
                {
                    case "int32":
                        engine[id].SetValue(val.Item1, Convert.ToInt32(val.Item3));
                        break;

                    case "int64":
                        engine[id].SetValue(val.Item1, Convert.ToInt64(val.Item3));
                        break;

                    case "double":
                        engine[id].SetValue(val.Item1, Convert.ToDouble(val.Item3));
                        break;

                    case "string":
                        engine[id].SetValue(val.Item1, Convert.ToString(val.Item3));
                        break;

                    case "float":
                        engine[id].SetValue(val.Item1, float.Parse((string) val.Item3));
                        break;

                    case "bool":
                        engine[id].SetValue(val.Item1, Convert.ToBoolean(val.Item3));
                        break;

                    case "object":
                        engine[id].SetValue(val.Item1, val.Item3);
                        break;

                    case "Undefined":
                        engine[id].SetValue(val.Item1, JsValue.Undefined);
                        break;
                }
        }

        /// <summary>
        /// executes the JS code with vales to set.
        /// </summary>
        /// <param name="engineID">the JS engine ID</param>
        /// <param name="code">the JavaScript Code</param>
        /// <param name="input">the JSInputVal object for custom values to add to the JS Engine</param>
        /// <returns>return the success or failure boolean</returns>
        public bool executeJSCode(int engineID, string code, JSInputVal input = null)
        {
            try
            {
                if (input != null) setValues(input.getData(), engineID);
                var e = engine[engineID].Execute(code);
                return true;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                //Log.RecordError(i);
                return false;
            }
        }

        /// <summary>
        /// execute JavaScript Code 
        /// </summary>
        /// <param name="engineID">Engine ID</param>
        /// <param name="code">The JavaScript Code</param>
        /// <returns>returns the JS object</returns>
        public object executeJSprogramWithReturn(int engineID, string code)
        {
            try
            {
                var e = engine[engineID].Execute(code);
                var ret = e.GetCompletionValue().ToObject();
                return ret;
            }
            catch (Exception i)
            {
                return null;
            }
        }

        /// <summary>
        /// Execute a javascript specific function
        /// </summary>
        /// <param name="engineID">Engine ID</param>
        /// <param name="name">the Function Name</param>
        /// <param name="args">an array of objects as the arguments of the JS function</param>
        /// <returns>the JS object</returns>
        public object invokeJSFunc(int engineID, string name, object[] args)
        {
            try
            {
                return engine[engineID].Invoke(name, args).ToObject();
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }

        /// <summary>
        /// execute JS code through a Program
        /// </summary>
        /// <param name="engineID">the JS engine ID</param>
        /// <param name="p">The Program</param>
        /// <returns>the JS object</returns>
        public object executeJSprogramWithReturn(int engineID, Program p)
        {
            try
            {
                var e = engine[engineID].Execute(p);
                var ret = e.GetCompletionValue().ToObject();
                return ret;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }
    }

    public class JSInputVal
    {
        private readonly List<Tuple<string, string, object>> vars = new List<Tuple<string, string, object>>();

        /// <summary>
        /// add a string variable to the JS Engine
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="val">variable value</param>
        public void addStringVar(string name, string val)
        {
            vars.Add(Tuple.Create(name, "string", (object) val));
        }

        /// <summary>
        /// add a integer variable to the JS engine
        /// </summary>
        /// <param name="name">variable name</param>
        /// <param name="val">variable value</param>
        public void addIntegerVar(string name, int val)
        {
            vars.Add(Tuple.Create(name, "int32", (object) val));
        }

        /// <summary>
        /// add a double variable to the JS engine
        /// </summary>
        /// <param name="name">the variable name</param>
        /// <param name="val">the variable value</param>
        public void addDoubleVar(string name, double val)
        {
            vars.Add(Tuple.Create(name, "double", (object) val));
        }

        public void addFloatVar(string name, float val)
        {
            vars.Add(Tuple.Create(name, "float", (object) val));
        }

        public void addBoolVar(string name, bool val)
        {
            vars.Add(Tuple.Create(name, "bool", (object) val));
        }

        public void addObjectVar(string name, object val)
        {
            vars.Add(Tuple.Create(name, "object", val));
        }

        /// <summary>
        /// get the List of tuples to set the values in the JS Engine
        /// </summary>
        /// <returns>Tuple<string,string,string></returns>
        public List<Tuple<string, string, object>> getData()
        {
            return vars;
        }
    }
}