using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    [SerializeField] private Slider music;
    [SerializeField] private Slider sound;
    public float musicVolume = 1f;
    public float soundVolume = 1f;
    public List<AudioSource> soundSources = new List<AudioSource>();
    public List<AudioSource> musicSources = new List<AudioSource>();

	// Start is called before the first frame update
	void Start()
    {
        instance = this;
        soundVolume = VolumeDataTransfer.instance.soundVolume;
        musicVolume = VolumeDataTransfer.instance.musicVolume;
        music.value = musicVolume;
        sound.value = soundVolume;
		UpdateSoundVolume();
        UpdateMusicVolume();
        musicSources[0].Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TransitionDanger(bool switches)
    {
        if (switches)
        {
			musicSources[0].Stop();
			musicSources[1].Play();
		}
        else
        {
			musicSources[1].Stop();
			musicSources[0].Play();
		}
    }
    public void UpdateSoundVolume()
    {
        soundVolume = sound.value;
        VolumeDataTransfer.instance.soundVolume = soundVolume;
        foreach(AudioSource source in soundSources)
        {
            source.volume = 0.5f * soundVolume;
        }
    }
    public void UpdateMusicVolume()
    {
        musicVolume = music.value;
		VolumeDataTransfer.instance.musicVolume = musicVolume;

		foreach (AudioSource source in musicSources)
        {
            source.volume = 0.5f * musicVolume;
        }
    }
}
