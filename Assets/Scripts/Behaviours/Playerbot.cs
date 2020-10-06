using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehaviours/Playerbot")]
public class Playerbot : AIBehaviour
{


    public override void Init(GameObject own, SnakeMovement ownMove)
    {
        base.Init(own, ownMove);
        ownerMovement.StartCoroutine(UpdateDirEveryXSeconds(timeChangeDir));
    }

    //seria interessante ter um controlador com o colisor que define o mundo pra poder gerar pontos dentro desse colisor

    public override void Execute()
    {
        MoveForward();
    }

    //ia basica, move, muda de direcao e move
    void MoveForward()
    {
        MouseRotationSnake();
        owner.transform.position = Vector2.MoveTowards(owner.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), ownerMovement.speed * Time.deltaTime);
    }

    void MouseRotationSnake()
    {

        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - owner.transform.position;
        direction.z = 0.0f;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, ownerMovement.speed * Time.deltaTime);
    }

    IEnumerator UpdateDirEveryXSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        ownerMovement.StopCoroutine(UpdateDirEveryXSeconds(x));
        randomPoint = new Vector3(
                Random.Range(
                    Random.Range(owner.transform.position.x - 10, owner.transform.position.x - 5),
                    Random.Range(owner.transform.position.x + 5, owner.transform.position.x + 10)
                ),
                Random.Range(
                    Random.Range(owner.transform.position.y - 10, owner.transform.position.y - 5),
                    Random.Range(owner.transform.position.y + 5, owner.transform.position.y + 10)
                ),
                0
            );
        direction = randomPoint - owner.transform.position;
        direction.z = 0.0f;

        ownerMovement.StartCoroutine(UpdateDirEveryXSeconds(x));
    }
}
