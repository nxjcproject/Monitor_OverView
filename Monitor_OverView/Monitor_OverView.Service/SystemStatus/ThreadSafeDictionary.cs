using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace Monitor_OverView.Service.SystemStatus
{
    public class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICloneable
    {
        public Dictionary<TKey, TValue> Clone()
        {
            BinaryFormatter Formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            MemoryStream stream = new MemoryStream();
            Formatter.Serialize(stream, this);
            stream.Position = 0;
            object clonedObj = Formatter.Deserialize(stream);
            stream.Close();
            return (Dictionary<TKey, TValue>)clonedObj;
        }
        public bool ContainsKey(TKey myKey)
        {
            return this.ContainsKey(myKey);
        }
        public void Add(TKey myKey, TValue myValue)
        {
            this.Add(myKey, myValue);
        }
    }
}
