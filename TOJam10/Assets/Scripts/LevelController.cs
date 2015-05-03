using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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

    public virtual void Awake()
    {
        LevelController.instance = this;

        this.photos = new List<Texture>();

        this.interactionOnPause = true;
        //this.savedPhotoPath = Application.persistentDataPath + "/SavedScreens/";

        //Debug.Log("Saving to " + this.savedPhotoPath);

        //new FileInfo(this.savedPhotoPath).Directory.Create();

        //IEnumerable<string> fileNames = Directory.GetFileSystemEntries(this.savedPhotoPath);
        //foreach (string fileName in fileNames)
        //{
        //    Texture2D texture = new Texture2D(this.photoCamera.targetTexture.width, this.photoCamera.targetTexture.height);
        //    texture.LoadImage(File.ReadAllBytes(fileName));
        //    this.photos.Add(texture);
        //}

        MusicPlayer.Instance.PlayMusic();
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

    public void NextLevel()
    {
        int nextGameLevel = 0;

        if (Application.CanStreamedLevelBeLoaded(Application.loadedLevelName) &&
            Application.loadedLevel < Application.levelCount - 1)
        {
            nextGameLevel = Application.loadedLevel + 1;
        }

        Application.LoadLevel(nextGameLevel);
    }
}
