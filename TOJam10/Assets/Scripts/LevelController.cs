using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;

    public virtual void Awake()
    {
        LevelController.instance = this;
    }

	// Update is called once per frame
	void Update () {
        Timer.UpdateAllRegisteredTimers();
	}
}
