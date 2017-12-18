using UnityEngine;

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

    private Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        // Setup
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
        float xs = Mathf.Sign(movement.x);
        float ys = Mathf.Sign(movement.y);

        if (movement.x * xs > 0f)
        {
            hit = Physics2D.BoxCast(transform.position + new Vector3(raycastSize.y / 2f * xs, 0f, 0f), new Vector2(raycastSize.x, raycastSize.y), 0f, new Vector2(movement.x, 0f), movement.x * xs);

            if (hit.transform != null)
            {
                movement.x = hit.point.x - (transform.position.x + raycastSize.y / 2f * xs) - raycastDepth * xs;
            }
        }

        if (movement.y * ys > 0f)
        {
            hit = Physics2D.BoxCast(transform.position + new Vector3(0f, raycastSize.y / 2f * ys, 0f), new Vector2(raycastSize.y, raycastSize.x), 0f, new Vector2(0f, movement.y), movement.y * ys);

            if (hit.transform != null)
            {
                movement.y = hit.point.y - (transform.position.y + raycastSize.y / 2f * ys) - raycastDepth * ys;
            }
        }

        anim.SetBool("Moving", movement.magnitude > 0f);
        transform.position += (Vector3)movement;
    }
}