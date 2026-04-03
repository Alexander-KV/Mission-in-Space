using UnityEngine;

public class SciFiDoor : MonoBehaviour
{
    public Transform LeftDoor;
    public Transform RightDoor;

    public float OpenDistance = 1.5f;
    public float Speed = 3f;

    bool playerNear;
    bool isOpen;

    Vector3 leftClosed;
    Vector3 rightClosed;

    Vector3 leftOpen;
    Vector3 rightOpen;

    void Start()
    {
        leftClosed = LeftDoor.localPosition;
        rightClosed = RightDoor.localPosition;

        // Используем ось родителя (двери)
        Vector3 sideDirection = transform.right;

        leftOpen = leftClosed - sideDirection * OpenDistance;
        rightOpen = rightClosed + sideDirection * OpenDistance;
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        Vector3 targetLeft = isOpen ? leftOpen : leftClosed;
        Vector3 targetRight = isOpen ? rightOpen : rightClosed;

        LeftDoor.localPosition = Vector3.Lerp(
            LeftDoor.localPosition,
            targetLeft,
            Time.deltaTime * Speed
        );

        RightDoor.localPosition = Vector3.Lerp(
            RightDoor.localPosition,
            targetRight,
            Time.deltaTime * Speed
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            Debug.Log("Player near door");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}