﻿

using AlgoQuora;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace TestSortedList; 

internal class Program {
    static void Assert(bool v) { if (!v) throw new Exception("bad"); }
    static void Test() {

        Random rnd = new();
        int next() { 
            //return rnd.Next(0, 100);
            return rnd.Next(0, 10000);
        }

        // avr depth
        {
            SortedListChecked<int> a = new();
            for (int q = 0; q < 1000000; q++) {
                a.Add(rnd.Next(0, 1000000));
            }
            a.SelfCheckRules();
            Console.WriteLine($"avr depth: {a.tot_depth / a.cnt_depth}, max={a.max_depth}");
        }

        // QuickList
        {
            var quick = new QuickList<int>();
            var checker = new List<int>();
            for (int j = 0; j < 1000; j++) {
                var v = next();
                var i = rnd.Next(0, checker.Count);
                quick.InsertAt(i, v);
                checker.Insert(i, v);
                v = next();
                quick.Add(v);
                checker.Add(v);
            }
            Assert(checker.SequenceEqual(quick));
        }

        // remove all test
        {
            for (int j = 0; j < 10; j++) {
                SortedListChecked<int> a = new();
                for (int i = 0; i < 1000; i++) {
                    a.Add(rnd.Next(0, 10));
                }
                a.RemoveAllOf(rnd.Next(0, 10));
                a.SelfCheckRules();
            }
        }

        // map test
        {
            Map<int, int> a = new();
            SortedDictionary<int, int> dict = new();
            for (int i = 0; i < 500; i++) {
                var v = rnd.Next(0, 5000);
                a[v]++; if (!dict.TryAdd(v, 1)) dict[v]++;
            }
            if (!dict.Keys.SequenceEqual(a.Select(x => x.Key))) throw new Exception("bad");
            if (!dict.Values.SequenceEqual(a.Select(x => x.Value))) throw new Exception("bad");
        }

        do {

            // common test
            {
                SortedList_Tester<int> tester = new();
                //for (int j = 0; j < 5000; j++) {
                for (int j = 0; j < 50000; j++) {

                    if (j == 0) {
                        int k = 0;
                    }

                    tester.Add(next(), (next() & 1) == 0);
                    tester.Check();
                    tester.Remove(next());
                    tester.Check();

                    if (tester.RemoveAllOf(next()) != 0) {
                        tester.Check();
                    }

                    tester.CountOf(next());
                    if (tester.Count > 0) {
                        tester.Get(rnd.Next(0, tester.Count));
                        tester.Add(next());
                        tester.RemoveAt(rnd.Next(0, tester.Count));
                        tester.Check();
                    }
                    tester.Contains(next());
                    tester.More(next());
                }
            }
            Console.WriteLine("Test OK");
        } 
        while (false);
        //while (true);


    }

    static void Main(string[] args) {
        Test();
        BenchmarkRunner.Run(typeof(Program).Assembly
            //, new Config()
            );

    }
}


//[MemoryDiagnoser]
//[DisassemblyDiagnoser]
public class MyBenchmarks {
    int N = 300000;
    Random rnd = new(44);
    int seed = new Random().Next(9);
    int next() { return rnd.Next(0,N); }
    public void clear() {
        rnd = new Random(seed);
    }

    public MyBenchmarks() {

    }

    SortedSet<int> sbt = new();
    Set<int> set = new();

    [Benchmark]
    public void WBT_Set_Insert() {
        clear();
        Set<int> list = new();
        for (int i = 0; i < 400000; i++) {
            list.Add(i);
            list.Add(next());
        }
    }

    [Benchmark]
    public void SortedSet_Insert() {
        clear();
        SortedSet<int> list = new();
        for (int i = 0; i < 400000; i++) {
            list.Add(i);
            list.Add(next());
        }
    }

    [Benchmark]
    public void WBT_Set_Add10() {
        clear();
        sbt.Clear();
        for (int i = 0; i < 10; i++) { sbt.Add(next()); }
    }

    [Benchmark]
    public void SortedSet_Add10() {
        clear();
        set.Clear();
        for (int i = 0; i < 10; i++) { set.Add(next()); }
    }

    [Benchmark]
    public void WBT_Set_Add1000() {
        clear();
        sbt.Clear();
        for (int i = 0; i < 1000; i++) { sbt.Add(next()); }
    }

    [Benchmark]
    public void SortedSet_Add1000() {
        clear();
        set.Clear();
        for (int i = 0; i < 1000; i++) { set.Add(next()); }
    }

    [Benchmark]
    public void WBT_Set_AddRemoveContainsRnd() {
        clear();
        Set<int> list = new();
        for (int i = 0; i < N; i++) {
            list.Add(next());
            list.Remove(next());
            list.Contains(next());
        }
    }

    [Benchmark]
    public void SortedSet_AddRemoveContainsRnd() {
        clear();
        SortedSet<int> list = new();
        for (int i = 0; i < N; i++) {
            list.Add(next());
            list.Remove(next());
            list.Contains(next());
        }
    }



    [Benchmark]
    public void WBT_Set_AddRemoveLast() {
        Set<int> list = new();
        for (int i = 0; i < N; i++) {
            list.Add(i);
        }
        for (int i = 0; i < N; i++) {
            list.Remove(i);
        }
    }

    [Benchmark]
    public void SortedSet_AddRemoveLast() {
        SortedSet<int> list = new();
        for (int i = 0; i < N; i++) {
            list.Add(i);
        }
        for (int i = 0; i < N; i++) {
            list.Remove(i);
        }
    }

    [Benchmark]
    public void WBT_Map_AddRemoveRnd() {
        clear();
        Map<int, int> list = new();
        for (int i = 0; i < N; i++) {
            list.Add(next(), next());
            list.Remove(next());
        }
    }

    [Benchmark]
    public void SortedDictionary_AddRemoveRnd() {
        clear();
        SortedDictionary<int, int> list = new();
        for (int i = 0; i < N; i++) {
            list.TryAdd(next(), next());
            list.Remove(next());
        }
    }

    [Benchmark]
    public void Dictionary_AddRemoveRnd() {
        clear();
        Dictionary<int, int> list = new();
        for (int i = 0; i < N; i++) {
            list.TryAdd(next(), next());
            list.Remove(next());
        }
    }

    [Benchmark]
    public void WBT_MultiSet_AddRemoveAll() {
        clear();
        MultiSet<int> list = new();
        for (int i = 0; i < N; i++) {
            list.Add(rnd.Next(0, N / 4));
        }
        for (int i = 0; i < N; i++) {
            list.RemoveAllOf(rnd.Next(0, N / 4));
        }
    }

}
