using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //https://www.youtube.com/watch?v=6OT43pvUyfY&ab_channel=Brackeys
    public Sound[] sounds;

    private void Awake()
    {
        foreach (Sound soundClip in sounds)
        {
            soundClip.source = gameObject.AddComponent<AudioSource>();
            soundClip.source.clip = soundClip.clip;

            soundClip.source.volume = soundClip.volume;
            soundClip.source.pitch = soundClip.pitch;
            soundClip.source.loop = soundClip.loop;
        }
    }

    public void PlaySound (string name)
    {
        Sound soundFound =  Array.Find(sounds, sound => sound.name == name);
        if(soundFound == null)
        {
            Debug.LogWarning($"The sound named {name}, dosn't exist");
            return;
        }                  
        soundFound.source.Play();
    }
}
