using System;
using UnityEngine;

namespace AtoGame.Base.UnityInspector.Editor {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SceneFieldAttribute : PropertyAttribute {
    }
}