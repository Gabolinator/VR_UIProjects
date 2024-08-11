using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DinoFracture;
using Unity.VisualScripting;
using UnityEngine;
using Cysharp.Threading.Tasks;







/// <summary>
/// Holds the Data relative to one specific collision between two astral bodies 
/// </summary>
[System.Serializable]
public class CollisionData
{
}




/// <summary>
/// Takes care of the handling what happens after the collision. 
/// Choosing the collision regime and what happens with each of those regime.
/// </summary>
public class CollisionManager : MonoBehaviour
{
    static CollisionManager _instance;
    public static CollisionManager Instance => _instance;

    public FractureManager _fractureManager = new FractureManager();

    public List<CollisionData> _unProcessedCollisions;
    public List<CollisionData> _processedCollisions;



    public bool showDebugLog = true;

    //   public  Action<CollisionData> OnImpact => EventBus.OnCollision;
    // public  Action<CollisionData> OnAfterImpact => EventBus.OnCollisionProcessed;


    [SerializeField] private bool _forceDisableCollisions;

    public bool ForceDisableCollisions
    {
        get => _forceDisableCollisions;
        set => _forceDisableCollisions = value;
    }


    public void ExplosionImpact(List<Rigidbody> allFragment, GameObject o, CollisionData collisionData)
    {
        throw new NotImplementedException();
    }
}