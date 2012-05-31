using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnitTestUtil;
using OmahaBot.Core;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(@"C:\Dev\OmahaBot\Tests\bin\Release\HAND_OODS_3_1500.bin")))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Dictionary<ulong, double> calcs = (Dictionary<ulong, double>)bf.Deserialize(ms);

                while (true)
                {
                    string line = Console.ReadLine();
                    Console.WriteLine(calcs[OmahaHandHash.GetHashCode(CardHelper.CreateHandFromString(line))]);
                }
            }
        }
    }
}
