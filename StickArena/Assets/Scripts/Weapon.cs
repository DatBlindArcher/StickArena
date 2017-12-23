using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform center;
    private Animation anim;

    private void Start()
    {
        anim = GetComponent<Animation>();
    }

    public void Attack()
    {
        anim.Stop();
        anim.Play();
    }

    public void ActualHit()
    {
        // Raycast and draw health
        // Send data to others
    }
}