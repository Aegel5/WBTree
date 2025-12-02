

using AlgoQuora;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Disassemblers;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Numerics;

namespace TestSortedList; 

internal class Program {
    static void Assert(bool v) { if (!v) throw new Exception("bad"); }
    static void Test() {

        Random rnd = new(1);
        int next(int max = 10000) { 
            //return rnd.Next(0, 100);
            return rnd.Next(0, max+1);
        }

        // avr depth
        {
            SortedListChecked<int> a = new();
            for (int q = 0; q < 100000; q++) {
                a.Add(next(100000));
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
                for (int i = 0; i < 10000; i++) {
                    a.Add(rnd.Next(0, 10));
                }
                //a.rototions_cnt = 0;
                var cnt_removed = a.RemoveAllOf(rnd.Next(0, 10));
                //Console.WriteLine($"{cnt_removed} {a.rototions_cnt}");
                a.SelfCheckRules();
            }
        }

        // remove while enumerate - НЕЛЬЗЯ!
        //{
        //    for (int j = 0; j < 10; j++) {
        //        Set<int> a = new(Enumerable.Range(1,100));
        //        var check = a.ToList();
        //        foreach (var item in next(1)==1 ? a : a.Skip(next(a.Count))) {
        //            if (next(1) == 1) {
        //                a.Remove(item);
        //                check.Remove(item);
        //            }
        //        }
        //        Assert(a.SequenceEqual(check));
        //    }
        //}

        // Merge test
        {
            for (int i_ = 0; i_ < 100; i_++) {
                SortedListChecked<int> a = new();
                SortedListChecked<int> b = new();
                var c1 = next(10000);
                var c2 = next(10);
                if (next(1) == 1) (c1, c2) = (c2, c1);
                for (int i = 0; i < c1; i++) a.Add(next(c1));
                for (int i = 0; i < c2; i++) b.Add(rnd.Next(c1+1, c1+1+c2));
                a.Append(b);
                a.SelfCheckRules();
            }
        }

        // split-merge test
        {
            int max_split = 0;
            int max_merge = 0;
            while (true) {

                SortedListChecked<int> a = new();
                SortedListChecked<int> b = new();
                for (int i = 0; i < 10000; i++) a.Add(next(1000000));
                var lst = a.ToArray();
                //a.rototions_cnt = 0;
                //a.call_cnt = 0;
                var (c, d) = a.Split(next(lst.Length));
                //max_split = Math.Max(max_split, a.rototions_cnt);
                //Console.WriteLine($"split rots: {a.rototions_cnt}, max = {max_split}");

                Assert(c.Count + d.Count == lst.Length);
                c.SelfCheckRules();
                d.SelfCheckRules();
                //for (int i = 0; i < next(10000); i++) {
                //    if (c.Count > 0) {
                //        c.RemoveAt(next(c.Count - 1));
                //        c.Add(next(0));
                //    }
                //    if (d.Count > 0) {
                //        d.RemoveAt(next(d.Count - 1));
                //        d.Add(10000000);
                //    }
                //}

                //c.rototions_cnt = 0;
                c.Append(d);
                //max_merge = Math.Max(max_merge, a.rototions_cnt);
                //Console.WriteLine($"merge rots: {c.rototions_cnt}, max = {max_merge}");

                c.SelfCheckRules();
                Assert(lst.SequenceEqual(c));
                //Console.WriteLine(a.avr_depth);
                //Console.WriteLine(a.d_test);
                break;
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
                for (int j = 0; j < 1000; j++) {
                //for (int j = 0; j < 50000; j++) {

                    if (j == 0) {
                        int k = 0;
                    }

                    tester.CountOf(next());

                    tester.Add(next(), (next() & 1) == 0);
                    tester.Check();
                    tester.Remove(next());
                    tester.Check();

                    if (tester.RemoveAllOf(next()) != 0) {
                        tester.Check();
                    }

                    tester.Add(next());

                    tester.Get(rnd.Next(0, tester.Count));

                    tester.Add(next());
                    tester.RemoveAt(rnd.Next(0, tester.Count));
                    tester.Check();

                    tester.Contains(next());

                    tester.More_Index(next());
                    tester.LessEq(tester.checker[next(tester.Count-1)]); // существующий
                    tester.LessEq(next());
                    tester.MoreEq(next());
                }
                Console.WriteLine(tester.lst.Count);
            }
            Console.WriteLine("Test OK");
        } 
        while (false);
        //while (true);


    }

    static void Main(string[] args) {
        Test();
        //var pfujdifu = new MyBenchmarks();
        //pfujdifu.WBT_Set_BS1();
        //Console.WriteLine(pfujdifu.RES);
        //return;
        BenchmarkRunner.Run(typeof(Program).Assembly
            //, new Config()
            );

    }
}


//[MemoryDiagnoser]
//[DisassemblyDiagnoser]
public class MyBenchmarks {
    int N = 300000;
    Random rnd;
    int next(int max = int.MaxValue) { return rnd.Next(0,max == int.MaxValue?N:max+1); }
    public void clear() {
        rnd = new Random(999);
    }

