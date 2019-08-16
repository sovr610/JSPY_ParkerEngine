using System;

namespace JSparkerEngine
{
    class ProgramCore
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" _ -- Welcome To JS-Parker-Engine 1.0v -- _");
            Console.WriteLine("");

            JScore core = new JScore();
            int main_engine = core.createEngine();

            while (true)
            {
                Console.Write(":> ");
                string cmd = Console.ReadLine();
                if(cmd != "")
                {
                    object ret = core.executeJSprogramWithReturn(main_engine, cmd);
                    Console.WriteLine(ret);
                }
            }
        }
    }
}
