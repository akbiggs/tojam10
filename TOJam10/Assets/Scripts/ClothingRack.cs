using UnityEngine;
using System.Collections;

public class ClothingRack : Satisfiable {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        Posable p = collision.gameObject.GetComponent<Posable>();
        if (p != null && p.isNaked)
        {
            p.skinRenderer.material = p.suitMaterial;
            p.isNaked = false;
        }
    }

}
