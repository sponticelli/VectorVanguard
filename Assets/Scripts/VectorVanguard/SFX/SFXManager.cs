using UnityEngine;
using UnityEngine.Audio;

namespace VectorVanguard.SFX
{
  public class SFXManager : MonoBehaviour
  {
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource[] audioSources;
    
    
    private void Awake()
    {
      if (audioSources.Length == 0) audioSources = GetComponentsInChildren<AudioSource>();
      
      if (mixer == null) return;
      var group = mixer.FindMatchingGroups("SFX")[0];
      foreach (var audioSource in audioSources)
      {
        audioSource.outputAudioMixerGroup = group;
      }
    }

    public void Play(AudioClip clip)
    {
      Play(clip, 1f, false);
    }
    
    public void Play(AudioClip clip, float volume, bool loop)
    {
      foreach (var audioSource in audioSources)
      {
        if (audioSource.isPlaying) continue;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.Play();
        break;
      }
    }
  }
}