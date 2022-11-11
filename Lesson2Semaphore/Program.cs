using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson2Semaphore
{
    internal class Work
    {
        static Semaphore semaphore = new Semaphore(initialCount: 0, maximumCount: 3);

        public static void Produce(object id)
        {
            Console.WriteLine("Producer {0} begins to produce.", id);
            while (true)
            {
                Thread.Sleep(1200);
                Console.WriteLine("Producer {0} made a work.", id);
                semaphore.Release();
                Console.WriteLine("Producer {0} released the semaphore and begins to make new work.", id);
            }
        }

        public static void Consume(object id)
        {
            Console.WriteLine("Consumer {0} begins to wait.", id);

            while (true)
            {
                semaphore.WaitOne();
                Console.WriteLine("Consumer {0} enters the semaphore.", id);
                Thread.Sleep(3000);
                Console.WriteLine("Consumer {0} completes consumption and begins to wait.", id);
            }
        }

        public static void Main()
        {


            for (int i = 0; i < 1; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Work.Produce));
                t.Start(i);
            }
            for (int i = 0; i < 5; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Work.Consume));
                t.Start(i);
            }
            Thread.Sleep(500 * 1000);
        }
    }
}


