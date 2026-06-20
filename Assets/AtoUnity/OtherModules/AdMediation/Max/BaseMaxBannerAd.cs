using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.Mediation
{
    public abstract class BaseMaxBannerAd : BaseAd
    {
        public abstract void Destroy();
        public abstract void Hide();
        public abstract void Display();
        public abstract void Reload();
    }
}
