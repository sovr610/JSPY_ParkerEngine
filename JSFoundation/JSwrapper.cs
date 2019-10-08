using JSparkerEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROS_PAD.JSFoundation
{
    public class JSwrapper
    {
        private JScore core = new JScore();
        private npm_loader npm = new npm_loader();

        public int setup( C_sharp_addon add)
        {
            int id = core.createEngine();
            core.setAddon(id, add);
            return id;
        }

        private string modLoc;

        public void setJavaScriptModuleDir(string file)
        {
            modLoc = file;
            
        }

        public object executeLine(int id, string code, bool runMain)
        {
            return core.executeJSprogramWithReturn(id, code, modLoc, runMain);
        }

        public object invoke(int id, string func, string mod, string js = null, params object[] args)
        {
            
            return core.invoke(id, func, mod, js, args);
        }

    }
}
