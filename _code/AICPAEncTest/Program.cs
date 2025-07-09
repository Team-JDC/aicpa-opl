using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AICPAEncTest
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            bool decr = false;
            bool ask = true;
            string input = string.Empty;
            foreach (string arg in args)
            {
                if (arg.StartsWith("/t="))
                {
                    input = arg.Remove(0, 3);
                    ask = true;
                }
                if (arg.StartsWith("/d"))
                {
                    decr = true;
                }                
            }

            if (ask)
            {
                if (decr)
                {
                    Console.Write("Enter Text to Decrypt:");
                    input = Console.ReadLine();
                }
                else
                {
                    Console.Write("Enter Text to Encrypt:");
                    input = Console.ReadLine();
                }
            }
            else
            {
                input = args[1];
            }

            AICPA.Destroyer.Shared.AICPAEncryption enc = new AICPA.Destroyer.Shared.AICPAEncryption();
            string output = string.Empty;
            if (decr)
            {
                output = enc.Decrypt(input);
                //Console.WriteLine();
            }
            else
            {
                output = enc.Encrypt(input);
                //Console.WriteLine(enc.Encrypt(input));
            }
            Console.WriteLine("Result: " + output);
            using(StreamWriter sw = new StreamWriter("log.txt",true))
            {
                sw.WriteLine(string.Format("{0},{1}", input, output));
                sw.Close();
            }
            Console.ReadKey();
            enc = null;
        }
    }
}
