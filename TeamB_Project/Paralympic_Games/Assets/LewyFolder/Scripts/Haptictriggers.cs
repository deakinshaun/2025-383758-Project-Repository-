using Oculus.Interaction;
using Oculus.Interaction.Input;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Haptictriggers : MonoBehaviour
{

    [Range(0, 2.5f)]
    public float duration;
    [Range(0, 1)]
    public float frequency;
    [Range(0, 1)]
    public float amplitude;

    public GrabInteractable grabInteractable;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabInteractable.WhenSelectingInteractorAdded.Action += WhenSelectingInteractorAdded_Action;
    }

    // Update is called once per frame
    private void WhenSelectingInteractorAdded_Action(GrabInteractor obj)
    {
        ControllerRef controllerRef = obj.GetComponent<ControllerRef>();
        if (controllerRef)
        {

            if (controllerRef.Handedness == Handedness.Right)
                TriggerHaptics(OVRInput.Controller.RTouch);
            else
                TriggerHaptics(OVRInput.Controller.LTouch);

        }

    }


    public void TriggerHaptics(OVRInput.Controller controller)
    {
        StartCoroutine(TriggerHapticsRoutine(controller));
    }

        public IEnumerator TriggerHapticsRoutine(OVRInput.Controller controller)
        {
            OVRInput.SetControllerVibration(frequency, amplitude, controller);
            yield return new WaitForSeconds(duration);
            OVRInput.SetControllerVibration(0, 0, controller);
        }
    
}
