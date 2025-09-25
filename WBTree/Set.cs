using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoQuora {
    public class Set<T> : _WBTreeSorted<T> {
        public Set(IEnumerable<T> col) { foreach (var item in col) { Add(item); } }
        public Set() { }
        public T this[Index key] {
            get => get_at(key).val;
            //set => get_at(key).node = value;
        }
        public bool Contains(T value) => _Contains(value);
        public int CountOf(T node) => Contains(node) ? 1 : 0;
        public bool Add(T node) => _Add(node, skip_if_equal: true).added;
        public bool Remove(T node) => _Remove(node) > 0;
        public BSResult More(T val, int l = 0, int r = int.MaxValue) => BinarySearch_First(x => Compare(x, val) > 0, l, r);
        public BSResult MoreEq(T val, int l = 0, int r = int.MaxValue) => BinarySearch_First(x => Compare(x, val) >= 0, l, r);
        public BSResult Less(T val, int l = 0, int r = int.MaxValue) => BinarySearch_Last(x => Compare(x, val) < 0, l, r);
        public BSResult LessEq(T val, int l = 0, int r = int.MaxValue) => BinarySearch_Last(x => Compare(x, val) <= 0, l, r);
        public int IndexOf(T val) => MoreEq(val);

    }

    public class MultiSet<T> : Set<T> {
        public MultiSet(IEnumerable<T> col) { foreach (var item in col) { Add(item); } }
        public MultiSet() { }
        new public bool Add(T val) => _Add(val, skip_if_equal: false).added;
        new public int CountOf(T val) => More(val) - MoreEq(val);
        public int RemoveAllOf(T val) => _Remove(val, all: true);
    }
}
