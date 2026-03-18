using UnityEngine;

public class CanvasFollow : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        canvas.transform.position = transform.position + new Vector3(0, 1f, 0);
    }
}