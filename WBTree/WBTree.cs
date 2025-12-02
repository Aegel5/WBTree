global using IM = System.Runtime.CompilerServices.MethodImplAttribute;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;


namespace AlgoQuora {
    public class _WBTree<T> : IEnumerable<T> {
        protected class Node {
            public int cnt = 1;
            public T val;
            public Node? left;
            public Node? right;
            public Node(T v) { val = v; }
        }
        protected Node? root;
        [IM(256)] protected bool is_nil(Node? t) { return t is null; }
        protected Node? nil { [IM(256)] get { return null; } }
        [IM(256)] protected int cnt_safe(Node t) { return t?.cnt ?? 0; }
        public int Count => cnt_safe(root);
        public bool IsEmpty => Count == 0;
        [IM(256)] protected int ActualIndex(Index index) => index.IsFromEnd ? Count - index.Value : index.Value;
        [IM(256)] protected Node get_at(Index index) => get_at(ActualIndex(index));
        [IM(256)] public void Clear() { root = nil; }
        [IM(256)] Node rotateR(Node t, Node l) { t.left = l.right; l.right = t; l.cnt = t.cnt; if (!is_nil(l.left)) t.cnt -= l.left.cnt; t.cnt--; return l; }
        [IM(256)] Node rotateL(Node t, Node r) { t.right = r.left; r.left = t; r.cnt = t.cnt; if (!is_nil(r.right)) t.cnt -= r.right.cnt; t.cnt--; return r; }
        protected const double ALPHA = 0.292;
        [IM(256)] protected bool too_big(int cnt, int total) => cnt > total * (1 - ALPHA);
        [IM(256)] protected bool too_small(int cnt, int total) => cnt < total * ALPHA - 1;
        //public int rototions_cnt = 0;
        [IM(256)] protected Node balanceR(Node t) { 
            //rototions_cnt++; 
            if (too_small(cnt_safe(t.right.right), t.cnt)) { t.right = rotateR(t.right, t.right.left); } return rotateL(t, t.right); 
        }
        [IM(256)] protected Node balanceL(Node t) { 
            //rototions_cnt++; 
            if (too_small(cnt_safe(t.left.left), t.cnt)) t.left = rotateL(t.left, t.left.right); return rotateR(t, t.left);
        }
        [IM(256)] protected bool balance_if_overflowR(ref Node t) { if (too_big(t.right.cnt, t.cnt)) { t = balanceR(t); return true; } return false; }
        [IM(256)] protected bool balance_if_overflowL(ref Node t) { if (too_big(t.left.cnt, t.cnt)) { t = balanceL(t); return true; } return false; }

        [IM(256)] protected void balance_if_overflowR_rec(ref Node t) {
            //call_cnt++;
            if (balance_if_overflowR(ref t)) {
                if (!is_nil(t.left.right))
                    balance_if_overflowR_rec(ref t.left);
                balance_if_overflowR(ref t);
            }
        }

        //public int call_cnt = 0;
        [IM(256)] protected void balance_if_overflowL_rec(ref Node t) {
            //call_cnt++;
            if (balance_if_overflowL(ref t)) {
                if (!is_nil(t.right.left))
                    balance_if_overflowL_rec(ref t.right);
                balance_if_overflowL(ref t);
            }
        }

        // потестить на net10
        //IEnumerable<T> left_to_right(Node t) {
        //    if (!is_nil(t.left))
        //        foreach (var item in left_to_right(t.left))
        //            yield return item;
        //    yield return t.val;
        //    if (!is_nil(t.right))
        //        foreach (var item in left_to_right(t.right))
        //            yield return item;
        //}
        // оптимизированная версия Skip + Take - перечислить указанный диапазон
        // удалять во время перечесления нельзя
        //public IEnumerable<T> Range(int l = 0, int r = int.MaxValue) {
        //    if (is_nil(root)) return Enumerable.Empty<T>();
        //    IEnumerable<T> func(Node t, int pos) {
        //        var cnt_left = cnt_safe(t.left);
        //        if (cnt_left > 0 && cnt_left > pos) {
        //            foreach (var item in func(t.left, pos)) yield return item;
        //        }
        //        if (cnt_left >= pos) 
        //            yield return t.val;
        //        if (!is_nil(t.right)) {
        //            pos -= cnt_left + 1;
        //            foreach (var item in func(t.right, pos)) yield return item;
        //        }
        //    }
        //    var res = func(root, l);
        //    return r == int.MaxValue ? res : res.Take(r - l + 1);
        //}

