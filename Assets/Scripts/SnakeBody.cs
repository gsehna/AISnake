using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    public int myOrder;
    public Transform head;
    public bool beginning = true;

    public Color purple, gold ;
    // Start is called before the first frame update
    void Start()
    {
        //TODO: passar as colors para um scriptable object
        purple = new Color(198.0f / 255.0f, 90.0f / 255.0f, 255.0f / 255.0f);
        gold = new Color(238.0f / 255.0f, 205.0f / 255.0f, 0.0f / 255.0f);

        

        for (int i = 0; i < head.GetComponent<SnakeMovement>().bodyParts.Count; i++)
        {
            if (gameObject == head.GetComponent<SnakeMovement>().bodyParts[i].gameObject)
            {
                myOrder = i;
                if (i % 2 == 0)
                {
                    gameObject.GetComponent<SpriteRenderer>().color = purple;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().color = gold;
                }

               
            }
        }
    }

    private Vector3 movementVelocity;
    [Range(0.0f, 1.0f)]
    public float overTime = 0.115f;

    // Update is called once per frame
    void FixedUpdate()
    {
        int nParts = head.GetComponent<SnakeMovement>().bodyParts.Count;

        if (myOrder == 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, head.position, ref movementVelocity, overTime);
            transform.LookAt(new Vector3(head.transform.position.x, head.transform.position.y, -90.0f));
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, head.GetComponent<SnakeMovement>().bodyParts[myOrder-1].position, ref movementVelocity, overTime);
            transform.LookAt(new Vector3(head.transform.position.x, head.transform.position.y, -90.0f));
        }

        gameObject.GetComponent<SpriteRenderer>().sortingOrder = nParts - myOrder;
    }
}
