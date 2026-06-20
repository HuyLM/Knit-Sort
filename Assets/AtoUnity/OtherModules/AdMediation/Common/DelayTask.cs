using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public class DelayTask
    {
        private float delay; // sec
        private Action onComplete;


        private bool isActive;
        private float time; // sec


        public DelayTask(float delay, Action onComplete)
        {
            this.delay = delay;
            this.onComplete = onComplete;
        }

        public void Start()
        {
            time = delay;
            isActive = true;
        }

        public void Pause()
        {
            isActive = false;
        }

        public void Resume()
        {
            isActive = true;
        }

        public void Stop(bool complete)
        {
            time = 0;
            isActive = false;

            if (complete)
            {
                onComplete?.Invoke();
            }
        }

        public void Update(float deltaTime)
        {
            if (isActive)
            {
                if (time > 0)
                {
                    time -= deltaTime;
                }
                else
                {
                    Stop(true);
                }
            }
        }

    }
}
