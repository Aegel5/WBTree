using AlgoQuora.details;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AlgoQuora {

    namespace details {
        public struct MapRecord<TKey, TValue> {
            public TKey Key;
            public TValue Value;
        }
    }

    public class Map<TKey, TValue> : _WBTreeSorted<MapRecord<TKey,TValue>> {
        protected IComparer<TKey> comparerKey;
        [MethodImpl(256)] protected int CompareKey(TKey v1, TKey v2) => comparerKey.Compare(v1, v2);
        public Map() {
            comparerKey = Comparer<TKey>.Default;
            comparer = Comparer<MapRecord<TKey,TValue>>.Create((x, y) => comparerKey.Compare(x.Key, y.Key));
        }
        [MethodImpl(256)] protected MapRecord<TKey, TValue> rec(TKey key, TValue val = default) 
            => new MapRecord<TKey, TValue> { Key = key, Value = val };
        public bool Add(TKey key, TValue value) => _Add(rec(key,value), skip_if_equal:true).added;
        public ref TValue this[TKey key] => ref _Add(rec(key), skip_if_equal: true).node.val.Value;
        public bool Contains(TKey key) => _Contains(rec(key));
        public int CountOf(TKey key) => Contains(key) ? 1 : 0;
        public bool Remove(TKey key) => _Remove(rec(key));
        public MapRecord<TKey, TValue> ByIndex(Index i) => get_at(i).val;
        public ref TValue ByIndexValue(Index i) => ref get_at(i).val.Value;
        public BSResult_Index More_Index(TKey val, int l = 0, int r = int.MaxValue) => BinarySearch_First_Index(x => CompareKey(x.Key, val) > 0, l, r);
        public BSResult_Index MoreEq_Index(TKey val, int l = 0, int r = int.MaxValue) => BinarySearch_First_Index(x => CompareKey(x.Key, val) >= 0, l, r);
        public BSResult_Index Less_Index(TKey val, int l = 0, int r = int.MaxValue) => BinarySearch_Last_Index(x => CompareKey(x.Key, val) < 0, l, r);
        public BSResult_Index LessEq_Index(TKey val, int l = 0, int r = int.MaxValue) => BinarySearch_Last_Index(x => CompareKey(x.Key, val) <= 0, l, r);
        public bool TryGetValue(TKey key, out TValue res){
            var node = _Find(rec(key)); 
            if (is_nil(node)) { res = default; return false; }
            res = node.val.Value; 
            return true;
        }
    }
    public class MultiMap<TKey, TValue> : Map<TKey, TValue> {
        new public bool Add(TKey key, TValue value)  => _Add(rec(key,value), skip_if_equal: false).added;
        public int RemoveAllOf(TKey key) => _RemoveAll(rec(key));
        new public int CountOf(TKey key) => More_Index(key) - MoreEq_Index(key);
    }
}
