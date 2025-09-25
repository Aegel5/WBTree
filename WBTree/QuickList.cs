using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoQuora {
    public class QuickList<T> : _WBTree<T> {
        public QuickList(IEnumerable<T> col) { foreach (var item in col) { Add(item); } }
        public QuickList() { }
        public ref T this[Index key] => ref get_at(key).val;
        public void InsertAt(Index index, T val) {
            int i = ActualIndex(index);
            Debug.Assert(i >= 0);
            if (is_nil(root)) { root = new Node(val); return; }
            void func(ref Node t) {
                var cnt_left = cnt_safe(t.left);
                if (i <= cnt_left) {
                    if (t.left == null) { t.left = new Node(val); t.cnt++; return; }
                    func(ref t.left);
                    t.cnt++;
                    balance_if_overflowL(ref t);
                } else {
                    if (t.right == null) { t.right = new Node(val); t.cnt++; return; }
                    i -= cnt_left + 1;
                    func(ref t.right);
                    t.cnt++;
                    balance_if_overflowR(ref t);
                }
            }
            func(ref root);
        }
        public void Add(T val) => InsertAt(int.MaxValue, val);
    }
}
