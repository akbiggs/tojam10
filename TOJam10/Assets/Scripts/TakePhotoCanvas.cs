using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class TakePhotoCanvas : MonoBehaviour {
    public Canvas photograph;

    public Satisfiable[] photoRequirements;

    public Text resultsText;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        Debug.Log("Camera enabled.");
        RectTransform rectTransform = this.photograph.GetComponent<RectTransform>();
        rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-10, 10)));

        int countSatisfied = photoRequirements.Where(x => x.isSatisfied()).Count();

        if (countSatisfied == this.photoRequirements.Length)
        {
            resultsText.text = "Congratulations!\nAll requirements have been satisfied!";

            SoundManager.PlaySound(SoundManager.instance.victorySound, Vector3.zero);

            LevelController.instance.wonThisLevel = true;
        }
        else
        {
            float percent = 100 * (float) countSatisfied / this.photoRequirements.Length;
            resultsText.text = "Only " + percent + "% of the requirements have been met.\tKeep trying!";
        }
    }
}
