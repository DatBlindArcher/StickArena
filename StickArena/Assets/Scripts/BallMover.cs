using UnityEngine;

public class BallMover : MonoBehaviour
{
    public float speed;
    private bool sw;

    private void Start()
    {
        sw = false;
        Debug.Log("Hello World!");
    }

    private void Update()
    {
        Vector3 vec = transform.position;

        if (sw)
            vec.x += speed;
        else
            vec.x -= speed;

        if (Mathf.Abs(vec.x) + speed >= 5) sw = !sw;

        transform.position = vec;
    }
}