using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtherModules.CommandSystem
{
    public abstract class BaseCommand
    {
        private string id;
        private string description;
        private string format;

        private string idField;
        private string descField;
        private string formatField;


        public string Id => id;
        public string Description => description;
        public string Format => format;

        public BaseCommand(string id, string description, string format)
        {
            this.id = id;
            this.description = description;
            this.format = format;
        }

        public abstract int GetParamNumber();

        public string ToString(Color color, int idFieldPos, int descFieldPos, int formatFieldPos)
        {
            if(idFieldPos > 0)
            {
                idField = $"<pos={idFieldPos}%>" + id;
            }
            else
            {
                idField = string.Empty;
            }

            if(descFieldPos > 0)
            {
                descField = $"<pos={descFieldPos}%>" + description;
            }
            else
            {
                descField = string.Empty;
            }

            if(formatFieldPos > 0)
            {
                formatField = $"<pos={formatFieldPos}%>" + format;
            }
            else
            {
                formatField = string.Empty;
            }
            return $"{idField}{descField}<color=#{ColorUtility.ToHtmlStringRGB(color)}>{formatField}</color>";
        }
    }
}