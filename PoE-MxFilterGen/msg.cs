using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_MxFilterGen
{
    class msg
    {
        public static void CM(string msg, bool time, int color)
        {
            string seconds = "";
            string minutes = "";
            string hours = "";
            if (DateTime.Now.Second < 10)
            {
                seconds = String.Format("0{0}", DateTime.Now.Second);
            }
            else
            {
                seconds = DateTime.Now.Second.ToString();
            }

            if (DateTime.Now.Minute < 10)
            {
                minutes = String.Format("0{0}", DateTime.Now.Minute);
            }
            else
            {
                minutes = DateTime.Now.Minute.ToString();
            }

            if (DateTime.Now.Hour < 10)
            {
                hours = String.Format("0{0}", DateTime.Now.Hour);
            }
            else
            {
                hours = DateTime.Now.Hour.ToString();
            }

            string date = String.Format("{0}:{1}:{2}", hours, minutes, seconds);


            //color switch
            ConsoleColor cc = ConsoleColor.White;
            switch (color)
            {
                case 0:
                    //nothing
                    break;
                case 1:
                    cc = ConsoleColor.Cyan;
                    break;
                case 2:
                    cc = ConsoleColor.Green;
                    break;
                case 3:
                    cc = ConsoleColor.Red;
                    break;
            }

            if (time)
            {
                Console.ForegroundColor = cc;
                Console.WriteLine(String.Format("[{0}] {1}", date, msg));
            }
            else
            {
                Console.ForegroundColor = cc;
                Console.WriteLine(String.Format("{0}", msg));
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void CMW(string msg, bool time, int color)
        {
            string seconds = "";
            string minutes = "";
            string hours = "";
            if (DateTime.Now.Second < 10)
            {
                seconds = String.Format("0{0}", DateTime.Now.Second);
            }
            else
            {
                seconds = DateTime.Now.Second.ToString();
            }

            if (DateTime.Now.Minute < 10)
            {
                minutes = String.Format("0{0}", DateTime.Now.Minute);
            }
            else
            {
                minutes = DateTime.Now.Minute.ToString();
            }

            if (DateTime.Now.Hour < 10)
            {
                hours = String.Format("0{0}", DateTime.Now.Hour);
            }
            else
            {
                hours = DateTime.Now.Hour.ToString();
            }

            string date = String.Format("{0}:{1}:{2}", hours, minutes, seconds);

            File.AppendAllText("mxfiltergen.logs", String.Format("[{0}] {1}", date, msg) + Environment.NewLine);

            //color switch
            ConsoleColor cc = ConsoleColor.White;
            switch (color)
            {
                case 0:
                    //nothing
                    break;
                case 1:
                    cc = ConsoleColor.Cyan;
                    break;
                case 2:
                    cc = ConsoleColor.Green;
                    break;
                case 3:
                    cc = ConsoleColor.Red;
                    break;
            }

            if (time)
            {
                Console.ForegroundColor = cc;
                Console.WriteLine(String.Format("[{0}] {1}", date, msg));
            }
            else
            {
                Console.ForegroundColor = cc;
                Console.WriteLine(String.Format(""));
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Splash()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("");
            Console.WriteLine("#### MxFilterGen");
            Console.WriteLine(string.Format("#### VERSION: {0}",main.version));
            Console.WriteLine("#### DEV: mikx");
            Console.WriteLine("#### POWERED BY: poe.ninja");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
