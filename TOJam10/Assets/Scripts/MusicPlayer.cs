using UnityEngine;
using System.Collections;

public class MusicPlayer : Singleton<MusicPlayer> {

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    //public void Awake()
    //{
    //    AudioSource audioSource = this.GetComponent<AudioSource>();

    //    Debug.Log("IN HERE");
    //    if (!audioSource.isPlaying)
    //    {
    //        audioSource.loop = true;
    //        audioSource.PlayOneShot(SoundManager.instance.music, 0.5f);
    //    }
    //}

    public void poop()
    {

    }
}
