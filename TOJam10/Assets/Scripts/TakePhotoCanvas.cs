using UnityEngine;
using System.Collections;

public class TakePhotoCanvas : MonoBehaviour {
    public Canvas photograph;

    public Satisfiable[] photoRequirements;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {
        Debug.Log("Camera enabled.");
        //this.photograph.transform.localRotation = new Quaternion(0, 0, 20, 0);
    }
}
