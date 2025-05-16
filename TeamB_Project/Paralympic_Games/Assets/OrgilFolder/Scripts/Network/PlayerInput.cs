using System;
using Fusion;
using UnityEngine;

[Serializable]
public struct PlayerInput : INetworkInput
{
    public Quaternion rotation;
    public Vector3 velocity;
}