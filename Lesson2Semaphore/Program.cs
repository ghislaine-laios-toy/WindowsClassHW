namespace Lesson2Semaphore;

internal class Work
{
    private static readonly Semaphore semaphore = new(0, 3);

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
        for (var i = 0; i < 1; i++)
        {
            var t = new Thread(Produce);
            t.Start(i);
        }

        for (var i = 0; i < 5; i++)
        {
            var t = new Thread(Consume);
            t.Start(i);
        }

        Thread.Sleep(500 * 1000);
    }
}