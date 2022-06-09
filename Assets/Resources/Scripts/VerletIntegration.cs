using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletIntegration : MonoBehaviour
{
    public CharacterController characterController;

    //Tether & Arm---------
    public Transform tetherTransform;
    public Vector3 tetherPosition;
    float armLength;

    //Velocity----
    Vector3 previousPos;
    Vector3 previousVelocity;
    Vector3 velocity;

    //Gravity--------
    public float gravity = 20f;
    Vector3 gravityDirection = new Vector3(0, 1, 0);

    //Drag & damping
    Vector3 dampingDirection;
    float drag = 0.001f;
    float maximumSpeed = 50;

    private void Start()
    {
        transform.parent = tetherTransform;
        armLength = Vector3.Distance(transform.localPosition, tetherPosition);
    }

    private void FixedUpdate()
    {
        previousVelocity = GetFinalVelocity(transform.localPosition, Time.fixedDeltaTime);
        characterController.Move(previousVelocity);
    }

    public Vector3 GetFinalVelocity(Vector3 pos, float time)
    {
        velocity += GetConstrainedVelocity(pos, previousPos, time);

        ApplyGravity();
        ApplyDamping();
        CapMaxSpeed();

        pos += velocity * time;

        previousPos = pos;
        return velocity * time;
    }

    public Vector3 GetConstrainedVelocity(Vector3 currentPos, Vector3 previousPos, float time)
    {
        float distanceToTether;
        Vector3 constrainedPos;
        Vector3 predictedPos;

        distanceToTether = Vector3.Distance(currentPos, tetherPosition);

        if (distanceToTether > armLength)
        {
            constrainedPos = Vector3.Normalize(currentPos - tetherPosition) * armLength;
            predictedPos = (constrainedPos - previousPos) / time;
            return predictedPos;
        }
        //else if (distanceToTether < armLength)
        //{
        //    constrainedPos = Vector3.Normalize(currentPos - tetherPosition) * armLength;
        //    predictedPos = (constrainedPos - previousPos) / time;
        //    return predictedPos;
        //}
        return Vector3.zero;
    }

    #region GravityFunctions

    public void ApplyGravity()
    {
        velocity -= gravityDirection * gravity * Time.fixedDeltaTime;
    }

    public void ApplyDamping()
    {
        dampingDirection = -velocity;
        dampingDirection *= drag;
        velocity += dampingDirection;
    }

    public void CapMaxSpeed()
    {
        velocity = Vector3.ClampMagnitude(velocity, maximumSpeed);
    }
    #endregion
}
