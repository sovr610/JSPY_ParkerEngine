using Jint;
using Jint.Native;
using Jint.Parser;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace JSparkerEngine
{


    public class JScore
    {
        private List<Engine> engine = new List<Engine>();
        private List<Program> JSprograms = new List<Program>();

        public JScore()
        {

        }

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
            using (Stream stream = new System.IO.FileStream(resourceName, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

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
                C_sharp_addon addon = new C_sharp_addon(eng);
                eng = addon.getEngine();

                engine.Add(eng);
                return engine.Count - 1;

            }
            catch(Exception i)
            {
                Console.WriteLine(i);
                return -1;
            }
        }

        public Engine getJSengine(int engineID)
        {
            return engine[engineID];
        }

        public int addJSEngine(Engine eng)
        {
            engine.Add(eng);
            return engine.Count - 1;
        }

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

        public void setValues(List<Tuple<string, string, object>> setValues, int id)
        {
            foreach (var val in setValues)
            {
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
                        engine[id].SetValue(val.Item1, float.Parse((string)val.Item3));
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
        }

        public bool executeJSCode(int engineID, string code, JSInputVal input = null)
        {
            try
            {
                
                if(input != null)
                {
                    setValues(input.getData(), engineID);
                }
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

        public object executeJSprogramWithReturn(int engineID,string code)
        {
            try
            {
                Engine e = engine[engineID].Execute(code);
                var ret = e.GetCompletionValue().ToObject();
                return ret;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }

        public object invokeJSFunc(int engineID, string name, object[] args)
        {
            try
            {
                return engine[engineID].Invoke(name, args).ToObject();
                
            }
            catch(Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }

        public object executeJSprogramWithReturn(int engineID, Program p)
        {
            try
            {
                Engine e = engine[engineID].Execute(p);
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
        private List<Tuple<string, string, object>> vars = new List<Tuple<string, string, object>>();
        public void addStringVar(string name, string val)
        {
            vars.Add(Tuple.Create(name, "string", (object)val));
        }

        public void addIntegerVar(string name, int val)
        {
            vars.Add(Tuple.Create(name, "int32", (object)val));
        }

        public void addDoubleVar(string name, double val)
        {
            vars.Add(Tuple.Create(name, "double", (object)val));
        }

        public void addFloatVar(string name, float val)
        {
            vars.Add(Tuple.Create(name, "float", (object)val));
        }

        public void addBoolVar(string name, bool val)
        {
            vars.Add(Tuple.Create(name, "bool", (object)val));
        }

        public void addObjectVar(string name, object val)
        {
            vars.Add(Tuple.Create(name, "object", val));
        }

        public List<Tuple<string, string, object>> getData()
        {
            return vars;
        }
    }


}


