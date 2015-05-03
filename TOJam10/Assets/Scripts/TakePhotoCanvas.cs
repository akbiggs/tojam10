using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class TakePhotoCanvas : MonoBehaviour {
    public Canvas photograph;

    private Satisfiable[] photoRequirements;

    public Text resultsText;

	// Use this for initialization
	void Awake() {
        this.photoRequirements = GameObject.FindObjectsOfType<Satisfiable>().Where(o => o.IsActive()).ToArray();
	}

    void OnEnable()
    {
        Debug.Log("Camera enabled.");
        RectTransform rectTransform = this.photograph.GetComponent<RectTransform>();
        rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-10, 10)));

        int countSatisfied = photoRequirements.Count(x => x.isSatisfied());

        if (countSatisfied == this.photoRequirements.Length)
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
            float percent = 100 * (float) countSatisfied / this.photoRequirements.Length;
            resultsText.text = "Only " + percent + "% of the requirements have been met.\tKeep trying!";
        }
    }
}
