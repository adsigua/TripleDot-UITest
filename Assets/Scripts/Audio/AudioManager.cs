using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AdoTrpDotTest.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioListener _audioListener;
        [SerializeField] private AudioMixer _mainMixer;
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private float _muteVolume = -20.0f;

        [SerializeField] private List<AudioClip> _sfxBank = new();

        public void ToggleMusic(bool isOn)
        {
            _mainMixer.SetFloat("MusicVolume", isOn ? 0.0f : _muteVolume);
        }

        public void ToggleSound(bool isOn)
        {
            _mainMixer.SetFloat("SoundVolume", isOn ? 0.0f : _muteVolume);
        }

        public bool IsMusicOn()
        {
            _mainMixer.GetFloat("MusicVolume", out float volume);
            return volume > _muteVolume;
        }

        public bool IsSoundOn()
        {
            _mainMixer.GetFloat("SoundVolume", out float volume);
            return volume > _muteVolume;
        }

        public void PlaySFXOneShot(SFXType sfxType)
        {
            if((int)sfxType >= _sfxBank.Count)
                return;
            _sfxAudioSource.PlayOneShot(_sfxBank[(int)sfxType]);
        }
    }
}
