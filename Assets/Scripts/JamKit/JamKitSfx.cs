using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace JamKit
{
    public partial class JamKit
    {
        [Header("Sfx")]
        [SerializeField] private AudioSource _commonAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private SfxDatabase _database;

        private const float SfxVolume = 0.5f;
        private const float MusicVolume = 0.5f;

        private bool _isMusicPaused;

        private void StartSfx()
        {
            _commonAudioSource.volume = SfxVolume;
            _musicAudioSource.volume = MusicVolume;
        }

        public void FollowTransform(Transform t)
        {
            transform.SetParent(t);
        }

        [CanBeNull]
        private bool TryGetClip(string clipName, out AudioClip clip)
        {
            clip = null;
            foreach (AudioClip audioClip in _database.Clips)
            {
                if (audioClip == null)
                {
                    Debug.LogWarning("There's a null clip in the sfx database");
                    continue;
                }

                if (audioClip.name == clipName)
                {
                    clip = audioClip;
                    return true;
                }
            }

            Debug.LogError($"Audioclip not found: {clipName}");
            return false;
        }

        public void StartMusic(string clipName, bool isLooped)
        {
            if (TryGetClip(clipName, out AudioClip clip))
            {
                _musicAudioSource.loop = isLooped;
                _musicAudioSource.clip = clip;
                _musicAudioSource.volume = _isMusicPaused ? 0f : MusicVolume;
                _musicAudioSource.Play();
            }
        }

        public void PlayRandom(string clipPrefix)
        {
            List<AudioClip> clips = _database.Clips.Where(x => x.name.StartsWith(clipPrefix)).ToList();

            if (clips.Count > 0)
            {
                _commonAudioSource.PlayOneShot(clips.GetRandom());
            }
            else
            {
                Debug.LogError($"Couldn't find any clips with prefix {clipPrefix}");
            }
        }

        public void Play(string clipName)
        {
            if (TryGetClip(clipName, out AudioClip clip))
            {
                _commonAudioSource.PlayOneShot(clip);
            }
        }

        public void ChangeMusicTrack(string clipName)
        {
            if (TryGetClip(clipName, out AudioClip clip))
            {
                _musicAudioSource.clip = clip;
                _musicAudioSource.volume = MusicVolume;
                _musicAudioSource.Play();
            }
        }

        public void FadeOutMusic(float duration)
        {
            Tween(AnimationCurve.Linear(0f, 0f, 1f, 1f),
                duration,
                t => { _musicAudioSource.volume = Mathf.Lerp(MusicVolume, 0f, t); },
                () => { _musicAudioSource.volume = 0f; });
        }

        public void PlayOneShot(string clipName, Vector3 position, float volume = 1.0f)
        {
            if (TryGetClip(clipName, out AudioClip clip))
            {
                AudioSource.PlayClipAtPoint(clip, position, volume);
            }
        }

        private void UpdateSfx()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (_musicAudioSource.volume > 0.01f)
                {
                    _musicAudioSource.volume = 0f;
                    _isMusicPaused = true;
                }
                else
                {
                    _musicAudioSource.volume = MusicVolume;
                    _isMusicPaused = false;
                }
            }
        }
    }
}