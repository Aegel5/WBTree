using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using IM = System.Runtime.CompilerServices.MethodImplAttribute;

namespace AlgoQuora {
    public class _WBTree<T> : IEnumerable<T> {
        protected class Node {
            public int cnt = 1;
            public T val;
            public Node? left;
            public Node? right;
            public Node(T v) { val = v; }
        }
        protected IComparer<T> comparer = Comparer<T>.Default;
        protected Node? root;
        [IM(256)] protected bool is_nil([NotNullWhen(false)] Node? t) { return t == null; }
        [IM(256)] protected int cnt_safe(Node t) { return t?.cnt ?? 0; }
        public int Count => cnt_safe(root);
        public bool IsEmpty => Count == 0;
        [IM(256)] protected int Compare(T v1, T v2) => comparer.Compare(v1, v2);
        [IM(256)] protected int ActualIndex(Index index) => index.IsFromEnd ? Count - index.Value : index.Value;
        [IM(256)] protected Node get_at(Index index) => get_at(ActualIndex(index));
        [IM(256)] public void Clear() { root = null; }
        [IM(256)] Node rotateR(Node t, Node l) { t.left = l.right; l.right = t; l.cnt = t.cnt; if (!is_nil(l.left)) t.cnt -= l.left.cnt; t.cnt--; return l; }
        [IM(256)] Node rotateL(Node t, Node r) { t.right = r.left; r.left = t; r.cnt = t.cnt; if (!is_nil(r.right)) t.cnt -= r.right.cnt; t.cnt--; return r; }
        protected const double ALPHA = 0.292f;
        [IM(256)] protected bool too_big(int cnt, int total) => cnt > total * (1 - ALPHA);
        [IM(256)] protected bool too_small(int cnt, int total) => cnt < total * ALPHA - 1;
        [IM(256)] protected Node balanceR(Node t) { if (too_small(cnt_safe(t.right.right), t.cnt)) { t.right = rotateR(t.right, t.right.left); } return rotateL(t, t.right); }
        [IM(256)] protected Node balanceL(Node t) { if (too_small(cnt_safe(t.left.left), t.cnt)) t.left = rotateL(t.left, t.left.right); return rotateR(t, t.left); }
        [IM(256)] bool balance_if_overflowR(ref Node t) { if (too_big(t.right.cnt, t.cnt)) { t = balanceR(t); return true; } return false; }
        [IM(256)] bool balance_if_overflowL(ref Node t) { if (too_big(t.left.cnt, t.cnt)) { t = balanceL(t); return true; } return false; }
        [IM(256)] void balance_if_overflowR_rec(ref Node t) {
            if (balance_if_overflowR(ref t)) {
                if (t.left.right != null)
                    balance_if_overflowR_rec(ref t.left);
                balance_if_overflowR(ref t);
            }
        }
        [IM(256)] void balance_if_overflowL_rec(ref Node t) {
            if (balance_if_overflowL(ref t)) {
                if (t.right.left != null)
                    balance_if_overflowL_rec(ref t.right);
                balance_if_overflowL(ref t);
            }
        }
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
        IEnumerable<T> left_to_right(Node t) {
            if (is_nil(t)) yield break;
            foreach (var item in left_to_right(t.left)) yield return item;
            yield return t.val;
            foreach (var item in left_to_right(t.right)) yield return item;
        }
        public IEnumerator<T> GetEnumerator() => left_to_right(root).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => left_to_right(root).GetEnumerator();
        protected bool _Contains(T node) {
            var t = root;
            while (!is_nil(t)) {
                int cmp = Compare(node, t.val);
                if (cmp == 0) return true;
                t = cmp < 0 ? t.left : t.right;
            }
            return false;
        }
        protected int _Remove(T node, bool all = false) {
            int func(ref Node t) {
                if (is_nil(t)) return 0;
                int cmp = Compare(node, t.val);
                if (cmp == 0) {
                    int cnt = 1;
                    if (all) { 
                        cnt += func(ref t.left); 
                        cnt += func(ref t.right);
                    }
                    t = merge(t.left, t.right);
                    return cnt;
                }
                int count;
                if(cmp < 0) {
                    count = func(ref t.left);
                    t.cnt -= count;
                    if (t.right != null) balance_if_overflowR_rec(ref t);
                } else {
                    count = func(ref t.right);
                    t.cnt -= count;
                    if (t.left != null) balance_if_overflowL_rec(ref t);
                }
                return count;
            }
            return func(ref root);
        }
        [IM(256)] protected Node extract_min(ref Node t) {
            Node min;
            if (t.left.left == null) {
                min = t.left;
                t.left = t.left.right;
                t.cnt--;
                if (too_small(cnt_safe(t.left), t.cnt)) t = balanceR(t);
            } else {
                min = extract_min(ref t.left);
                t.cnt--;
                if(too_small(t.left.cnt, t.cnt)) t = balanceR(t);
            }
            return min;
        }
        [IM(256)] protected Node merge(Node left, Node right) {
            if (left == null) return right;
            if (right == null) return left;
            Node min;
            if (right.left == null) min = right;
            else {
                min = extract_min(ref right);
                min.right = right;
            }
            min.left = left;
            var r_cnt = cnt_safe(min.right);
            min.cnt = left.cnt + r_cnt + 1;
            if (left.cnt > r_cnt) {
                balance_if_overflowL_rec(ref min);
            } else {
                balance_if_overflowR_rec(ref min);
            }
            return min;
        }
        public void RemoveAt(Index index) {
            var i = ActualIndex(index);
            Debug.Assert(i >= 0 && i < Count);
            int key = i+1; 
            void func(ref Node t) {
                var cur = t.cnt - cnt_safe(t.right);
                if (key == cur) {
                    t = merge(t.left, t.right);
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
        }
        public struct BSResult {
            public int Index { get; init; }
            public bool Ok { get; init; }
            [IM(256)] public static implicit operator int(BSResult value) => value.Index;
            [IM(256)] public static implicit operator long(BSResult value) => value.Index;
            public BSResult(int i, bool ok) { Index = i; Ok = ok; }
        }
        int _First(Func<T, bool> check, int l = 0) {
            var t = root;
            int offset = 0;
            int res = Count; // последний верный ответ.
            while (!is_nil(t)) {
                var l_cnt = cnt_safe(t.left);
                if (l <= l_cnt// имеем ли право проверять текущий элемент?
                    && check(t.val)) {  // обычный поиск в дереве. используем технику "последнего верного ответа"
                    res = l_cnt + offset; // верный ответ
                    t = t.left; // идем влево, так как справа 100% уже понятно.
                } else {
                    // текущий элемент проверять запрещено. просто идем вправо.
                    offset += l_cnt + 1;
                    l -= offset;
                    t = t.right;
                }
            }
            return res;
        }
        [IM(256)] public BSResult BinarySearch_Last(Func<T, bool> check, int l = 0, int r = int.MaxValue) {
            int i = _First(x => !check(x), l) - 1;
            if (i > r) i = r;
            return new(i, i >= l);
        }
        [IM(256)] public BSResult BinarySearch_First(Func<T, bool> check, int l = 0, int r = int.MaxValue) {
            int i = _First(check, l);
            return new(i, i <= r);
        }
        protected (bool added, Node node) _Add(T val, bool skip_if_equal = false) {
            if (is_nil(root)) { return (true, root = new Node(val)); }
            bool added = false; Node res = null;
            void func(ref Node t) {
                int cmp = Compare(val, t.val);
                if (cmp <= 0) {
                    if (cmp == 0 && skip_if_equal) { res = t; return; }
                    if (t.left == null) { added = true; t.left = res = new Node(val); t.cnt++; return; }
                    func(ref t.left); if (!added) return; 
                    t.cnt++;
                    balance_if_overflowL(ref t);
                } else {
                    if (t.right == null) { added = true; t.right = res = new Node(val); t.cnt++; return; }
                    func(ref t.right); if (!added) return; 
                    t.cnt++;
                    balance_if_overflowR(ref t);
                }
            }
            func(ref root); return (added, res);
        }
    }

}