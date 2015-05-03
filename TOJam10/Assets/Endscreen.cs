using UnityEngine;
using System.Collections;

public class Endscreen : MonoBehaviour
{
    private bool canExit;

	// Use this for initialization
	void Start ()
	{
	    this.canExit = false;
	    Timer.Register(2f, () =>
	    {
	        this.canExit = true;
	    });
	}

    void Update()
    {
        Timer.UpdateAllRegisteredTimers();

        if (this.canExit && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("???");
            Application.LoadLevel(0);
        }
    }
}
