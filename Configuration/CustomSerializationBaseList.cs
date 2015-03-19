using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WordSequence.Configuration
{
    public abstract class CustomSerializationBaseList<T> : IList<T>, IXmlSerializable
    {
        protected CustomSerializationBaseList()
        {
            BackingList = new List<T>();
        }

        protected List<T> BackingList;

        public IEnumerator<T> GetEnumerator()
        {
            return BackingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            BackingList.Add(item);
        }

        public void Clear()
        {
            BackingList.Clear();
        }

        public bool Contains(T item)
        {
            return BackingList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            BackingList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return BackingList.Remove(item);
        }

        public int Count
        {
            get { return BackingList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(T item)
        {
            return BackingList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            BackingList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            BackingList.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return BackingList[index]; }
            set { BackingList[index] = value; }
        }

        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        public abstract void ReadXml(XmlReader reader);
        //{
        //    string content = reader.ReadContentAsString();
        //    string[] contentArray = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //    _backingList = new List<T>(contentArray.Cast<T>());
        //}

        public abstract void WriteXml(XmlWriter writer);
        //{
        //    writer.WriteString(string.Join(" ", _backingList.Select(i => i.ToString()).ToArray()));
        //}

        public override string ToString()
        {
            return BackingList.ToString();
        }
    }
}
