using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/**
 * Surrogate class for Observable Collection. Needed to serialize. 
 * 
 * */

namespace SSLog
{
    sealed class ObservableCollectionSerializationSurrogate<T> : ISerializationSurrogate
    {
        private const string _itemsKey = "items";

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Debug.Assert(obj is ObservableCollection<T>);
            ObservableCollection<T> observableCollection = obj as ObservableCollection<T>;
            T[] items = new T[observableCollection.Count];
            observableCollection.CopyTo(items, 0);
            info.AddValue(_itemsKey, items);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            T[] items = info.GetValue(_itemsKey, typeof(T[])) as T[];
            return new ObservableCollection<T>(new List<T>(items));
        }
    }
}
