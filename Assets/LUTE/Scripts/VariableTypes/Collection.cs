using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// Provides a common and complete MonoBehavior based reference point for custom collections.
/// GenericCollection inherits from this.
/// </summary>
[AddComponentMenu("")]
[System.Serializable]
public abstract class Collection : MonoBehaviour, ICollection
{
    public abstract int Capacity { get; set; }
    public abstract int Count { get; }
    public bool IsFixedSize { get { return false; } }
    public bool IsReadOnly { get { return false; } }
    public bool IsSynchronized { get { return false; } }
    public object SyncRoot { get { return null; } }
    public string Name { get { return name; } }

    public object this[int index] { get { return Get(index); } set { Set(index, value); } }

    public abstract int Add(object o);

    public abstract void Add(ICollection rhsCol);

    public abstract void AddUnique(object o);

    public abstract void AddUnique(ICollection rhsCol);

    public abstract void Clear();

    public abstract Type ContainedType();

    public abstract bool Contains(object o);

    public abstract bool ContainsAllOf(ICollection rhsCol);

    public abstract bool ContainsAllOfOrdered(ICollection rhsCol);

    public abstract bool ContainsAnyOf(ICollection rhsCol);

    public abstract void CopyFrom(ICollection rhsCol);

    public abstract void CopyFrom(System.Array array);

    public abstract void CopyFrom(System.Collections.IList list);

    public abstract void CopyTo(Array array, int index);

    public abstract void Exclusive(ICollection rhsCol);

    public abstract object Get(int index);

    public abstract void Get(int index, ref Variable variable);

    public abstract IEnumerator GetEnumerator();

    public abstract int IndexOf(object o);

    public abstract void Insert(int index, object o);

    public abstract void Intersection(ICollection rhsCol);

    public abstract bool IsCollectionCompatible(object o);

    public abstract bool IsElementCompatible(object o);

    public abstract int LastIndexOf(object o);

    public abstract int Occurrences(object o);

    public abstract void Remove(object o);

    public abstract void RemoveAll(ICollection rhsCol);

    public abstract void RemoveAll(object o);

    public abstract void RemoveAt(int index);

    public abstract void Reserve(int count);

    public abstract void Resize(int count);

    public abstract void Reverse();

    public abstract void Set(int index, object o);

    public abstract void Shuffle();

    public abstract void Sort();

    public abstract void Unique();
}
