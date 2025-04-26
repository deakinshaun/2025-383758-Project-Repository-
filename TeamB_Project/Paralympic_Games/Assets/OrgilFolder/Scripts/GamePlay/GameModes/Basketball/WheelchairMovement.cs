using System;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay.GameModes.Basketball
{
    public class WheelchairMovement : MonoBehaviour
    {
        [SerializeField] private Transform leftWheel;
        [SerializeField] private Transform rightWheel;
        [SerializeField] private float wheelRadius;

        [SerializeField] private float speedDiffThreshold = 2f;
        [Header("PC Only")] [SerializeField] private float rotSpeed = 3;

        private float leftPrevAngle;
        private float rightPrevAngle;
        private float wheelBase;
        private Rigidbody _leftRb;
        private Rigidbody _rightRb;

        private Vector3 leftWheelUpLocal;
        private Vector3 rightWheelUpLocal;

        private void Start()
        {
            _leftRb = leftWheel.GetComponent<Rigidbody>();
            _rightRb = rightWheel.GetComponent<Rigidbody>();

            leftWheelUpLocal = transform.InverseTransformDirection(leftWheel.transform.up);
            rightWheelUpLocal = transform.InverseTransformDirection(rightWheel.transform.up);


            leftPrevAngle = GetCurrentRotAngle(leftWheel, transform.TransformDirection(leftWheelUpLocal));
            rightPrevAngle = GetCurrentRotAngle(rightWheel, transform.TransformDirection(rightWheelUpLocal));


            wheelBase = Vector3.Distance(leftWheel.transform.position, rightWheel.transform.position);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                float x = Input.mousePositionDelta.x;
                leftWheel.transform.rotation = Quaternion.AngleAxis(x * Time.deltaTime, leftWheel.right) *
                                               leftWheel.transform.rotation;
            }

            if (Input.GetKey(KeyCode.A))
            {
                leftWheel.transform.rotation = Quaternion.AngleAxis(-rotSpeed * Time.deltaTime, leftWheel.right) *
                                               leftWheel.transform.rotation;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                leftWheel.transform.rotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, leftWheel.right) *
                                               leftWheel.transform.rotation;
            }


            if (Input.GetKey(KeyCode.D))
            {
                rightWheel.transform.rotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, rightWheel.right) *
                                                rightWheel.transform.rotation;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                rightWheel.transform.rotation = Quaternion.AngleAxis(-rotSpeed * Time.deltaTime, rightWheel.right) *
                                                rightWheel.transform.rotation;
            }
        }

        private void FixedUpdate()
        {
            var LRotAngle = -GetCurrentRotAngle(leftWheel, transform.TransformDirection(leftWheelUpLocal));
            var RRotAngle = GetCurrentRotAngle(rightWheel, transform.TransformDirection(rightWheelUpLocal));

            float leftAngVel = CalculateRotVel(leftPrevAngle, LRotAngle);
            float rightAngVel = CalculateRotVel(rightPrevAngle, RRotAngle);

            float leftSpeed = Mathf.Deg2Rad * leftAngVel * wheelRadius;
            float rightSpeed = Mathf.Deg2Rad * rightAngVel * wheelRadius;

            float forwardSpeed = (leftSpeed + rightSpeed) * 0.5f;
            float rotAngVel = Mathf.Rad2Deg * (leftSpeed - rightSpeed) / wheelBase;

            transform.position += transform.forward * (forwardSpeed * Time.fixedDeltaTime);

            if (Mathf.Abs(rotAngVel) > speedDiffThreshold)
            {
                transform.rotation = Quaternion.AngleAxis(rotAngVel * Time.fixedDeltaTime, Vector3.up) *
                                     transform.rotation;
            }

            leftPrevAngle = LRotAngle;
            rightPrevAngle = RRotAngle;
        }


        private float GetCurrentRotAngle(Transform wheelTf, Vector3 wheelUp)
        {
            return Vector3.SignedAngle(wheelUp, wheelTf.up, wheelTf.transform.right);
        }

        private float CalculateRotVel(float prevRot, float currentRot)
        {
            return Mathf.DeltaAngle(prevRot, currentRot) / Time.fixedDeltaTime;
        }
    }
}