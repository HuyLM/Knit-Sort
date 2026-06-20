using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AtoGame.Mediation
{
    public class AdsEventExecutor : MonoBehaviour
    {
        public static AdsEventExecutor instance = null;

        private static List<Action> adEventsQueue = new List<Action>();

        private volatile static bool adEventsQueueEmpty = true;

        private static List<DelayTask> tasks = new List<DelayTask>();


        public static void Initialize()
        {
            if (IsActive())
            {
                return;
            }

            // Add an invisible game object to the scene
            GameObject obj = new GameObject("AdsMainThreadExecuter");
            obj.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(obj);
            instance = obj.AddComponent<AdsEventExecutor>();
        }

        public static bool IsActive()
        {
            return instance != null;
        }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public static void ExecuteInUpdate(Action action)
        {
            lock (adEventsQueue)
            {
                adEventsQueue.Add(action);
                adEventsQueueEmpty = false;
            }
        }

        public static void InvokeInUpdate(UnityEvent eventParam)
        {
            ExecuteInUpdate(() =>
            {
                eventParam.Invoke();
            });
        }

        public void Update()
        {
            if (adEventsQueueEmpty)
            {
                return;
            }

            List<Action> stagedAdEventsQueue = new List<Action>();

            lock (adEventsQueue)
            {
                stagedAdEventsQueue.AddRange(adEventsQueue);
                adEventsQueue.Clear();
                adEventsQueueEmpty = true;
            }

            foreach (Action stagedEvent in stagedAdEventsQueue)
            {
                if (stagedEvent.Target != null)
                {
                    stagedEvent.Invoke();
                }
            }
        }

        public void OnDisable()
        {
            instance = null;
        }

        #region Tasks


        public static void AddTask(DelayTask task)
        {
            if (tasks.Contains(task) == false)
            {
                tasks.Add(task);
            }
        }

        public static void Remove(DelayTask task)
        {
            if (tasks.Contains(task))
            {
                tasks.Remove(task);
            }
        }

        private void FixedUpdate()
        {
            if (tasks != null && tasks.Count > 0)
            {
                for (int i = 0; i < tasks.Count; i++)
                {
                    if (tasks[i] != null)
                    {
                        tasks[i].Update(Time.fixedDeltaTime);
                    }
                }
            }
        }
        #endregion
    }
}
