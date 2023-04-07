using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace VectorVanguard.SFX
{
  public class MusicManager : MonoBehaviour
  {
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioMixer mixer;
    
    [SerializeField] private bool _playOnStart;

    private void Awake()
    {
      musicSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
    }
    
    private void Start()
    {
      if (!_playOnStart) return;
      if (musicSource.clip != null) PlayMusic(musicSource.clip);
    }

    public void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true, float fadeDuration = 1f)
    {
      StartCoroutine(CrossFadeMusic(clip, volume, loop, fadeDuration));
    }

    public void StopMusic(float fadeDuration = 1f)
    {
      StartCoroutine(FadeOutMusic(fadeDuration));
    }

    private IEnumerator CrossFadeMusic(AudioClip clip, float volume, bool loop, float fadeDuration)
    {
      if (musicSource.isPlaying)
      {
        yield return StartCoroutine(FadeOutMusic(fadeDuration));
      }

      musicSource.clip = clip;
      musicSource.volume = 0f;
      musicSource.loop = loop;
      musicSource.Play();

      yield return StartCoroutine(FadeInMusic(volume, fadeDuration));
    }

    private IEnumerator FadeOutMusic(float fadeDuration)
    {
      var startVolume = musicSource.volume;

      while (musicSource.volume > 0)
      {
        musicSource.volume -= startVolume * Time.deltaTime / fadeDuration;
        yield return null;
      }

      musicSource.Stop();
    }

    private IEnumerator FadeInMusic(float targetVolume, float fadeDuration)
    {
      musicSource.volume = 0f;

      while (musicSource.volume < targetVolume)
      {
        musicSource.volume += targetVolume * Time.deltaTime / fadeDuration;
        yield return null;
      }
    }
  }
}