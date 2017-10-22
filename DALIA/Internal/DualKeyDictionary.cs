using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dalia.Internal
{
    internal class DualKeyDictionary<TKey1, TKey2, TValue> : ICollection, IEnumerable<TValue>
    {

        private List<TKey1> keys1 = new List<TKey1>();
        private List<TKey2> keys2 = new List<TKey2>();
        private List<TValue> values = new List<TValue>();
        private Dictionary<TKey1, int> keysDic1;
        private Dictionary<TKey2, int> keysDic2;
        private IEqualityComparer<TKey1> comparer1;
        private IEqualityComparer<TKey2> comparer2;

        public DualKeyDictionary()
        {
            keysDic1 = new Dictionary<TKey1, int>();
            keysDic2 = new Dictionary<TKey2, int>();
        }

        public DualKeyDictionary(IEqualityComparer<TKey1> comparer1, IEqualityComparer<TKey2> comparer2)
        {
            if (comparer1 != null)
            {
                keysDic1 = new Dictionary<TKey1, int>(comparer1);
                this.comparer1 = comparer1;
            }
            else
            {
                keysDic1 = new Dictionary<TKey1, int>();
            }

            if (comparer2 != null)
            {
                keysDic2 = new Dictionary<TKey2, int>(comparer2);
                this.comparer2 = comparer2;
            }
            else
            {
                keysDic2 = new Dictionary<TKey2, int>();
            }
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            if (keysDic1.ContainsKey(key1))
            {
                throw (new ArgumentException("An item with the same key has already been added."));
            }
            if (keysDic2.ContainsKey(key2))
            {
                throw (new ArgumentException("An item with the same key has already been added."));
            }

            keys1.Add(key1);
            keys2.Add(key2);
            values.Add(value);
            keysDic1.Add(key1, keys1.Count - 1);
            keysDic2.Add(key2, keys2.Count - 1);

        }

        public void Clear()
        {
            keysDic1.Clear();
            keysDic2.Clear();
            keys1.Clear();
            keys2.Clear();
            values.Clear();
        }

        public int Count
        {
            get
            {
                return values.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }


        public bool ContainsKey(TKey1 key)
        {
            return keysDic1.ContainsKey(key);
        }

        public bool ContainsKey2(TKey2 key)
        {
            return keysDic2.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return values.Contains(value);
        }

        public TValue this[TKey1 key]
        {
            get
            {
                return values[keysDic1[key]];
            }
            set
            {
                values[keysDic1[key]] = value;
            }
        }

        public TValue this[TKey2 key]
        {
            get
            {
                return values[keysDic2[key]];
            }
            set
            {
                values[keysDic2[key]] = value;
            }
        }

        public TValue ItemAt(int index)
        {
            return values[index];
        }

        public void SetItemAt(int index, TValue value)
        {
            values[index] = value;
        }

        public ICollection<TKey1> Keys1
        {
            get
            {
                return keys1;
            }
        }

        public TKey1 Key1(TKey2 key2)
        {
            int index = keysDic2[key2];
            return keys1[index];
        }

        public TKey1 Key1At(int index)
        {
            return keys1[index];
        }

        public ICollection<TKey2> Keys2
        {
            get
            {
                return keys2;
            }
        }

        public TKey2 Key2(TKey1 key1)
        {
            int index = keysDic1[key1];
            return keys2[index];
        }

        public TKey2 Key2At(int index)
        {
            return keys2[index];
        }

        public int IndexOf1(TKey1 key)
        {
            if (comparer1 != null)
            {
                int i = 0;
                foreach (TKey1 k in keys1)
                {
                    if (comparer1.Equals(k, key))
                    {
                        return i;
                    }
                    i++;
                }
                return -1;
            }
            else
            {
                return keys1.IndexOf(key);
            }
        }

        public int IndexOf2(TKey2 key)
        {
            if (comparer1 != null)
            {
                int i = 0;
                foreach (TKey2 k in keys2)
                {
                    if (comparer2.Equals(k, key))
                    {
                        return i;
                    }
                    i++;
                }
                return -1;
            }
            else
            {
                return keys2.IndexOf(key);
            }
        }

        public bool Remove(TKey1 key)
        {
            if (keysDic1.ContainsKey(key))
            {
                int i = keysDic1[key];
                keysDic2.Remove(keys2[i]);
                keysDic1.Remove(key);
                keys1.RemoveAt(i);
                keys2.RemoveAt(i);
                values.RemoveAt(i);
                return true;
            }
            return false;
        }

        public bool Remove(TKey2 key)
        {
            if (keysDic2.ContainsKey(key))
            {
                int i = keysDic2[key];
                keysDic1.Remove(keys1[i]);
                keysDic2.Remove(key);
                keys1.RemoveAt(i);
                keys2.RemoveAt(i);
                values.RemoveAt(i);
                return true;
            }
            return false;
        }

        public bool RemoveAt(int index)
        {
            if (index >= 0 && index < keys1.Count)
            {
                keysDic1.Remove(keys1[index]);
                keysDic2.Remove(keys2[index]);
                keys1.RemoveAt(index);
                keys2.RemoveAt(index);
                values.RemoveAt(index);
                return true;
            }
            return false;
        }

        public bool TryGetValue(TKey1 key, out TValue value)
        {
            var index = -1;
            if (keysDic1.TryGetValue(key, out index))
            {
                value = values[index];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public bool TryGetValue(TKey2 key, out TValue value)
        {
            var index = -1;
            if (keysDic2.TryGetValue(key, out index))
            {
                value = values[index];
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return values;
            }
        }

        public TValue ValueAt(int index)
        {
            return values[index];
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return values.GetEnumerator();
        }


        public void CopyTo(System.Array array, int index)
        {
            ((ICollection)values).CopyTo(array, index);
        }



        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)values).IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)values).SyncRoot;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }


        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return values.GetEnumerator();
        }
    }
}
