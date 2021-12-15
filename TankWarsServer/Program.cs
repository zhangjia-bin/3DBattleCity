using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSever
{
    class Program
    {
        static void Main(string[] args)
        {
            PlayerMove.Ins.Init();
            NetManager.Ins.Init();
            Console.ReadKey();
        }
    }
}
