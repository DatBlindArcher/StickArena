using ArcherNetwork;
using UnityEngine;

// Disable camera if not mine
public class PlayerController : MonoBehaviour
{
    public float speed;
    public float sprintSpeed;
    public float raycastRadius;
    public float raycastDistance;

    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode sprint;

    public Transform root;
    public Transform stick;
    public Transform weapon;

    private PlayerState currentstate;
    private PlayerState nextstate;
    private Animator anim;
    private Player player;

    public bool isMine
    {
        get
        {
            return player.ID == GameController.instance.player.ID;
        }
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    private void Start()
    {
        anim = stick.GetComponent<Animator>();
        currentstate = nextstate = new PlayerState() { timestamp = Time.time, pos = root.position, cam = Vector3.zero };
    }

    private void Update()
    {
        // Apply State
        float interpolateFactor = (Time.time - currentstate.timestamp) / Time.fixedDeltaTime;
        root.position = Vector3.LerpUnclamped(currentstate.pos, nextstate.pos, interpolateFactor);
        weapon.rotation = Quaternion.LerpUnclamped(Quaternion.Euler(0f, 0f, currentstate.rot), Quaternion.Euler(0f, 0f, nextstate.rot), interpolateFactor);

        // Movement Rotation
        stick.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(currentstate.pos.y - root.position.y, currentstate.pos.x - root.position.x) * Mathf.Rad2Deg);
        anim.SetBool("Moving", Vector3.Distance(currentstate.pos, (Vector2)root.position) > 0f);

        // Setup Between-State
        currentstate.timestamp = Time.time;
        currentstate.pos = root.position;
        currentstate.rot = weapon.rotation.eulerAngles.z;
    }

    public void ReceiveState(PlayerState state)
    {
        currentstate = nextstate.copy;
        nextstate = state;
    }

    private void FixedUpdate()
    {
        if (!isMine) return;

        float currentSpeed = speed;
        currentstate = nextstate.copy;
        Vector2 pos = nextstate.pos;
        Vector2 movement = Vector2.zero;
        
        if (Input.GetKey(forward)) movement.y++;
        if (Input.GetKey(backward)) movement.y--;
        if (Input.GetKey(right)) movement.x++;
        if (Input.GetKey(left)) movement.x--;
        if (Input.GetKey(sprint)) currentSpeed = sprintSpeed;

        RaycastHit2D hit;
        movement = Vector2.ClampMagnitude(movement, 1f) * currentSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(movement.x) > 0f)
        {
            hit = Physics2D.CircleCast(nextstate.pos + new Vector2(movement.x, 0f) * raycastDistance, raycastRadius, new Vector2(movement.x, 0f), movement.x);

            if (hit.transform != null)
            {
                movement.x = 0f;
            }
        }

        if (Mathf.Abs(movement.y) > 0f)
        {
            hit = Physics2D.CircleCast(nextstate.pos + new Vector2(0f, movement.y) * raycastDistance, raycastRadius, new Vector2(0f, movement.y), movement.y);

            if (hit.transform != null)
            {
                movement.y = 0f;
            }
        }

        nextstate.pos = pos + movement;
        nextstate.timestamp = Time.fixedTime + Time.fixedDeltaTime;

        Vector2 mouse = Input.mousePosition;
        float mouseX = mouse.x - Screen.width / 2f;
        float mouseY = mouse.y - Screen.height / 2f;
        nextstate.rot = Mathf.Atan2(mouseY, mouseX) * Mathf.Rad2Deg - 90f;

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.Write(PacketType.State);
        buffer.Write(nextstate);
        GameController.instance.SendPacket(SendType.SlowButReliable, NetworkTarget.Others, buffer);
    }
}