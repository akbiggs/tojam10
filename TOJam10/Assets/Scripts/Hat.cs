using UnityEngine;
using System.Collections;

public class Hat : MonoBehaviour {
    private Transform originalParentTransform;

	// Use this for initialization
	void Start () {
        this.originalParentTransform = this.transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void RestorParent()
    {
        this.transform.SetParent(this.originalParentTransform);
    }
}
