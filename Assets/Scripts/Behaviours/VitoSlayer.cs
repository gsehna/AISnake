using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

[CreateAssetMenu(fileName = "Vito Slayer",
                 menuName = "AIBehaviours/Vito Slayer",
                 order = 1)]

public class VitoSlayer : AIBehaviour
{
    [Header("Properties")]
    public OrbBehavior target;
    public Gradient color;
    public float normalSpeed            = 3.5f;
    public float rotationSensitivity    = 300f;
    public float sightRange             = 7.5f;
    public float avoidRange             = 3.5f;
    public float maxRange               = 15f;

    public float frontAngle             = 45f;
    public float sideAngle              = 135f;

    private bool avoiding               = false;
    private Vector2 avoidDirection      = Vector2.zero;

    [Header("Refs")]
    public SnakeMovement snake;
    private Transform orbsHolder;
    private SpriteRenderer renderer;
    private SpriteRenderer eyesRenderer;

    private List<OrbBehavior> orbsSeen;
    private List<Collider2D> walls;

    private List<RaycastHit2D> forwardHits;

    public override void Init(GameObject own, SnakeMovement ownMove)
    {
        color = new Gradient();
        color.colorKeys = new GradientColorKey[]
        {
            new GradientColorKey(Color.red, 0f),
            new GradientColorKey(Color.yellow, 0.2f),
            new GradientColorKey(Color.green, 0.4f),
            new GradientColorKey(Color.cyan, 0.6f),
            new GradientColorKey(Color.blue, 0.8f),
            new GradientColorKey(Color.magenta, 1f)
        };

        snake = ownMove;
        orbsHolder = GameObject.Find("Orbs").transform;
        renderer = snake.GetComponent<SpriteRenderer>();
        eyesRenderer = transform.Find("Eyes").GetComponent<SpriteRenderer>();

        orbsSeen = new List<OrbBehavior>();
        walls = new List<Collider2D>();

        Transform wallHolders = GameObject.Find("Walls").transform;
        foreach (Transform child in wallHolders)
        {
            Collider2D collider = child.GetComponent<Collider2D>();
            if (collider)
            {
                walls.Add(collider);
            }
        }

        forwardHits = new List<RaycastHit2D>();

        transform.parent.name = "Vito Slayer";
    }

    public override void Execute()
    {
        LookForOrbs();
        CastVision();

        if (!target && !avoiding)
        {
            DecideTarget();
            CheckWall();
        }

        if (!avoiding)
        {
            MoveForward();
        }
        else
        {
            Avoid();
        }

        UpdateColors();

        int bodyCount = snake.bodyParts.Count;
        renderer.sortingOrder = bodyCount + 1;
        eyesRenderer.sortingOrder = bodyCount + 2;
    }

