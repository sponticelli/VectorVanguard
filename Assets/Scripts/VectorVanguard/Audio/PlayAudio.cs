using UnityEngine;

namespace VectorVanguard.Audio
{
  public class PlayAudio : MonoBehaviour
  {
    [SerializeField] private AudioSource _audioSource;
    
    public void PlayOneShot(AudioClip clip)
    {
      _audioSource.PlayOneShot(clip);
    }
    
    public void Play(AudioClip clip)
    {
      _audioSource.clip = clip;
      _audioSource.loop = false;
      _audioSource.Play();
    }
    
    public void PlayLoop(AudioClip clip)
    {
      if (_audioSource.isPlaying && _audioSource.clip == clip)
        return;
      _audioSource.clip = clip;
      _audioSource.loop = true;
      _audioSource.Play();
    }
    
    public void Stop()
    {
      _audioSource.Stop();
      _audioSource.loop = false;
    }
  }
}