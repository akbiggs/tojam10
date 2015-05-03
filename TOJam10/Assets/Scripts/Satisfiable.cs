using UnityEngine;
using System.Collections;

public abstract class Satisfiable : MonoBehaviour {

    abstract public bool isSatisfied();

    public virtual bool IsActive()
    {
        return false;
    }
}
