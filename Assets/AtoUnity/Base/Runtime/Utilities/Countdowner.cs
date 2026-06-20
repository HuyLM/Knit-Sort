namespace AtoGame.Base.Utilities 
{
    [System.Serializable]
    public struct Countdowner
    {
        [UnityEngine.SerializeField] private float countdown;

        public float Countdown { get => countdown; }

        public void StartCountdown(float time)
        {
            countdown = time;
        }

        public void Countdowning(float deltaTime)
        {
            if (countdown > 0)
            {
                countdown -= deltaTime;
            }
        }

        public bool IsTimeOut()
        {
            return countdown <= 0;
        }

        public bool IsCountdowning()
        {
            return countdown > 0;
        }

        public void Addtime(float time)
        {
            countdown += time;
        }

        public void Stop()
        {
            countdown = -1;
        }
    }
}