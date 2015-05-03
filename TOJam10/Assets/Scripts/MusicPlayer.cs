using UnityEngine;
using System.Collections;

public class MusicPlayer : Singleton<MusicPlayer> {

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void awake()
    {

    }

    public void poop()
    {
        AudioSource audSource = this.gameObject.GetComponent<AudioSource>();
        if (audSource == null)
        {
            Debug.Log("Playing music.");

            audSource = this.gameObject.AddComponent<AudioSource>();

            audSource.loop = true;
            audSource.PlayOneShot(SoundManager.instance.music, 0.5f);
        }
    }
}
