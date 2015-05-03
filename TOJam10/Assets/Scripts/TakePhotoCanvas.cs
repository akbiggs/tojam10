using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class TakePhotoCanvas : MonoBehaviour {
    public Canvas photograph;

    private Satisfiable[] photoRequirements;

    public Text resultsText;

    private bool isTitleScreen;

	// Use this for initialization
	void Awake() {
        this.photoRequirements = GameObject.FindObjectsOfType<Satisfiable>().Where(o => o.getTotalToSatisfy() > 0).ToArray();
	}

    void OnEnable()
    {
        Debug.Log("Camera enabled.");

        if (this.isTitleScreen)
        {
            return;
        }

        if (this.photograph != null)
        {
            RectTransform rectTransform = this.photograph.GetComponent<RectTransform>();
            rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-10, 10)));
        }

        //Returna  total count of the number satisfied
        int countSatisfied = 0;
        int countTotal = 0;
        foreach (Satisfiable sat in this.photoRequirements)
        {
            countSatisfied += sat.getNumSatisfy();
            countTotal += sat.getTotalToSatisfy();
        }

        if (countSatisfied == countTotal)
        {
            resultsText.text = "Congratulations!\nAll requirements have been satisfied!";

            Timer.Register(0.5f, () =>
            {
                SoundManager.PlaySound(SoundManager.instance.victorySound, Vector3.zero);

                Timer.Register(0.2f, () =>
                {
                    LevelController.instance.wonThisLevel = true;
                });
            });
        }
        else
        {
            float percent = 100 * (float)countSatisfied / countTotal;
            resultsText.text = "Only " + percent + "% of the requirements have been met.\n\tKeep trying!";
        }
    }

    public void TitlePhotoSnapFinished()
    {
        this.isTitleScreen = true;
        LevelController.instance.FadeToNextLevel();
    }
}
