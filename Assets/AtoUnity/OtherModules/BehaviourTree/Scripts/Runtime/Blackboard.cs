using UnityEngine;

namespace AtoGame.TheKiwiCoder.BT
{

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    public abstract class Blackboard : ScriptableObject
    {
       
    }
}