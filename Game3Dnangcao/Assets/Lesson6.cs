using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour
{
    private bool isTurning = true;
    private bool isMoving = true;
    private Vector3 targetPosition = new Vector3(5f, 0f, 0f);
    private float moveSpeed = 3f;

    void Start()
    {
        StartCoroutine(MoveTank());
    }

    IEnumerator MoveTank()
    {
        while (isTurning)
        {
            yield return StartCoroutine(TurnTankCoroutine());
        }
        yield return new WaitForSeconds(1f);
        while (isMoving)
        {
            yield return StartCoroutine(MoveToPositionCoroutine());
        }
        yield return new WaitForSeconds(1f);
        Fire();
    }

    IEnumerator TurnTankCoroutine()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = transform.rotation * Quaternion.Euler(0f, 90f, 0f);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
        transform.rotation = endRotation;
        isTurning = false;
    }

    IEnumerator MoveToPositionCoroutine()
    {
        Vector3 startPosition = transform.position;
        targetPosition = startPosition + transform.forward * 5f;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
    }

    void Fire()
    {
        print("Fire");
    }
}
