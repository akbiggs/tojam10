using UnityEngine;
using System.Collections;

public class Hat : MonoBehaviour {
    private Transform originalParentTransform;
    public GameObject owner;
    public float offsetFromHead = 0.5f;

	// Use this for initialization
	void Start () {
        this.originalParentTransform = this.transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    internal void RestoreParent()
    {
        Vector3 position = this.transform.position;
        this.transform.SetParent(this.originalParentTransform);
        this.transform.position = position;
    }
}
