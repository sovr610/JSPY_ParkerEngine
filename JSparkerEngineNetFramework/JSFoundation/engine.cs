using Jint;
using Jint.CommonJS;
using Jint.Native;
using Jint.Parser;
using Jint.Parser.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;

namespace JSparkerEngine
{


    public class JScore
    {
        private List<PADengine> engine = new List<PADengine>();
        private List<Program> JSprograms = new List<Program>();

        public JScore()
        {

        }

        public object invoke(int id, string func, string mod, string js = null, params object[] obj)
        {
            return engine[id].Invoke(func, mod, js, obj);
        }

        public int createEngine(List<string> scripts = null)
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
                }).SetValue("log", new Action<object>(Console.WriteLine));
                PADengine pad = new PADengine(eng);
                engine.Add(pad);
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
            return engine[engineID].getEngine();
        }

        public int addJSEngine(Engine eng)
        {
            PADengine pad = new PADengine(eng);
            engine.Add(pad);
            return engine.Count - 1;
        }

        public bool executeJSCode(int engineID, string code, string fileLocation)
        {
            try
            {
                engine[engineID].run(code, fileLocation);
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
                        engine[id].setValue(val.Item1, Convert.ToInt32(val.Item3));
                        break;

                    case "int64":
                        engine[id].setValue(val.Item1, Convert.ToInt64(val.Item3));
                        break;

                    case "double":
                        engine[id].setValue(val.Item1, Convert.ToDouble(val.Item3));
                        break;

                    case "string":
                        engine[id].setValue(val.Item1, Convert.ToString(val.Item3));
                        break;

                    case "float":
                        engine[id].setValue(val.Item1, float.Parse((string)val.Item3));
                        break;

                    case "bool":
                        engine[id].setValue(val.Item1, Convert.ToBoolean(val.Item3));
                        break;

                    case "object":
                        engine[id].setValue(val.Item1, val.Item3);
                        break;

                    case "Undefined":
                        engine[id].setValue(val.Item1, JsValue.Undefined);
                        break;
                }
            }
        }

        public void setAddon(int id, C_sharp_addon add)
        {
            engine[id].setAddon(add);
        }

        public bool executeJSCode(int engineID, string code, string fileLocation, JSInputVal input = null)
        {
            try
            {
                
                if(input != null)
                {
                    setValues(input.getData(), engineID);
                }
                var e = engine[engineID].run(code, fileLocation);
                return true;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                //Log.RecordError(i);
                return false;
            }

        }

        public object executeJSprogramWithReturn(int engineID,string code, string fileLocation, bool runMain)
        {
            try
            {
                return engine[engineID].run(code, fileLocation);
                
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }

        public object invokeJSFunc(int engineID, string name, string mod, object[] args, string js = null)
        {
            try
            {
                return engine[engineID].Invoke(name, mod, js, args);
                
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
                return engine[engineID].run(p);
               
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

    public class PADengine
    {
        private Engine eng;
        private List<string> scripts = new List<string>();
        private C_sharp_addon addon;

        public void setAddon(C_sharp_addon addon)
        {
            this.addon = addon;
            eng = addon.loadUpEngine(eng);
        }

        public PADengine(Engine e)
        {
            eng = e;
        }


        public void addScript(string script)
        {
            scripts.Add(script);
        }

        public void addModules(string name, object obj)
        {
            modules.Add(Tuple.Create(name, obj));
        }

        List<Tuple<string, object>> modules = new List<Tuple<string, object>>();

        public ModuleLoadingEngine run(string code, string fileLocation)
        {
            try
            {
                
                preinit();
                eng.CommonJS()
                    .RegisterInternalModule("console", typeof(Console))
                    .RegisterInternalModule("file", typeof(File))
                    .RegisterInternalModule("webClient", typeof(WebClient))
                    .RegisterInternalModule("math", typeof(Math))
                    .RegisterInternalModule("stringBuilder", typeof(StringBuilder))
                    .RegisterInternalModule("Environment", typeof(System.Environment))
                    .RegisterInternalModule("Array", typeof(System.Array))
                    .RegisterInternalModule("BitConverter", typeof(System.BitConverter))
                    .RegisterInternalModule("Directory", typeof(System.IO.Directory))
                    .RegisterInternalModule("File", typeof(System.IO.File))
                    .RegisterInternalModule("DriveInfo", typeof(System.IO.DriveInfo))
                    .RegisterInternalModule("Process", typeof(System.Diagnostics.Process))
                    .RegisterInternalModule("Exception", typeof(Exception))
                    .RegisterInternalModule("Calender", typeof(System.Globalization.Calendar))
                    .RegisterInternalModule("CultureInfo", typeof(System.Globalization.CultureInfo))
                    .RegisterInternalModule("RegionInfo", typeof(System.Globalization.RegionInfo))
                    .RegisterInternalModule("Dns", typeof(System.Net.Dns))
                    .RegisterInternalModule("HttpClient", typeof(System.Net.Http.HttpClient))
                    .RegisterInternalModule("HttpContent", typeof(System.Net.Http.HttpContent))
                    .RegisterInternalModule("HttpResponseMessage", typeof(System.Net.Http.HttpResponseMessage))
                    .RegisterInternalModule("MailMessage", typeof(System.Net.Mail.MailMessage))
                    .RegisterInternalModule("MailAddress", typeof(System.Net.Mail.MailAddress))
                    .RegisterInternalModule("MailAttachment", typeof(System.Net.Mail.Attachment))
                    .RegisterInternalModule("SmtpClient", typeof(System.Net.Mail.SmtpClient))
                    .RegisterInternalModule("Encoding", typeof(System.Text.Encoding))
                    .RegisterInternalModule("Thread", typeof(System.Threading.Thread))
                    .RunMain(fileLocation);







                return null;
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;

            }
        }

        public object run(Program code)
        {
            try
            {
                preinit();
                return eng.Execute(code).GetCompletionValue().ToObject();
            }
            catch (Exception i)
            {
                //Log.RecordError(i);
                return null;

            }
        }

        public void setValue(string name, object val)
        {
            eng.SetValue(name, val);
        }

        public object getValue(string name)
        {
            return eng.GetValue(name);
        }

        public Engine getEngine()
        {
            return eng;
        }

        private void preinit()
        {
            foreach(string scr in scripts)
            {
                AddScript(eng, scr);
            }
        }

        private string fileLoc;
        public void setJavaScriptFile(string file)
        {
            fileLoc = file;
        }

        

        public object Invoke(string name, string mod, string js = null, params object[] args)
        {
            try
            {
                ModuleLoadingEngine enng = null;
                if (js != null)
                {
                    enng = this.run(js, mod);
                }
                if(args != null)
                {
                    if(args[0] != null)
                    {
                        
                        return eng.Invoke(name, args).ToObject();
                    }
                    else
                    {
                        return eng.Invoke(name).ToObject();
                    }
                }
                else
                {
                    return eng.Invoke(name).ToObject();
                }
            }
            catch(Exception i)
            {
                //Log.RecordError(i);
                return null;
            }
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
            using (Stream stream = new System.IO.FileStream(Environment.CurrentDirectory + "\\" + resourceName, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }


}


