using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.Inventory
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ItemFieldAttribute : PropertyAttribute
    {
        [NonSerialized] public string[] NameCollectors;
        [NonSerialized] public bool RemoveCollectorName;

        public ItemFieldAttribute()
        {
            this.NameCollectors = null;
            RemoveCollectorName = false;
        }

        public ItemFieldAttribute(params string[] nameCollector)
        {
            this.NameCollectors = nameCollector;
            RemoveCollectorName = false;
        }

        public ItemFieldAttribute(bool removeCollectorName, params string[] nameCollector)
        {
            this.NameCollectors = nameCollector;
            RemoveCollectorName = removeCollectorName;
        }
    }
}
