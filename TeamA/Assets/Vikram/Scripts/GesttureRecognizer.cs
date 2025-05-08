using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Hands;

public class GestureRecognizer : MonoBehaviour
{
    private XRHandSubsystem handSubsystem;

    void Start()
    {
        // Get the XR Hand Subsystem
        List<XRHandSubsystem> handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);
        if (handSubsystems.Count > 0)
        {
            handSubsystem = handSubsystems[0];
        }
        else
        {
            Debug.LogError("No XR Hand Subsystem found. Hand tracking may not be supported.");
        }
    }

    void Update()
    {
        if (handSubsystem == null || !handSubsystem.running)
            return;

        // Update hand tracking data
        handSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);

        // Check left and right hands
        XRHand leftHand = handSubsystem.leftHand;
        XRHand rightHand = handSubsystem.rightHand;

        if (leftHand.isTracked)
            RecognizeGesture(leftHand, "Left");
        if (rightHand.isTracked)
            RecognizeGesture(rightHand, "Right");
    }

    void RecognizeGesture(XRHand hand, string handSide)
    {
        // Get joint positions (assuming wrist as palm center for simplicity)
        if (hand.GetJoint(XRHandJointID.Wrist).TryGetPose(out Pose wristPose) &&
            hand.GetJoint(XRHandJointID.ThumbTip).TryGetPose(out Pose thumbTipPose) &&
            hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out Pose indexTipPose) &&
            hand.GetJoint(XRHandJointID.MiddleTip).TryGetPose(out Pose middleTipPose) &&
            hand.GetJoint(XRHandJointID.RingTip).TryGetPose(out Pose ringTipPose) &&
            hand.GetJoint(XRHandJointID.LittleTip).TryGetPose(out Pose pinkyTipPose))
        {
            Vector3 wristPos = wristPose.position;
            Vector3 thumbTip = thumbTipPose.position;
            Vector3 indexTip = indexTipPose.position;
            Vector3 middleTip = middleTipPose.position;
            Vector3 ringTip = ringTipPose.position;
            Vector3 pinkyTip = pinkyTipPose.position;

            // Calculate distances from wrist to finger tips
            float distThumb = Vector3.Distance(wristPos, thumbTip);
            float distIndex = Vector3.Distance(wristPos, indexTip);
            float distMiddle = Vector3.Distance(wristPos, middleTip);
            float distRing = Vector3.Distance(wristPos, ringTip);
            float distPinky = Vector3.Distance(wristPos, pinkyTip);

            // Define thresholds (adjust these based on testing)
            float closedThreshold = 0.1f; // meters
            float openThreshold = 0.15f;  // meters

            // Detect closed fist: all finger tips close to wrist
            if (distThumb < closedThreshold && distIndex < closedThreshold &&
                distMiddle < closedThreshold && distRing < closedThreshold &&
                distPinky < closedThreshold)
            {
                Debug.Log($"{handSide} Hand: Closed fist detected");
            }
            // Detect open hand: all finger tips far from wrist
            else if (distThumb > openThreshold && distIndex > openThreshold &&
                     distMiddle > openThreshold && distRing > openThreshold &&
                     distPinky > openThreshold)
            {
                Debug.Log($"{handSide} Hand: Open hand detected");
            }
        }
    }
}