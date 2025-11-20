using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Audio[] audios;

    public static AudioManager instance;

    private void Awake()
    {
       if (instance == null )
            instance = this;
       else
        {
            Destroy(gameObject);    
            return;
        }
        
        DontDestroyOnLoad(gameObject);

        foreach (Audio a in audios)
        {
            a.source = gameObject.AddComponent<AudioSource>();
            a.source.clip = a.clip;

            a.source.volume = a.volume;
            a.source.pitch = a.pitch;
            a.source.loop = a.loop;
        }
    }

    private void Start()
    {
        Play("Ambientação");
    }

    public void Play (string name)
    {
        Audio s = Array.Find(audios, Audio => Audio.name == name);
        if (s == null)
        { 
          Debug.LogWarning("som:" +  name + "não achado");
            return;
        }
        s.source.Play();
        
    }
}