    private void MoveForward()
    {
        if (target)
        {
            float angle = Vector2.SignedAngle(Vector2.up, target.transform.position - snake.transform.position);
            snake.transform.rotation = Quaternion.RotateTowards(snake.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), rotationSensitivity * Time.deltaTime);
        }
        transform.Translate(transform.InverseTransformDirection(transform.up) * normalSpeed * Time.deltaTime);
    }

    private void Avoid()
    {
        float angle = Vector2.SignedAngle(Vector2.up, avoidDirection);
        snake.transform.rotation = Quaternion.RotateTowards(snake.transform.rotation, Quaternion.Euler(new Vector3(0, 0, angle)), rotationSensitivity * Time.deltaTime);
        transform.Translate(transform.InverseTransformDirection(transform.up) * normalSpeed * Time.deltaTime);

        if (Vector2.Angle(transform.up, avoidDirection) <= 1)
        {
            avoiding = false;
        }
    }

    private void LookForOrbs()
    {
        foreach (Transform child in orbsHolder)
        {
            OrbBehavior orb = child.GetComponent<OrbBehavior>();
            if (!orbsSeen.Contains(orb) &&
                Vector2.Distance(child.position, transform.position) <= sightRange)
            {
                orbsSeen.Add(orb);
            }
        }
    }

    private void CastVision()
    {
        List<RaycastHit2D> allHits = new List<RaycastHit2D>();

        forwardHits = CastRay(transform.up);
        List<RaycastHit2D> rightHits = CastRay(Quaternion.AngleAxis(15, Vector3.forward) * transform.up);
        List<RaycastHit2D> leftHits = CastRay(Quaternion.AngleAxis(-15, Vector3.forward) * transform.up);
        List<RaycastHit2D> moreRightHits = CastRay(Quaternion.AngleAxis(45, Vector3.forward) * transform.up);
        List<RaycastHit2D> moreLeftHits = CastRay(Quaternion.AngleAxis(-45, Vector3.forward) * transform.up);

        // Add to all
        foreach (RaycastHit2D hit in forwardHits)
        {
            allHits.Add(hit);
        }

        // Add to all
        foreach (RaycastHit2D hit in rightHits)
        {
            allHits.Add(hit);
        }

        // Add to all
        foreach (RaycastHit2D hit in leftHits)
        {
            allHits.Add(hit);
        }

        // Add to all
        foreach (RaycastHit2D hit in moreRightHits)
        {
            allHits.Add(hit);
        }

        // Add to all
        foreach (RaycastHit2D hit in moreLeftHits)
        {
            allHits.Add(hit);
        }

        // Check snake
        foreach (RaycastHit2D hit in allHits)
        {
            SnakeBody body = hit.transform.GetComponent<SnakeBody>();
            if (body &&
                body.transform.parent != transform.parent)
            {
                // Other body
                AvoidSnake(body.head.GetComponent<SnakeMovement>());
                return;
            }

            SnakeMovement head = hit.transform.GetComponent<SnakeMovement>();
            if (head)
            {
                // Other head
                AvoidSnake(head);
                return;
            }
        }
    }

    private void CheckWall()
    {
        foreach (RaycastHit2D hit in forwardHits)
        {
            foreach (Collider2D collider in walls)
            {
                if (hit.collider == collider)
                {
                    avoidDirection = Vector2.Reflect(transform.up, hit.normal);
                    avoiding = true;
                    target = null;
                    return;
                }
            }
        }
    }

    private void AvoidSnake(SnakeMovement snake)
    {
        Vector2 generalDirection = snake.transform.position - transform.position;

        for (int i = 0; i < snake.bodyParts.Count; i++)
        {
            generalDirection += (Vector2)snake.bodyParts[i].position - (Vector2)transform.position;
        }

        avoidDirection = -generalDirection.normalized;
        avoiding = true;
        target = null;
    }

    private List<RaycastHit2D> CastRay(Vector2 direction)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        Physics2D.Raycast(transform.position + (Vector3)direction, direction, filter, hits, avoidRange);

        if (avoiding)
        {
            Debug.DrawLine(transform.position + (Vector3)direction, transform.position + (Vector3)direction + ((Vector3)direction * avoidRange), Color.red);
        }
        else if (!target)
        {
            Debug.DrawLine(transform.position + (Vector3)direction, transform.position + (Vector3)direction + ((Vector3)direction * avoidRange), Color.yellow);
        }
        else
        {
            Debug.DrawLine(transform.position + (Vector3)direction, transform.position + (Vector3)direction + ((Vector3)direction * avoidRange), Color.green);
        }

        return hits;
    }

    private void DecideTarget()
    {
        CleanNullOrbs(orbsSeen);

        if (orbsSeen.Count > 0)
        {
            List<OrbBehavior> orbsInFront   = new List<OrbBehavior>();
            List<OrbBehavior> orbsInLeft    = new List<OrbBehavior>();
            List<OrbBehavior> orbsInRight   = new List<OrbBehavior>();
            List<OrbBehavior> orbsInBack    = new List<OrbBehavior>();

            foreach (OrbBehavior orb in orbsSeen)
            {
                float angle = Vector2.SignedAngle(transform.up, orb.transform.position - transform.position);

                if (angle >= -frontAngle && angle <= frontAngle)
                {
                    orbsInFront.Add(orb);
                }
                else if (angle < -frontAngle && angle >= -sideAngle)
                {
                    orbsInLeft.Add(orb);
                }
                else if (angle > frontAngle && angle <= sideAngle)
                {
                    orbsInRight.Add(orb);
                }
                else
                {
                    orbsInBack.Add(orb);
                }
            }

            List<List<OrbBehavior>> allOrbs = new List<List<OrbBehavior>>();
            allOrbs.Add(orbsInLeft);
            allOrbs.Add(orbsInRight);
            allOrbs.Add(orbsInBack);

            List<OrbBehavior> greaterList = orbsInFront;

            for (int i = 0; i < allOrbs.Count; i++)
            {
                if (allOrbs[i].Count > greaterList.Count)
                {
                    greaterList = allOrbs[i];
                }
            }

            if (greaterList.Count > 0)
            {
                target = GetNearest(greaterList);
            }
        }
    }

    private OrbBehavior GetNearest(List<OrbBehavior> orbs)
    {
        OrbBehavior nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (OrbBehavior orb in orbs)
        {
            float distance = Vector2.Distance(orb.transform.position, transform.position);

            if (distance < nearestDistance)
            {
                nearest = orb;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    private void CleanNullOrbs(List<OrbBehavior> orbs)
    {
        for (int i = orbs.Count - 1; i >= 0; i--)
        {
            if (orbs[i] == null ||
                Vector2.Distance(orbs[i].transform.position, transform.position) > maxRange)
            {
                orbs.RemoveAt(i);
            }
        }
    }

    private void UpdateColors()
    {
        renderer.color = color.Evaluate(0);

        for (int i = 0; i < snake.bodyParts.Count; i++)
        {
            SpriteRenderer renderer = snake.bodyParts[i].GetComponent<SpriteRenderer>();
            renderer.color = color.Evaluate(((i / 10f) + 0.1f) % 1);
        }
    }

    public Transform transform
    {
        get
        {
            return snake.transform;
        }
    }
}