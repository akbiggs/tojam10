using UnityEngine;
using System.Collections;

public class MusicPlayer : Singleton<MusicPlayer> {

    public AudioClip music;
    public AudioSource musicPlayer;
	// Use this for initialization
	void Start () {
        musicPlayer = new AudioSource();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void PlayMusic()
    {
        if (!this.musicPlayer.isPlaying)
        {
            this.musicPlayer.loop = true;
            this.musicPlayer.PlayOneShot(this.music, 0.5f);
        }
    }
}
