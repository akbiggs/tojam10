using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// This class applies forces to a rigidbody so that its movement mimics that
/// of a character controller.
/// </summary>
public abstract class PhysicsMovement : MonoBehaviour
{

    const float EPSILON = 0.25f;

    /// <summary>
    /// The movement velocity of the character that we try to reach.
    /// </summary>
    public float maxVelocity = 5;

    /// <summary>
    /// The strength of the forces applied to the character to reach the maximum velocity.
    /// </summary>
    public float movementForce = 10;

    /// <summary>
    /// The strength of the movement force when suspended in midair.
    /// </summary>
    public float midairForce = 5;

    /// <summary>
    /// Additional gravity force to be applied to the character while it is grounded.
    /// </summary>
    public float groundStickyForce = 1f;

    /// <summary>
    /// The length of the ray that is shot beneath the character to determine if it is grounded.
    /// </summary>
    public float midairRayLength = 1;

    /// <summary>
    /// The size of the sphere-cast that is shot to determine if it is grounded.
    /// </summary>
    public float midairRayRadius = 0.5f;

    /// <summary>
    /// The speed at which the object turns to face its direction.
    /// </summary>
    public float turnSpeed = 4;

    /// <summary>
    /// A cached value of CheckIfGrounded that is updated at the beginning of each update loop.
    /// </summary>
    public bool isGrounded { get; protected set; }

    /// <summary>
    /// The direction that points up from the character's head.
    /// </summary>
    public Vector3 upDirection
    {
        get { return -Physics.gravity.normalized; }
    }

    private Vector3 lastRotateDirection;


    public abstract Vector3 GetDesiredMovementDirection();

    protected virtual void Update()
    {
        this.UpdateMovement();

        if (this.GetComponent<Rigidbody>().velocity.sqrMagnitude <= EPSILON * EPSILON)
        {
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    protected virtual void FixedUpdate()
    {
        // Lets the object stay rooted to the ground when standing.
        if (this.isGrounded)
        {
            this.GetComponent<Rigidbody>().AddForce(-this.transform.up * this.groundStickyForce);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position - this.transform.up * this.midairRayLength, this.midairRayRadius);
    }

    protected virtual bool ResolveCanMove()
    {
        return true;
    }

    protected virtual void UpdateMovement()
    {
        this.isGrounded = this.CheckIfGrounded();
        Vector3 direction = this.GetDesiredMovementDirection();

        if (this.ResolveCanMove())
        {
            Vector3 desiredVelocity = direction * this.CheckMaximumVelocity();
            Vector3 currentVelocity = this.GetComponent<Rigidbody>().velocity.SetY(0); // TOOD: Generalize to allow 
            Vector3 diff = desiredVelocity - currentVelocity;
            float force = this.CheckMovementForce();
            this.GetComponent<Rigidbody>().AddForceAtPosition(diff * force, this.transform.position);
        }

        Vector3 rotateDirection = this.GetDesiredRotateDirection();
        if (this.ResolveCanRotate() && rotateDirection != Vector3.zero)
        {
            float angle = Vector3.Angle(this.GetRotationModel().transform.forward.SetY(0), rotateDirection)
                * Mathf.Sign(Vector3.Cross(this.GetRotationModel().transform.forward.SetY(0), rotateDirection).y);
            this.GetRotationModel().transform.Rotate(this.upDirection, angle * this.turnSpeed * Time.deltaTime);
        }
    }

    protected virtual Vector3 GetDesiredRotateDirection()
    {
        return this.GetDesiredMovementDirection();
    }

    protected virtual bool ResolveCanRotate()
    {
        return this.ResolveCanMove() && this.isGrounded;
    }

    protected virtual GameObject GetRotationModel()
    {
        return this.gameObject;
    }

    public virtual bool CheckIfGrounded()
    {
        return !Input.GetMouseButton(0) && Physics.Raycast(transform.position, -Vector3.up, this.GetComponent<Collider>().bounds.extents.y + 0.15f);
    }

    public virtual float CheckMovementForce()
    {
        float force = this.isGrounded ? this.movementForce : this.midairForce;
        return force;
    }

    public virtual float CheckMaximumVelocity()
    {
        return this.maxVelocity;
    }
}