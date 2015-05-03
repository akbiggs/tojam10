using UnityEngine;
using System.Collections;

public class PosableModel : MonoBehaviour {

    public void ShowHearts()
    {
        this.transform.parent.FindChild("Heart Particle System").GetComponent<ParticleSystem>().Play();
    }
    
    public void ShowSparkles()
    {
        this.transform.parent.FindChild("Sparkle Particle System").GetComponent<ParticleSystem>().Play();
    }
}
