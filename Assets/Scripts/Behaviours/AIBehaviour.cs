using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : ScriptableObject
{
    public Vector3 direction;
    public GameObject owner;
    public SnakeMovement ownerMovement;

    public float timeChangeDir;
    public Vector3 randomPoint;
    public virtual void Init(GameObject own, SnakeMovement ownMove)
    {
        direction = new Vector3(0, 0, 0);
        randomPoint = new Vector3(0, 0, 0);

        owner = own;
        ownerMovement = ownMove;
    }

    public virtual void Execute()
    {

    }
}
