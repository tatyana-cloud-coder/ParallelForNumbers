using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;

namespace _273BalashovaParallel
{
    class Program
    {
        static ConcurrentBag<Result> _results = new ConcurrentBag<Result>();
        static void Main(string[] args)
        {
            WriteToFile("randonNumbers.txt");
            string text = File.ReadAllText("randonNumbers.txt");
            var collection = text.Split(' ');
            var infs = new List<int>();
            var sups = new List<int>();
            var isValid = true; 

            Console.WriteLine("Количество потоков:");
            if (int.TryParse (Console.ReadLine(), out var n))
            {
                for (int i = 0; i < n; i++)
                {
                    Console.WriteLine("Нижняя граница диапазона:");

                    if (int.TryParse(Console.ReadLine(), out var inf))
                    {
                        infs.Add(inf); 
                        Console.WriteLine("Верхняя граница диапазона:");


                        if (int.TryParse(Console.ReadLine(), out var sup))
                        {
                            sups.Add(sup);
                        } else
                        {
                            Console.WriteLine("Некорректная верхняя граница диапазона");
                            isValid = false;
                            break;

                        }
                           
                    } else
                    {
                        Console.WriteLine("Некорректная нижняя граница диапазона");
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    
                    ThreadPocket.Numbers = collection;
                    var threads = new List<Thread>();
                    for (int i = 0; i<n; i++)
                    {
                        var pocket = new ThreadPocket();
                        threads.Add(new Thread(new ParameterizedThreadStart(ProccessNumbers)));
                        pocket.Inf = infs[i];
                        pocket.Sup = sups[i];
                        threads[i].Start(pocket);
                    }
                    WaitAll(threads);
                }

            } else
            {
                Console.WriteLine("Некорректное количество потоков");
            }


            Console.WriteLine();
            Console.WriteLine("Результаты:");
            Console.WriteLine($" {"Число",-10} {"Позиция",-10} {"Номер потока",-3}");
            foreach (var res in _results.OrderBy(x => x.Number).ThenBy(x=>x.Position))
            {
                Console.WriteLine($" {res.Number,-10} {res.Position,-10} {res.ThreadNumber,-3}");
            }
            Console.ReadKey();
          
        }

        static void WriteToFile (string fileName)
        {
            Random rnd = new Random();
            var countNumbers = 1000000;
            var maxValue = 1000000;
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (int i = 0; i < countNumbers; i ++)
                {
                    var num = rnd.Next(0, maxValue);
                    writer.Write(num + " ") ;
                }
             
            }
        }


        static void ProccessNumbers (object argument)
        {
            ThreadPocket pocket = argument as ThreadPocket;
            int inf = pocket.Inf;
            int sup = pocket.Sup;
            var numbers = ThreadPocket.Numbers;
            for ( int i = inf; i <= sup; i++)
            {
                var num = int.Parse(numbers[i]);
                if (num % 77777 == 0 )
                {
                    _results.Add(new Result
                    {
                        Number = num,
                        Position = i,
                        ThreadNumber = Thread.CurrentThread.ManagedThreadId
                    });
                    
                }
            }
        }

        static void WaitAll(IEnumerable<Thread> threads)
        {
            if (threads != null)
            {
                foreach (Thread thread in threads)
                {
                    thread.Join();
                }
            }
        }
    }
}
