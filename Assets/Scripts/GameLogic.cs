using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public List<GameObject> snakes = new List<GameObject>();
    public List<AIBehaviour> behaviors = new List<AIBehaviour>();

    public int OrbsPerSnake, NumberSnakes;
    public float OrbSpawnTime;
    public GameObject orbPreFab;

    float minX, minY, maxX, maxY;
    int selectedId;

    // Start is called before the first frame update
    public GameObject snakePrefab;
    void Start()
    {
        selectedId = 0;
        BoxCollider2D col = GetComponent<BoxCollider2D>();

        minX = col.bounds.min.x;
        minY = col.bounds.min.y;
        maxX = col.bounds.max.x;
        maxY = col.bounds.max.y;


        // Cria 5 SnakeBots em posições aleatórias
        //aqui depois vai precisar da lista dos behaviours que vai carregar da pasta de recursos e
        // provelmente spawnar um por behaviour presente na pasta e alocar como behaviour
        for (int i = 0; i < NumberSnakes; i++)
        {
            //Vector3 randomPosition = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), 0.0f);
            Vector3 randomPosition;
            if (i == selectedId)
            {
                randomPosition = new Vector3(0.0f,0.0f,0.0f);
            }
            else
            {
                randomPosition = new Vector3(
                Random.Range(
                   minX, maxX
                ),
                Random.Range(
                    minY, maxY
                ),
                0
            );
            }
 
            GameObject newSnake = Instantiate(snakePrefab, randomPosition, Quaternion.identity) as GameObject;
            newSnake.name = "SnakeBot" + i.ToString();

            snakes.Add(newSnake);

            if (i == selectedId)
            {
                snakes[i].GetComponentInChildren<SnakeMovement>().SetBehaviour(behaviors[1]);
            }
            else
            {
                snakes[i].GetComponentInChildren<SnakeMovement>().SetBehaviour(behaviors[0]);
            }
        }
        snakes[selectedId].GetComponentInChildren<SnakeMovement>().selected = true;
       
        StartCoroutine("SpawnXOrbsEveryYSeconds", OrbSpawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        //verifica se alguém morreu para remover da lista
        for (int i = 0; i < snakes.Count; i++)
        {
            if (snakes[i].GetComponentInChildren<SnakeMovement>().isDead)
            {
                Destroy(snakes[i]);
                snakes.RemoveAt(i);
                Debug.Log("Morreu a Snake " + i.ToString());

                if (i == selectedId) //a priori deve sempre sobrar 1 cobrinha (a menos que as duas últimas colidam de cabeça)
                {
                    selectedId = 0;
                }
           
            }
        }
        CheckInput();
    }

    IEnumerator SpawnXOrbsEveryYSeconds(float timeBetweenSpawns)
    {
        yield return new WaitForSeconds(timeBetweenSpawns);
        StopCoroutine("SpawnXOrbsEveryYSeconds");

        for (int i = 0; i < OrbsPerSnake * snakes.Count; ++i)
        {
            Vector3 randomOrbPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0);
            GameObject newOrb = Instantiate(orbPreFab, randomOrbPos, Quaternion.identity);
            GameObject orbParent = GameObject.Find("Orbs");
            newOrb.transform.parent = orbParent.transform;
        }

        StartCoroutine("SpawnXOrbsEveryYSeconds", OrbSpawnTime);

    }

    void CheckInput()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            selectNext();
        }
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            selectPrevious();
        }
    }

    

    void selectPrevious()
    {
        snakes[selectedId].GetComponentInChildren<SnakeMovement>().selected = false;
        if (selectedId > 0) selectedId--;
        else selectedId = snakes.Count - 1;
        snakes[selectedId].GetComponentInChildren<SnakeMovement>().selected = true;
        Debug.Log("SELECTED ID = " + selectedId.ToString());

    }

    void selectNext()
    {
            snakes[selectedId].GetComponentInChildren<SnakeMovement>().selected = false;
            selectedId = (selectedId + 1) % snakes.Count;
            snakes[selectedId].GetComponentInChildren<SnakeMovement>().selected = true;
            Debug.Log("SELECTED ID = " + selectedId.ToString());

    }
    
}
