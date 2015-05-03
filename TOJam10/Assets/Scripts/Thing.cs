using UnityEngine;
using System.Collections;

public class Thing : PhysicsMovement {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override Vector3 GetDesiredMovementDirection()
    {
        return Random.insideUnitCircle;
    }
}
