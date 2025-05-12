// using UnityEngine;
// using System.Collections.Generic;
// using System.Collections;
// using UnityEditor.Experimental.GraphView;

// public class TypingArea : MonoBehaviour
// {

//     public GameObject leftHand;
//     public GameObject RightHand;
//     public GameObject leftTypingHand;
//     public GameObject RightTypingHand;
   
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     private void OnTriggerEnter(Collider other)
//     {
//         GameObject hand = other.GetComponent<OVRGrabber>().gameObject;
//         if (hand == null) return;
//         if (hand == leftHand)
//         {
//             leftTypingHand.SetActive(true);

//         }
//         else if (hand == RightHand)
//         {
//             RightTypingHand.SetActive(true);
//         }
        
//     }

//     // Update is called once per frame
//     private void OnTriggerExit(Collider other)
//     {
//         GameObject hand = other.GetComponent<OVRGrabber>().gameObject;
//         if (hand == null) return;
//         if (hand == leftHand)
//         {
//             leftTypingHand.SetActive(false);

//         }
//         else if (hand == RightHand)
//         {
//             RightTypingHand.SetActive(false);
//         }
//     }
// }
