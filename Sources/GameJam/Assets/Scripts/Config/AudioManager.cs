using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OneP.Samples;
using Random = UnityEngine.Random;

public class AudioManager: SingletonMono<AudioManager>
{

	[SerializeField] private AudioSource backgroundMusic;
	AudioSource musicPlayer;
	
	public AudioSource[] soundEffect;

    public float effectVolume = 0.8f;
	
	// Use this for initialization
	void Start () {
		GameObject musicObject = new GameObject("MusicPlayer");
		musicPlayer = musicObject.AddComponent<AudioSource>();
		musicPlayer.loop = false;
		musicPlayer.playOnAwake = false;
		musicObject.transform.parent = this.transform;
	}

    //public bool istest = false;
	// Update is called once per frame
	void Update () {
        //if (istest) {
        //    istest = false;
        //    RefreshSongList();
        //}
	}

	public void LoadLocalMusic(String songName)
	{
		AudioClip audioClip = (AudioClip) Resources.Load(songName);
		musicPlayer.clip = audioClip;
		musicPlayer.Stop();
	}

	public void SetVolume(float value) {
		musicPlayer.volume = value;
		backgroundMusic.volume = value;
	}

	public void SetMute(bool isMute) {
		
		if (isMute) {
			SetVolume(0);
		} else {
			SetVolume(1f);
		}
	}

	public void PlayMusic() {
		backgroundMusic.Stop();
		musicPlayer.Play();
	}

	public void StopMusic()
	{
		musicPlayer.Stop();
	}

	public void PlayBackgroundMusic()
	{
		if (musicPlayer!=null&&musicPlayer.isPlaying)
		{
		}
		else
		{
			backgroundMusic.Play();
		}
	}


	public void PauseMusic(bool isPause)
	{
		if (isPause)
		{
			musicPlayer.Pause();
		}
		else
		{
			musicPlayer.UnPause();
		}
	}

	public void PlaySoundEffect(int index){
		if (index < soundEffect.Length && soundEffect [index] != null)
		{
			soundEffect[index].volume = this.effectVolume;
			soundEffect [index].Play ();

            Debug.Log("Volume Effect: " + this.effectVolume);
		}
	}

    public void SetSoundEffectVolume(float volume)
    {
        Debug.Log("Change Effect Volume To: " + volume);
        this.effectVolume = volume;
    }
}
