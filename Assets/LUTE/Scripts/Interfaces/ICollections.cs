using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extension of IList for collections and support for associated orders
///
/// Built upon objects being passed in and returned as the base starting point.
/// The inherited classes may wish to provided typed access to underlying container,
/// this is what the GenericCollection does.
/// </summary>
public interface ICollections : System.Collections.IList
{
    int Capacity { get; set; }
    string Name { get; }

    void Add(ICollections rhsCol);

    void AddUnique(object o);

    void AddUnique(ICollections rhsCol);

    System.Type ContainedType();

    bool ContainsAllOf(ICollections rhsCol);

    bool ContainsAllOfOrdered(ICollections rhsCol);

    bool ContainsAnyOf(ICollections rhsCol);

    void CopyFrom(ICollections rhsCol);

    void CopyFrom(System.Array array);

    void CopyFrom(System.Collections.IList list);

    void Exclusive(ICollections rhsCol);

    object Get(int index);

    void Get(int index, ref Variable variable);

    void Intersection(ICollections rhsCol);

    bool IsCollectionCompatible(object o);

    bool IsElementCompatible(object o);

    int LastIndexOf(object o);

    int Occurrences(object o);

    void RemoveAll(ICollections rhsCol);

    void RemoveAll(object o);

    void Reserve(int count);

    void Resize(int count);

    void Reverse();

    void Set(int index, object o);

    void Shuffle();

    void Sort();

    void Unique();
}
