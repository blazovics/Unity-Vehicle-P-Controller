using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPilot : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 25.0f;

    [SerializeField]
    private float brakeDistance = 60.0f;

    Rigidbody rigidBody;
    WheelDrive wheelDrive;

    Transform target;

    [SerializeField]
    List<Transform> targetList = new List<Transform>();

    int currentTargetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        wheelDrive = GetComponent<WheelDrive>();
        rigidBody = GetComponent<Rigidbody>();

        LoadFirstTarget();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance > 10)
        {
            SteerControl();
            DistanceControl(distance);

        }
        else
        {
            LoadNextTarget();
        }
    }

    //For target loading at the Start
    private void LoadFirstTarget()
    {
        currentTargetIndex = 0;

        if (targetList.Count > 0)
        {
            target = targetList[currentTargetIndex];
        }
    }

    //For contious target loading
    private void LoadNextTarget()
    {
        currentTargetIndex++;

        if(currentTargetIndex >= targetList.Count)
        {
            currentTargetIndex = 0;
        }

        if (targetList.Count > 0)
        {
            target = targetList[currentTargetIndex];
        }
    }

    //P controller for distance -> Interacts with the speed controller
    private void DistanceControl(float distance)
    {
        float desiredSpeed = (distance > brakeDistance) ? maxSpeed : maxSpeed * (distance / brakeDistance);

        Debug.Log("distance: " + distance + " desired speed: " + desiredSpeed);

        SpeedControl(desiredSpeed);
    }

    //P controller for steering -> Interacts with the wheel angle
    private void SteerControl()
    {
        //only planar angles are interresting -> omit Y values
        Vector3 currentPos = transform.position;
        currentPos.y = 0;

        //only planar angles are interresting -> omit Y values
        Vector3 targetPos = target.position;
        targetPos.y = 0;

        Vector3 targetDir = targetPos - currentPos;
        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        wheelDrive.angle = -1.0f * ((Mathf.Abs(angle) > 60.0f) ? Mathf.Sign(angle) : angle / 60.0f);
    }

    //P controller for speed -> Interacts with the wheel torque
    private void SpeedControl(float desiredSpeed)
    {
        float currentSpeed = rigidBody.velocity.magnitude;
        float eSpeed = desiredSpeed - currentSpeed;
        Debug.Log("current speed: " + currentSpeed + " Speed error: " + eSpeed);
        wheelDrive.torque = ((Mathf.Abs(eSpeed) > 10.0f) ? Mathf.Sign(eSpeed) :  eSpeed / 10.0f);
    }
}
