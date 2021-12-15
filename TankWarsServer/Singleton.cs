using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllSever
{
    class Singleton<T>where T:class,new()
    {
        private static T ins;
        public static T Ins
        {

            get {
                if (ins == null)
                {
                    ins = new T();

                }
                return ins;
            }
        }
    }
}
