using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlgoQuora {
    public class Set<T> : _WBTreeSorted<T> {
        public Set(IEnumerable<T> col) { foreach (var item in col) { Add(item); } }
        public Set() { }
        public T this[Index key] => get_at(key).val;
        public bool Contains(T value) => _Contains(value);
        public int CountOf(T node) => Contains(node) ? 1 : 0;
        public bool Add(T node) => _Add(node, skip_if_equal: true).added;
        [IM(256)] public bool Remove(T node) => _Remove(node);

        public BSResult_Index More_Index(T val, int l = 0, int r = int.MaxValue) => BinarySearch_First_Index(x => Compare(x, val) > 0, l, r);
        public BSResult_Index MoreEq_Index(T val, int l = 0, int r = int.MaxValue) => BinarySearch_First_Index(x => Compare(x, val) >= 0, l, r);
        public BSResult_Index Less_Index(T val, int l = 0, int r = int.MaxValue) => BinarySearch_Last_Index(x => Compare(x, val) < 0, l, r);
        public BSResult_Index LessEq_Index(T val, int l = 0, int r = int.MaxValue) => BinarySearch_Last_Index(x => Compare(x, val) <= 0, l, r);

        public BSResult<T> More(T val, int l = 0, int r = int.MaxValue) => BinarySearch_First(x => Compare(x, val) > 0, l, r);
        public BSResult<T> MoreEq(T val, int l = 0, int r = int.MaxValue) => BinarySearch_First(x => Compare(x, val) >= 0, l, r);

        // этого пока нет
        //public BSResult<T> Less(T val, int l = 0, int r = int.MaxValue) => BinarySearch_Last_Index(x => Compare(x, val) < 0, l, r);
        //public BSResult<T> LessEq(T val, int l = 0, int r = int.MaxValue) => BinarySearch_Last_Index(x => Compare(x, val) <= 0, l, r);
        public int IndexOf(T val) => MoreEq_Index(val);

    }

    public class MultiSet<T> : Set<T> {
        public MultiSet(IEnumerable<T> col) { foreach (var item in col) { Add(item); } }
        public MultiSet() { }
        new public bool Add(T val) => _Add(val, skip_if_equal: false).added;
        new public int CountOf(T val) => More_Index(val) - MoreEq_Index(val);
        public int RemoveAllOf(T val) => _RemoveAll(val);
    }
}
