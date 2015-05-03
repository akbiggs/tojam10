using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Tossable : MonoBehaviour
{

    // sets how much the height of the toss is affected by the magnitude of the other axes
    public float tossHeightFactor;

    public new Rigidbody rigidbody;

    public virtual void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody>();
    }

    public virtual void GetPickedUp(Vector3 mousePos)
    {
        
    }

    public virtual void GetTossed(Vector3 dirAndSpeed)
    {
        //this.rigidbody.constraints = RigidbodyConstraints.None;
        this.rigidbody.AddForce(dirAndSpeed.SetY(dirAndSpeed.magnitude * tossHeightFactor) * 1000);
    }

}
