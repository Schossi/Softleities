using System;
using UnityEngine;
using UnityEngine.Events;

public class DependenciesDebug : MonoBehaviour
{
    [Serializable]
    public class SubTest
    {
        public DependenciesDebug SubField;
        public SubSubTest SubSub;
    }
    [Serializable]
    public class SubSubTest
    {
        public DependenciesDebug SubSubField;
    }

    //public MonoScript MonoScript;
    public DependenciesDebug Field;
    public DependenciesDebug FieldB;
    public DependenciesDebug[] ArrayField;
    public UnityEvent<DependenciesDebug> Event;
    public SubTest Sub;

    public void Test()
    {

    }
}
