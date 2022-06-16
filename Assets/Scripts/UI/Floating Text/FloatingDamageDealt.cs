using UnityEngine;

public class FloatingDamageDealt : MonoBehaviour
{
    public float MoveSpeed = 2f;
    private Vector3 movePosition;

    void Start()
    {
        movePosition = transform.position + new Vector3(0f, 3f, 0f);
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePosition, MoveSpeed * Time.deltaTime);
    }
}
