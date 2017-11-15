using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace Monitor
{
    
    class Program
    {
        static void Main(string[] args)
        {
          
            var th = new ServerThread();

            Thread ServerThread = new Thread(th.Run);
            ServerThread.Start();

            Console.ReadKey();
        }
    }
}
