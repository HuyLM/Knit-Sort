using System;
using UnityEngine;

namespace AtoGame.Base.UnityInspector
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute {
    }
}