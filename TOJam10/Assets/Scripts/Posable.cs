using UnityEngine;
using System.Collections;

public enum PosableState 
{
    Posing,
    Wandering,
    Bored,
    LayingDown,
    Helpless
}

public enum PoseAnimation
{
    Flattered
}

public class Posable : MonoBehaviour
{
    public float rotationSpeed = 0.1f;

    private PosableState state;
    private PoseAnimation poseAnimation;

    private Vector3 wanderDirection;

    public void Start()
    {
        this.Wander();
    }

    public void Update()
    {
        switch (this.state)
        {
            case PosableState.Helpless:
                this.transform.Rotate(Vector3.up, this.rotationSpeed);
                break;

            case PosableState.Wandering:
                this.transform.position += this.wanderDirection;
                break;
        }
    }

    public void EnterState(PosableState state)
    {
        this.state = state;
    }

    public void Pose(PoseAnimation anim)
    {
        this.poseAnimation = anim;

        this.EnterState(PosableState.Posing);
    }

    public void BecomeHelpless(Vector3 mousePosition)
    {
        this.EnterState(PosableState.Helpless);
    }

    public void Wander()
    {
        this.wanderDirection = Random.insideUnitSphere.SetY(0);

        this.EnterState(PosableState.Wandering);
    }
}
