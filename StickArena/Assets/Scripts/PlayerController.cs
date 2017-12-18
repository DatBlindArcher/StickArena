using ArcherNetwork;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    public Transform root;
    public Transform stick;
    public Transform weapon;
    public GameObject cam;

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

        if (!isMine)
            Destroy(cam);
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
        if (isMine) return;
        state.timestamp = Time.time + Time.fixedDeltaTime;
        currentstate = nextstate.copy;
        nextstate = state;
    }

    private void FixedUpdate()
    {
        if (!isMine) return;

        // Input
        float currentSpeed = speed;
        currentstate = nextstate.copy;
        Vector2 pos = nextstate.pos;
        Vector2 movement = Vector2.zero;
        
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

        // States
        nextstate.pos = pos + movement;
        nextstate.timestamp = Time.fixedTime + Time.fixedDeltaTime;

        Vector2 mouse = Input.mousePosition;
        float mouseX = mouse.x - Screen.width / 2f;
        float mouseY = mouse.y - Screen.height / 2f;
        nextstate.rot = Mathf.Atan2(mouseY, mouseX) * Mathf.Rad2Deg - 90f;

        Packet packet = new Packet();
        packet.Write(PacketType.State);
        packet.Write(nextstate);
        GameController.instance.SendPacket(SendType.FastButUnreliable, NetworkTarget.Others, packet);
    }
}