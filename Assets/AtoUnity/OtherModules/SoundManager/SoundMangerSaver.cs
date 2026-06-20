using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AtoGame.OtherModules.SoundManager
{
    public interface SoundMangerSaver
    {
        // Master
        void SetMasterEnable(bool enable);
        void SetMasterVolume(float volume);
        bool GetMasterEnable();
        float GetMasterVolume();

        // Music
        void SetMusicEnable(bool enable);
        void SetMusicVolume(float volume);
        bool GetMusicEnable();
        float GetMusicVolume();

        // SFX
        void SetSFXEnable(bool enable);
        void SetSFXVolume(float volume);
        bool GetSFXEnable();
        float GetSFXVolume();

        // Others
        void SetOtherEnable(string id, bool enable);
        void SetOtherVolume(string id, float volume);
        bool GetOtherEnable(string id);
        float GetOtherVolume(string id);

    }
}
