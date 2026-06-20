using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtherModules.CommandSystem
{
    public class Command : BaseCommand
    {
        private Action command;


        public Command(string id, string description, string format, Action command) : base(id, description, format)
        {
            this.command = command;
        }

        public override int GetParamNumber()
        {
            return 0;
        }

        public void Invoke()
        {
            command?.Invoke();
        }
    }

    public class Command<T> : BaseCommand
    {
        private Action<T> command;
        private ValueType valueType1;

        public Command(string id, string description, string format, Action<T> command) : base(id, description, format)
        {
            this.command = command;
        }

        public override int GetParamNumber()
        {
            return 1;
        }


        public void Invoke(T t)
        {
            command?.Invoke(t);
        }
    }


    public class Command<T1, T2> : BaseCommand
    {
        private Action<T1, T2> command;


        public Command(string id, string description, string format, Action<T1, T2> command) : base(id, description, format)
        {
            this.command = command;
        }

        public override int GetParamNumber()
        {
            return 2;
        }


        public void Invoke(T1 t1, T2 t2)
        {
            command?.Invoke(t1, t2);
        }
    }

    public class Command<T1, T2, T3> : BaseCommand
    {
        private Action<T1, T2, T3> command;


        public Command(string id, string description, string format, Action<T1, T2, T3> command) : base(id, description, format)
        {
            this.command = command;
        }

        public override int GetParamNumber()
        {
            return 3;
        }


        public void Invoke(T1 t1, T2 t2, T3 t3)
        {
            command?.Invoke(t1, t2, t3);
        }
    }
}