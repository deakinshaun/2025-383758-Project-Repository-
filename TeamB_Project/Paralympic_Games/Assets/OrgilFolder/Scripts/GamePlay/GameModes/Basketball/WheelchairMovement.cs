using System;
using Fusion;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay.GameModes.Basketball
{
    public class WheelchairMovement : NetworkBehaviour
    {
        [SerializeField] private Transform leftWheel;
        [SerializeField] private Transform rightWheel;
        [SerializeField] private float wheelRadius;

        [SerializeField] private float speedDiffThreshold = 2f;
        [Header("PC Only")] [SerializeField] private float rotSpeed = 3;
        private float leftPrevAngle;
        private float rightPrevAngle;
        private float wheelBase;
        private Vector3 leftWheelUpLocal;
        private Vector3 rightWheelUpLocal;
        private InputSystem_Actions _inputSystemActions;

        private Vector3 inputVel;
        private Quaternion inputRot;
        
        public override void Spawned()
        {
            base.Spawned();

            leftWheelUpLocal = transform.InverseTransformDirection(leftWheel.transform.up);
            rightWheelUpLocal = transform.InverseTransformDirection(rightWheel.transform.up);
            leftPrevAngle = GetCurrentRotAngle(leftWheel, transform.TransformDirection(leftWheelUpLocal));
            rightPrevAngle = GetCurrentRotAngle(rightWheel, transform.TransformDirection(rightWheelUpLocal));
            wheelBase = Vector3.Distance(leftWheel.transform.position, rightWheel.transform.position);

            inputRot = transform.rotation;

            _inputSystemActions = new InputSystem_Actions();
            _inputSystemActions.Enable();
            if (HasInputAuthority)
            {
                PlayerInputBehaviour.GetInput += ProvideInput;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            if (HasInputAuthority)
            {
                PlayerInputBehaviour.GetInput -= ProvideInput;
            }
        }

        private PlayerInput ProvideInput()
        {
            return new PlayerInput
            {
                rotation = inputRot,
                velocity = inputVel
            };
        }

        private void Update()
        {
            if (_inputSystemActions.Player.LeftWheelUp.IsPressed())
            {
                leftWheel.transform.rotation = Quaternion.AngleAxis(-rotSpeed * Time.deltaTime, leftWheel.right) *
                                               leftWheel.transform.rotation;
            }
            else if (_inputSystemActions.Player.LeftWheelDown.IsPressed())
            {
                leftWheel.transform.rotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, leftWheel.right) *
                                               leftWheel.transform.rotation;
            }

            if (_inputSystemActions.Player.RightWheelUp.IsPressed())
            {
                rightWheel.transform.rotation = Quaternion.AngleAxis(rotSpeed * Time.deltaTime, rightWheel.right) *
                                                rightWheel.transform.rotation;
            }
            else if (_inputSystemActions.Player.RightWheelDown.IsPressed())
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

            inputVel = transform.forward * forwardSpeed;

            if (Mathf.Abs(rotAngVel) > speedDiffThreshold)
            {
                inputRot = Quaternion.AngleAxis(rotAngVel * Time.fixedDeltaTime, Vector3.up) *
                           inputRot;
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