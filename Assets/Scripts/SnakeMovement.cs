using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    public List<Transform> bodyParts = new List<Transform>();

    public Transform head;
    public GameObject Eyes;
    public AIBehaviour behave;

    public bool selected;
    public bool isDead;

  
    private Vector3 direction;
    public float speed;
    public float speedWalking = 3.5f, speedRunning = 7.0f;


    // Start is called before the first frame update
    void Start()
    {
        head = transform;
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBehaviour(AIBehaviour behaviour)
    {
        behave = Instantiate(behaviour);
        behave.Init(this.gameObject, this);
    }

    
    void FixedUpdate()
    {
        
        behave.Execute();

        if (selected)
            CameraFollow();


    }

    public bool isRunning = false;
   
    [Range(0.0f, 1.0f)]
    public float smoothTime = 0.05f;

    void CameraFollow()
    {

        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        Vector3 cameraVelocity = Vector3.zero;
        camera.position = Vector3.SmoothDamp(camera.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10), ref cameraVelocity, smoothTime);
    }

    public Transform bodyObject;
    void OnTriggerEnter2D(Collider2D other)
    {
  
        if (other.gameObject.transform.tag == "Body")
        {
          
            if (transform.parent.name != other.gameObject.transform.parent.name)
            {
                isDead = true;
                    for (int i = 0; i < bodyParts.Count; i++)
                    {
                        Destroy(bodyParts[i].gameObject);
                        Destroy(bodyParts[i]);
                    }
                    //Destroy(this.gameObject);

            }
           
        }

        if (other.transform.tag == "Orb")
        {

            Destroy(other.gameObject);

            //Adiciona uma parte do corpo no final
            Vector3 currentPos;
            if (bodyParts.Count == 0)
            {
                currentPos = transform.position;               
            }
            else
            {
                currentPos = bodyParts[bodyParts.Count - 1].position;
            }
            CreateNewPart(currentPos);
        }
    }

    void CreateNewPart(Vector3 currentPos)
    {
        Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;
        newBodyPart.parent = transform.parent;
        bodyParts.Add(newBodyPart.transform);

        int nParts = head.GetComponent<SnakeMovement>().bodyParts.Count;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = nParts;
        Eyes.GetComponent<SpriteRenderer>().sortingOrder = nParts + 1;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
       
    }



}
