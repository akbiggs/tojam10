using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class StartLevelCanvas : MonoBehaviour {
    Timer countdownTimer;

    public Text infoText;

    private string[] messages = { "\tReady?", " 3", " . . . 2", " . . . 1" };
    private int index = 0;

	// Use this for initialization
	void Start () {
        this.countdownTimer = Timer.Register(1, countDown, true);
	}

    void countDown()
    {
        if (index < messages.Length)
        {
            this.infoText.text += this.messages[index++];
        }
        else
        {
            this.countdownTimer.Cancel();

            this.gameObject.SetActive(false);

            BlurOptimized cameraBlur = Camera.main.GetComponent<BlurOptimized>();
            cameraBlur.enabled = false;

            LevelController.instance.interactionOnPause = false;
        }
    }   

	// Update is called once per frame
	void Update () {
	
	}
}
