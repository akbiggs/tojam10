using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip[] niceHomeSounds;
    public AudioClip[] lonelySounds;
    public AudioClip[] childSounds;
    
    public AudioClip[] pineappleSounds;
    public AudioClip[] fancyClothesSounds;

    public AudioClip[] poseSounds;

    public AudioClip[] collisionSounds;

    public AudioClip[] walkingSounds;

    public AudioClip victorySound;

    public AudioClip cameraShutter;

	public AudioReverbPreset reverbType = AudioReverbPreset.Off;

    public static void PlaySound(AudioClip sound, Vector3 pos)
    {
        SoundManager.instance.PlayClipAt(sound, pos);
    }

    public static void MaybePlaySound(AudioClip sound, Vector3 pos, float probability)
    {
        if (Random.value <= probability)
        {
            SoundManager.PlaySound(sound, pos);
        }
    }

    public static void PlayRandomSound(AudioClip[] sounds, Vector3 pos)
    {
        if (sounds.Length == 0)
            return;

        SoundManager.PlaySound(sounds[Random.Range(0, sounds.Length)], pos);
    }

	// Use this for initialization
	void Awake()
	{
	    SoundManager.instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private AudioSource PlayClipAt(AudioClip clip, Vector3 pos, bool useReverb = false) {
		if (clip == null) {
			Debug.LogWarning("Audio clip is not assigned to a value!");
			return null;
		}

		GameObject tempGO = new GameObject("TempAudio"); // create the temp object
		tempGO.transform.position = pos; // set its position
		AudioSource aSource = tempGO.AddComponent<AudioSource>(); // add an audio source
		aSource.clip = clip; // define the clip

		if (useReverb) {
			AudioReverbFilter reverbFilter = aSource.gameObject.AddComponent<AudioReverbFilter>();
			reverbFilter.reverbPreset = reverbType;
		}

		// set other aSource properties here, if desired
		aSource.Play(); // start the sound
		Destroy(tempGO, clip.length + 0.1f); // destroy object after clip duration
		return aSource; // return the AudioSource reference
    }
}
