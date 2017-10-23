﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float sprintSpeed;

    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;

    public KeyCode sprint;


    private PlayerState currentstate;
    private PlayerState nextstate;

    private void Start()
    {
        currentstate = nextstate = new PlayerState() { timestamp = Time.time, pos = transform.position, cam = Vector3.zero };
    }

    private void Update()
    {
        float interpolateFactor = (Time.time - currentstate.timestamp) / Time.fixedDeltaTime;
        transform.position = Vector3.LerpUnclamped(currentstate.pos, nextstate.pos, interpolateFactor);
        currentstate.timestamp = Time.time;
        currentstate.pos = transform.position;
    }

    private void FixedUpdate()
    {
        float currentSpeed = speed;
        currentstate = nextstate;
        Vector2 pos = nextstate.pos;
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(sprint)) currentSpeed = sprintSpeed;

        if (Input.GetKey(forward)) movement.y++;
        if (Input.GetKey(backward)) movement.y--;
        if (Input.GetKey(right)) movement.x++;
        if (Input.GetKey(left)) movement.x--;

        pos += Vector2.ClampMagnitude(movement, 1f) * currentSpeed * Time.fixedDeltaTime;
        nextstate.pos = pos;
        nextstate.timestamp = Time.fixedTime;
    }
}