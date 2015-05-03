using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;
    public List<Texture> photos { get; private set; }
    public Camera photoCamera;
    private string savedPhotoPath;

    public GameObject ui;
    public GameObject photoReelPrefab;

    public Boolean wonThisLevel { get; set; }
    public Boolean interactionOnPause { get; set; }

    public Canvas fadeoutCanvas;

    public float quipMinTime;
    public float quipMaxTime;
    private Posable[] posables;

    public Text timeLeftText;
    private float timeAvailable = 11;

  //  public bool startCountdown {public set; private get;}

    public virtual void Awake()
    {
        LevelController.instance = this;

        this.photos = new List<Texture>();
        this.posables = GameObject.FindObjectsOfType<Posable>();

        if (Application.loadedLevelName.ToLower().Contains("titlescreen"))
        {
            this.PlayRandomQuipAfterTime();
        }

        this.interactionOnPause = true;
        //this.savedPhotoPath = Application.persistentDataPath + "/SavedScreens/";

       // this.startCountdown = false;

        //Debug.Log("Saving to " + this.savedPhotoPath);

        //new FileInfo(this.savedPhotoPath).Directory.Create();

        //IEnumerable<string> fileNames = Directory.GetFileSystemEntries(this.savedPhotoPath);
        //foreach (string fileName in fileNames)
        //{
        //    Texture2D texture = new Texture2D(this.photoCamera.targetTexture.width, this.photoCamera.targetTexture.height);
        //    texture.LoadImage(File.ReadAllBytes(fileName));
        //    this.photos.Add(texture);
        //}


        MusicPlayer.Instance.poop();

        this.timeLeftText.text = "";
    }

    public void PlayRandomQuipAfterTime()
    {
        Posable posable = this.posables[Random.Range(0, this.posables.Length)];

        Timer.Register(Random.Range(this.quipMinTime, this.quipMaxTime), () =>
        {
            Debug.Log("Random sound");

            posable.PlayRandomQuip();

            this.PlayRandomQuipAfterTime();
        });
    }

    void Update()
    {
        Timer.UpdateAllRegisteredTimers();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameObject photoReel = Instantiate(this.photoReelPrefab) as GameObject;
            photoReel.transform.position = Vector3.zero;
            //photoReel.transform.SetParent(this.ui.transform);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (!this.interactionOnPause)
        {
            if (this.timeAvailable < 0)
            {
                SoundManager.PlaySound(SoundManager.instance.outOfTime, this.transform.position);
                this.timeAvailable = 0;
                this.timeLeftText.text = "0:00   ";

                this.interactionOnPause = true;
                this.RestartLevel();
            }
            else
            {
                timeAvailable -= Time.deltaTime;
                this.timeLeftText.text = Math.Floor(timeAvailable) + ":00   ";
            }
        }
    }

    public void AddPhoto(Texture photo)
    {
        //Texture2D photo2D = new Texture2D(renderedPhoto.width, renderedPhoto.height);
        
        //// write photo to file
        //RenderTexture.active = renderedPhoto;
        //photo2D.ReadPixels(new Rect(0, 0, photo2D.width, photo2D.height), 0, 0);
        //photo2D.Apply();

        //this.photos.Add(photo2D);

        //string photoName = string.Format("photo{0}.png", Guid.NewGuid());
        //Debug.Log("Saving to " + this.savedPhotoPath + photoName);
        //File.WriteAllBytes(this.savedPhotoPath + photoName, photo2D.EncodeToPNG());
        this.photos.Add(photo);
    }

    public void RemovePhoto(Texture photo)
    {
        this.photos.Remove(photo);
    }

    public void RestartLevel()
    {
        Timer.Register(2f, () =>
        {
            MusicPlayer.Instance.poop();
            Timer.CancelAllRegisteredTimers();
            Application.LoadLevel(Application.loadedLevel);
        }, false);

        this.fadeoutCanvas.gameObject.SetActive(true);
    }

    public void NextLevel()
    {
        int nextGameLevel = 0;

        MusicPlayer.Instance.poop();

        if (Application.CanStreamedLevelBeLoaded(Application.loadedLevelName) &&
            Application.loadedLevel < Application.levelCount - 1)
        {
            nextGameLevel = Application.loadedLevel + 1;
        }

        Timer.CancelAllRegisteredTimers();

        Application.LoadLevel(nextGameLevel);
    }

    public void FadeToNextLevel()
    {
        Timer.Register(1, this.NextLevel, false);
        this.fadeoutCanvas.gameObject.SetActive(true);
    }
}