    public MyBenchmarks() {
        clear();
        for (int i = 0; i < N; i++) { filled.Add(next()); }
    }
    SortedListChecked<int> filled = new();
    SortedSet<int> sbt = new();
    Set<int> set = new();

    public long RES = 0;

    //[Benchmark]
    //public void WBT_SplitMerge() {
    //    clear();
    //    for (int i = 0; i < N; i++) {
    //        var left_cnt = rnd.Next(0, N + 1);
    //        var (l, r) = filled.Split(left_cnt);
    //        if (l.Count != left_cnt) throw new Exception("bad");
    //        filled.Append(l);
    //        filled.Append(r);
    //        if (filled.Count != N) throw new Exception("bad");
    //    }
    //}

    //[Benchmark]
    //public void WBT_Set_Range() {
    //    clear();
    //    RES=filled.Range(next(filled.Count)).Where(x => x%4==1).Count();
    //    if (RES != 27676) throw new Exception("bad");
    //}

    [Benchmark]
    public void WBT_Set_ENUM() {
        clear();
        RES = filled.Where(x => x % 4 == 1).Count();
        if (RES < 5) throw new Exception("bad");
    }
    //[Benchmark]
    //public void WBT_Set_ENUM2() {
    //    clear();
    //    RES = filled.Range(0).Where(x => x % 4 == 1).Count();
    //    if (RES < 5) throw new Exception("bad");
    //}

    //[Benchmark]
    //public void WBT_Set_BS1() {
    //    clear();
    //    RES = 0;
    //    for (int i = 0; i < N; i++) {
    //        RES += filled.More(next());
    //    }
    //    if (RES != 45000300593) throw new Exception("bad");
    //}


    //[Benchmark]
    //public void WBT_Set_Insert() {
    //    clear();
    //    Set<int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(i);
    //        list.Add(next());
    //    }
    //}

    //[Benchmark]
    //public void WBT_Set_Insert() {
    //    clear();
    //    Set<int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(i);
    //        list.Add(next());
    //    }
    //}

    //[Benchmark]
    //public void SortedSet_Insert() {
    //    clear();
    //    SortedSet<int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(i);
    //        list.Add(next());
    //    }
    //}

    //[Benchmark]
    //public void WBT_Set_Add10() {
    //    clear();
    //    sbt.Clear();
    //    for (int i = 0; i < 10; i++) { sbt.Add(next()); }
    //}

    //[Benchmark]
    //public void SortedSet_Add10() {
    //    clear();
    //    set.Clear();
    //    for (int i = 0; i < 10; i++) { set.Add(next()); }
    //}

    //[Benchmark]
    //public void WBT_Set_Add1000() {
    //    clear();
    //    sbt.Clear();
    //    for (int i = 0; i < 1000; i++) { sbt.Add(next()); }
    //}

    //[Benchmark]
    //public void SortedSet_Add1000() {
    //    clear();
    //    set.Clear();
    //    for (int i = 0; i < 1000; i++) { set.Add(next()); }
    //}

    //[Benchmark]
    //public void WBT_Set_AddRemoveContainsRnd() {
    //    clear();
    //    Set<int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(next());
    //        list.Remove(next());
    //        list.Contains(next());
    //    }
    //}

    //[Benchmark]
    //public void SortedSet_AddRemoveContainsRnd() {
    //    clear();
    //    SortedSet<int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(next());
    //        list.Remove(next());
    //        list.Contains(next());
    //    }
    //}



    //[Benchmark]
    //public void WBT_Set_AddRemoveLast() {
    //    Set<int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(i);
    //    }
    //    for (int i = 0; i < N; i++) {
    //        list.Remove(i);
    //    }
    //}

    //[Benchmark]
    //public void SortedSet_AddRemoveLast() {
    //    SortedSet<int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(i);
    //    }
    //    for (int i = 0; i < N; i++) {
    //        list.Remove(i);
    //    }
    //}

    //[Benchmark]
    //public void WBT_Map_AddRemoveRnd() {
    //    clear();
    //    Map<int, int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(next(), next());
    //        list.Remove(next());
    //    }
    //}

    //[Benchmark]
    //public void SortedDictionary_AddRemoveRnd() {
    //    clear();
    //    SortedDictionary<int, int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.TryAdd(next(), next());
    //        list.Remove(next());
    //    }
    //}

    //[Benchmark]
    //public void Dictionary_AddRemoveRnd() {
    //    clear();
    //    Dictionary<int, int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.TryAdd(next(), next());
    //        list.Remove(next());
    //    }
    //}

    //[Benchmark]
    //public void WBT_MultiSet_AddRemoveAll() {
    //    clear();
    //    MultiSet<int> list = new();
    //    for (int i = 0; i < N; i++) {
    //        list.Add(rnd.Next(0, N / 4));
    //    }
    //    for (int i = 0; i < N; i++) {
    //        list.RemoveAllOf(rnd.Next(0, N / 4));
    //    }
    //}





}
