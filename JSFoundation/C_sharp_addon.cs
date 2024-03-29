﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Jint;
using ROS_PAD;

using Jint.CommonJS;

namespace JSparkerEngine
{
    public class C_sharp_addon
    {
        private Engine engine;

        public void addAction(string name, Action<object> a)
        {
            engine.SetValue(name,a);
        }

        public void addAction(string name, Action<string> a)
        {
            engine.SetValue(name, a);
        }

        public void addAction(string name, Action<double> a)
        {
            engine.SetValue(name, a);
        }

        public void addAction(string name, Action<float> a)
        {
            engine.SetValue(name, a);
        }

        public void addAction(string name, Action<bool> a)
        {
            engine.SetValue(name, a);
        }

        public void addAction(string name, Action<string,string> a)
        {
            engine.SetValue(name, a);
        }

        public void addAction(string name, Action<int,int> a)
        {
            engine.SetValue(name, a);
        }

        public Engine getEngine()
        {
            return engine;
        }

        public C_sharp_addon()
        {
        }

        

        private List<string> modules = new List<string>();


        

        public Engine loadUpEngine(Engine e)
        { 
            engine = e;
            /*engine.SetValue("writeFile", new Action<string,string>(File.WriteAllText));
            engine.SetValue("deleteFile", new Action<string>(File.Delete));
            engine.SetValue("fileExists", new Func<string,bool>(File.Exists));
            engine.SetValue("readFile", new Func<string, string>(File.ReadAllText));

            engine.CommonJS().RegisterInternalModule("console", typeof(Console));//.RunMain("./");
            modules.Add("console");*/

            return engine;
            //engine.SetValue("fileExists", new Action<bool,string>(File.Exists));
        }
    }
}