        [InlineArray(60)] struct MyInlineArray { private Node _element0; }
        public IEnumerable<T> Range(int pos = 0) {

            if (is_nil(root)) yield break;

            var stack = new MyInlineArray();

            var t = root;
            int stack_i = 0;

            // Делаем log проверок.

            while (true) {
                var cnt_left = cnt_safe(t.left);
                if (cnt_left > 0 && cnt_left > pos) {
                    stack[stack_i++] = t;
                    t = t.left;
                    continue;
                }
                if (cnt_left >= pos) { yield return t.val; }
                pos -= cnt_left + 1;

                if (is_nil(t.right)) break;
                t = t.right;
            }

            // все что осталось на стеке обрабатываем без проверок.

            while (true) {

                if (stack_i == 0) yield break;
                t = stack[--stack_i];
                //t = Unsafe.Add(ref stack[0], --stack_i);
                yield return t.val;

                if (!is_nil(t.right)) {
                    t = t.right;
                    while (true) {
                        if (!is_nil(t.left)) {

                            //Unsafe.Add(ref stack[0], stack_i++) = t;
                            stack[stack_i++] = t;

                            t = t.left;
                        } else {
                            yield return t.val;
                            if (is_nil(t.right)) break;
                            t = t.right;
                        }
                    }
                }
            }
        }


        public IEnumerable<T> Range(int l, int r) => Range(l).Take(r - l + 1);
        public IEnumerator<T> GetEnumerator() => Range(0).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        protected Node get_at(int pos) {
            Debug.Assert(pos >= 0 && pos < Count);
            var t = root;
            while (true) {
                var cnt_left = cnt_safe(t.left);
                if (pos == cnt_left) return t;
                if (pos < cnt_left)
                    t = t.left;
                else {
                    t = t.right;
                    pos -= cnt_left + 1;
                }
            }
        }
        protected Node extract_leftmost(ref Node t) {
            Node min;
            int cnt;
            if (is_nil(t.left.left)) {
                min = t.left;
                t.left = t.left.right;
                cnt = cnt_safe(t.left);
            } else {
                min = extract_leftmost(ref t.left);
                cnt = t.left.cnt;
            }
            t.cnt--;
            if (too_small(cnt, t.cnt)) t = balanceR(t);
            return min;
        }

        // оптимизированная версия для сбалансированных между собой узлов.
        protected Node merge_balanced(Node left, Node right) {
            if (left == null) return right;
            if (right == null) return left;

            if (right.left == null) {
                right.left = left;
                right.cnt += left.cnt;
                balance_if_overflowL(ref right);
                return right;
            }

            var min = extract_leftmost(ref right);
            min.right = right;
            min.left = left;
            min.cnt = left.cnt + right.cnt + 1;
            balance_if_overflowL(ref min);
            return min;
        }


        protected Node merge_any(Node left, Node right) {
            if (left == null) return right;
            if (right == null) return left;

            if (right.left == null) {
                right.left = left;
                right.cnt += left.cnt;
                balance_if_overflowL_rec(ref right);
                return right;
            }

            var min = extract_leftmost(ref right);
            min.right = right;
            min.left = left;
            min.cnt = left.cnt + right.cnt + 1;
            if (left.cnt > right.cnt) balance_if_overflowL_rec(ref min);
            else balance_if_overflowR_rec(ref min);
            return min;
        }

