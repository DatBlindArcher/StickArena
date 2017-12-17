using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BallMover : MonoBehaviour
{
    public float speed;
    public float sprintSpeed;
    public float raycastDepth;
    public Vector2 raycastSize;

    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode sprint;

    public Vector2 movement;

    private LineRenderer box;

    private void Start()
    {
        box = GetComponent<LineRenderer>();
    }

    private void FixedUpdate()
    {
        float currentSpeed = speed;
        movement = Vector2.zero;

        // Input
        if (Input.GetKey(forward)) movement.y++;
        if (Input.GetKey(backward)) movement.y--;
        if (Input.GetKey(right)) movement.x++;
        if (Input.GetKey(left)) movement.x--;
        if (Input.GetKey(sprint)) currentSpeed = sprintSpeed;

        // Physics
        RaycastHit2D hit;
        movement = Vector2.ClampMagnitude(movement, 1f) * currentSpeed * Time.fixedDeltaTime;
        float xs = -Mathf.Sign(movement.x);
        float ys = -Mathf.Sign(movement.y);
        float xd = raycastDepth * -xs;
        float yd = raycastDepth * -ys;

        if (Mathf.Abs(movement.x) > 0f)
        {
            hit = Physics2D.BoxCast(transform.position, raycastSize, 0f, new Vector2(movement.x, 0f), movement.x + xd);

            if (hit.transform != null)
            {
                movement.x = 0f;
                Debug.Log(hit.transform.name);
            }
        }

        if (Mathf.Abs(movement.y) > 0f)
        {
            hit = Physics2D.BoxCast(transform.position, raycastSize, 0f, new Vector2(0f, movement.y), movement.y + yd);

            if (hit.transform != null)
            {
                movement.y = 0f;
                Debug.Log(hit.transform.name);
            }
        }

        // Draw Boxes
        if (Input.GetKey(forward) || Input.GetKey(backward) || Input.GetKey(left) || Input.GetKey(right))
        {
            List<Vector3> points = new List<Vector3>();

            if (Input.GetKey(left) || Input.GetKey(right))
            {
                // Right corner
                points.Add((Vector2)transform.position + new Vector2(raycastSize.x, raycastSize.y) / 2f * xs);
                points.Add((Vector2)transform.position + new Vector2(raycastSize.x, -raycastSize.y) / 2f * xs);

                // Left corner
                points.Add((Vector2)transform.position + new Vector2(-raycastSize.x / 2f + (movement.x + xd) * xs, -raycastSize.y / 2f) * xs);
                points.Add((Vector2)transform.position + new Vector2(-raycastSize.x / 2f + (movement.x + xd) * xs, raycastSize.y / 2f) * xs);

                // Close box
                points.Add((Vector2)transform.position + new Vector2(raycastSize.x, raycastSize.y) / 2f * xs);
            }

            if (Input.GetKey(forward) || Input.GetKey(backward))
            {
                // Right corner
                points.Add((Vector2)transform.position + new Vector2(raycastSize.x, raycastSize.y) / 2f * ys);
                points.Add((Vector2)transform.position + new Vector2(-raycastSize.x, raycastSize.y) / 2f * ys);

                // Left corner
                points.Add((Vector2)transform.position + new Vector2(-raycastSize.x / 2f, -raycastSize.y / 2f + (movement.y + yd) * ys) * ys);
                points.Add((Vector2)transform.position + new Vector2(raycastSize.x / 2f, -raycastSize.y / 2f + (movement.y + yd) * ys) * ys);

                // Close box
                points.Add((Vector2)transform.position + new Vector2(raycastSize.x, raycastSize.y) / 2f * ys);
            }

            box.positionCount = points.Count;
            box.SetPositions(points.ToArray());
        }

        else
        {
            box.positionCount = 0;
            box.SetPositions(new Vector3[0]);
        }

        if (Input.GetKeyDown(KeyCode.Space))
            transform.position += (Vector3)movement;
    }
}