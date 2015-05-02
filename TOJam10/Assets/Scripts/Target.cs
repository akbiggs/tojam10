using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider c)
    {
        Debug.Log("HERE");
        Posable posable = c.gameObject.GetComponent<Posable>();

        if (posable != null)
        {
            posable.Pose(PoseAnimation.Flattered);
        }
    }
}