        protected (Node, Node) split(int i) {
            void upd(Node t) { t.cnt = cnt_safe(t.left) + cnt_safe(t.right) + 1; }
            void func(Node t, ref Node left, ref Node right, int key) {
                var cnt_left = cnt_safe(t.left);
                if (key <= cnt_left) {

                    if (cnt_left == 0) {
                        left = nil;
                    } else {
                        func(t.left, ref left, ref t.left, key);
                    }
                    right = t;

                    upd(t);
                    if (!is_nil(t.right))
                        balance_if_overflowR_rec(ref right);
                } else {
                    if (is_nil(t.right)) {
                        right = nil;
                    } else {
                        func(t.right, ref t.right, ref right, key - cnt_left - 1);
                    }
                    left = t;

                    upd(t);
                    if (!is_nil(t.left))
                        balance_if_overflowL_rec(ref left);
                }

            }
            Node left = nil;
            Node right = nil;
            func(root, ref left, ref right, i);
            root = nil;
            return (left, right);
        }
        public T RemoveAt(Index index) {
            var i = ActualIndex(index);
            Debug.Assert(i >= 0 && i < Count);
            int key = i + 1;
            T res;
            void func(ref Node t) {
                var cur = t.cnt - cnt_safe(t.right);
                if (key == cur) {
                    res = t.val;
                    t = merge_balanced(t.left, t.right);
                    return;
                }
                if (key < cur) {
                    func(ref t.left);
                    t.cnt--;
                    if (t.right != null) balance_if_overflowR(ref t);
                } else {
                    key -= cur;
                    func(ref t.right);
                    t.cnt--;
                    if (t.left != null) balance_if_overflowL(ref t);
                }
            }
            func(ref root);
            return res;
        }
        public struct BSResult_Index {
            public int index;
            public bool Ok;
            [IM(256)] public static implicit operator int(BSResult_Index v) => v.index;
            public override string ToString() => index.ToString();
            public BSResult_Index(int v, bool ok) { index = v; Ok = ok; }
        }

        public struct BSResult<T> {
            public T Val;
            public bool Ok;
            [IM(256)] public static implicit operator T(BSResult<T> v) => v.Val;
            public override string ToString() => Val.ToString();
            public BSResult(T v, bool ok) { Val = v; Ok = ok; }
        }
        int _First_Index(Func<T, bool> check, int l = 0) {
            var t = root;
            int res = 0; 
            while (!is_nil(t)) {
                var l_cnt = cnt_safe(t.left);
                if (l <= l_cnt // имеем ли право проверять текущий элемент?
                    && check(t.val)) {  
                    t = t.left; 
                } else {
                    res += l_cnt + 1;
                    l -= res;
                    t = t.right;
                }
            }
            return res;
        }

        int _First(Func<T, bool> check, out T last_res, int l = 0) {
            var t = root;
            int res = 0;
            last_res = default;
            while (!is_nil(t)) {
                var l_cnt = cnt_safe(t.left);
                if (l <= l_cnt // имеем ли право проверять текущий элемент?
                    && check(t.val)) {
                    last_res = t.val;
                    t = t.left;
                } else {
                    res += l_cnt + 1;
                    l -= res;
                    t = t.right;
                }
            }
            return res;
        }

        // Тоже самое, что и _First только идем вправо, а не влево.
        int _Last(Func<T, bool> check, out T last_res, int l = 0) {
            var t = root;
            int res = 0;
            last_res = default;
            while (!is_nil(t)) {
                var l_cnt = cnt_safe(t.left);
                if (l <= l_cnt // имеем ли право проверять текущий элемент?
                    && check(t.val)) {
                    last_res = t.val;
                    res += l_cnt + 1;
                    l -= res;
                    t = t.right;
                } else {
                    t = t.left;
                }
            }
            return res-1;
        }

        [IM(256)] public BSResult_Index BinarySearch_Last_Index(Func<T, bool> check, int l = 0, int r = int.MaxValue) {
            int i = _First_Index(x => !check(x), l) - 1;
            if (i > r) i = Math.Min(r, Count - 1);
            return new(i, i >= l);
        }
        [IM(256)] public BSResult_Index BinarySearch_First_Index(Func<T, bool> check, int l = 0, int r = int.MaxValue) {
            int i = _First_Index(check, l);
            return new(i, i <= Math.Min(r, Count - 1));
        }

        // TODO - добавить индекс так как все равно находим его?
        [IM(256)]
        public BSResult<T> BinarySearch_First(Func<T, bool> check, int l = 0, int r = int.MaxValue) {
            int i = _First(check, out var res, l);
            return new(res, i <= Math.Min(r, Count - 1));
        }
        [IM(256)]
        public BSResult<T> BinarySearch_Last(Func<T, bool> check, int l = 0, int r = int.MaxValue) {
            int i = _Last(check, out var res, l);
            if (i > r) i = Math.Min(r, Count - 1);
            return new(res, i >= l);
        }
    }
}