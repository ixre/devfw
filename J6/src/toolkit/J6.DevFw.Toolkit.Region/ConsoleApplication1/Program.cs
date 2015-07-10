using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ops.Regions;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("::List Provinces");
            foreach (Province p in Region.Provinces)
            {
                Console.Write(p.Text+",");
            }
            Console.WriteLine("\n::Get cities of 北京市\n");

            foreach (City c in Region.GetCities(Region.GetProvince("北京市").ID))
            {
                Console.Write(c.Text + ",");
            }

            Console.WriteLine("\n::Get districts of 北京市\n");

            foreach (District c in Region.GetDistricts(Region.GetCity("北京市").ID))
            {
                Console.Write(c.Text + ",");
            }


            Console.WriteLine("\n::Get districts of 巴中市\n");
            foreach (District c in Region.GetDistricts(Region.GetCity("巴中市").ID))
            {
                Console.Write(c.Text + ",");
            }

            Console.ReadKey();
        }
    }
}
