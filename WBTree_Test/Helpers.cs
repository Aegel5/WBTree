using AlgoQuora;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSortedList;
class SortedListChecked<T> : MultiSet<T> where T : new() {
    public double tot_depth = 0;
    public int cnt_depth = 0;
    public int max_depth = 0;
    public double avr_depth = 0;
    public bool AddUnique(T val) => _Add(val, true).added;
    public void extract_min_test() {
        extract_min(ref root);
    }

    public void Append(SortedListChecked<T> t) {
        root = merge(root, t.root);
        t.root = null;
    }

    SortedListChecked(Node _root) { root = _root; }
    public SortedListChecked() { }

    public (SortedListChecked<T>, SortedListChecked<T>) Split(int index) {
        var (l, r) = split(index);
        return (new(l), new(r));
    }

    public void MaxLen() {
        void func(Node t, int d = 0) {
            if (is_nil(t)) {
                max_depth = Math.Max(max_depth, d - 1);
                return;
            }
            func(t.left, d + 1);
            func(t.right, d + 1);
        }
        func(root);
    }

    public void SelfCheckRules(bool check_violate = true) {
        void func(Node t, int d = 0) {
            if (is_nil(t)) return;
            func(t.left, d + 1);
            func(t.right, d + 1);
            if (t.cnt != cnt_safe(t.left) + cnt_safe(t.right) + 1) {
                throw new Exception("bad cnt");
            }
            void check(Node ch) { if (too_big(cnt_safe(ch), t.cnt)) { throw new Exception("violate"); } }
            if (check_violate) { check(t.left); check(t.right); }
            if (!is_nil(t.left)) {
                if (Compare(t.left.val, t.val) > 0) {
                    throw new Exception("bad order");
                }
            }
            if (!is_nil(t.right)) {
                if (Compare(t.right.val, t.val) < 0) {
                    throw new Exception("bad order");
                }
            }
            if (is_nil(t.left) && is_nil(t.right)) {
                tot_depth += d;
                cnt_depth++;
                avr_depth = tot_depth / cnt_depth;
                max_depth = Math.Max(max_depth, d);
            }
        }
        func(root);
    }
}

class SortedList_Tester<T> where T : new() {
    static void Assert(bool v) { if (!v) throw new Exception("bad"); }
    public SortedListChecked<T> lst = new();
    List<T> checker = new();

    int Compare(T v1, T v2) => Comparer<T>.Default.Compare(v1, v2);

    public int Count => checker.Count;

    public void Add(T v, bool unique = false) {
        if (unique) lst.AddUnique(v);
        else lst.Add(v);
        for (int i = 0; i < checker.Count; i++) {
            int cmp = Compare(v, checker[i]);
            if (unique && cmp == 0) return;
            if (cmp < 0) {
                checker.Insert(i, v);
                return;
            }
        }
        checker.Add(v);
    }
    public void Remove(T v) {
        lst.Remove(v);
        for (int i = 0; i < checker.Count; i++) {
            int cmp = Compare(v, checker[i]);
            if (cmp == 0) {
                checker.RemoveAt(i);
                return;
            }
        }
    }
    public int RemoveAllOf(T v) {
        var cnt = lst.RemoveAllOf(v);
        var cnt2 = checker.RemoveAll(x => Compare(x, v) == 0);
        if (cnt != cnt2)
            throw new Exception("bad");
        return cnt;
    }
    public void Get(int i) {
        if (Compare(lst[i], checker[i]) != 0)
            throw new Exception("bad");

    }
    public void RemoveAt(int i) {
        lst.RemoveAt(i);
        checker.RemoveAt(i);
    }
    public int CountOf(T v) {
        var cnt = lst.CountOf(v);
        if (cnt > 1) {
            int k = 0;
        }
        var cnt2 = checker.Count(x => Compare(x, v) == 0);
        if (cnt != cnt2)
            throw new Exception("bad");
        return cnt;
    }
    public void Contains(T v) {
        if (lst.Contains(v) != checker.Contains(v)) throw new Exception("bad");
    }
    public void More(T v) {
        var i1 = lst.More(v);
        int i2 = 0;
        for (i2 = 0; i2 < checker.Count; i2++) {
            if (Compare(checker[i2], v)>0) break;
        }
        Assert(i1==i2);
        Assert(i1.Ok == (i1.Index >= 0 && i1.Index < checker.Count));
    }
    public void Check() {
        lst.SelfCheckRules();
        if (lst.Count != checker.Count) throw new Exception("bad count");
        if (!lst.SequenceEqual(checker)) throw new Exception("not equal");
    }


}
