using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IM = System.Runtime.CompilerServices.MethodImplAttribute;

namespace AlgoQuora {
    public class _WBTreeSorted<T>:_WBTree<T> {
        protected IComparer<T> comparer = Comparer<T>.Default;
        [IM(256)] protected int Compare(T v1, T v2) => comparer.Compare(v1, v2);
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
                if (cmp < 0) {
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
