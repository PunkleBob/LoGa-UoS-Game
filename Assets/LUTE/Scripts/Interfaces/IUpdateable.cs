using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Interface for Engine components which can be updated when the 
/// scene loads in the editor. This is used to maintain backwards 
/// compatibility with earlier versions of our loga engine.
interface IUpdateable
{
    void UpdateToVersion(int oldVersion, int newVersion);
}
