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
        [Header("PC Only")][SerializeField] private float rotSpeed = 3;
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

            if (HasInputAuthority)
            {
                _inputSystemActions.Enable();
                Debug.Log("Registering for player input");
                PlayerInputBehaviour.GetInput = ProvideInput;
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

        public PlayerInput ProvideInput()
        {
            var input = new PlayerInput();
            input.rotation = inputRot;
            input.velocity = inputVel;
            inputVel = Vector3.zero;

            return input;
        }

        private void Update()
        {
            float leftWheelInput = _inputSystemActions.Player.LeftWheel.ReadValue<float>();
            float rightWheelInput = _inputSystemActions.Player.RightWheel.ReadValue<float>();

            leftWheel.transform.rotation =
                Quaternion.AngleAxis(-leftWheelInput * rotSpeed * Time.deltaTime, leftWheel.right) *
                leftWheel.transform.rotation;


            rightWheel.transform.rotation =
                Quaternion.AngleAxis(rightWheelInput * rotSpeed * Time.deltaTime, rightWheel.right) *
                rightWheel.transform.rotation;
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
            inputRot = Quaternion.Slerp(inputRot, Quaternion.AngleAxis(rotAngVel * Time.fixedDeltaTime, Vector3.up) *
                                                  transform.rotation,
                Mathf.Lerp(0, 1, Mathf.Abs(rotAngVel) / speedDiffThreshold));
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