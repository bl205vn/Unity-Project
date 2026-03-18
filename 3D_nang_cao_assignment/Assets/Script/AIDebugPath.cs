using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Debug script cho Tiger AI - hiển thị thông tin đường đi và khoảng cách đến Player.
/// </summary>
public class AIDebugPath : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Debug Settings")]
    public Color pathColor = Color.yellow;
    public Color destinationColor = Color.red;
    public bool showGUI = true;

    [Header("Player Reference (kéo Player từ Hierarchy vào đây)")]
    public GameObject player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.LogError($"[AIDebug] Không tìm thấy NavMeshAgent trên {gameObject.name}!");

        if (player == null)
            Debug.LogWarning("[AIDebug] Player chưa được gán vào script! Kéo Player từ Hierarchy vào ô Player.");
    }

    private void Update()
    {
        if (agent == null) return;

        // Vẽ đường path
        if (agent.hasPath)
        {
            var corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
                Debug.DrawLine(corners[i], corners[i + 1], pathColor);

            Vector3 dest = agent.destination;
            Debug.DrawLine(dest + Vector3.up * 2, dest + Vector3.down * 0.5f, destinationColor);
            Debug.DrawLine(dest + Vector3.left, dest + Vector3.right, destinationColor);
        }

        // Vẽ đường đến Player
        if (player != null)
        {
            Debug.DrawLine(transform.position + Vector3.up, player.transform.position + Vector3.up, Color.cyan);
        }
    }

    private void OnGUI()
    {
        if (!showGUI || agent == null) return;

        string navInfo = agent.isOnNavMesh ? "On NavMesh" : "NOT ON NAVMESH!";
        string pathInfo = agent.hasPath ? $"OK ({agent.path.corners.Length} corners)" : "NO PATH";

        // Thông tin Player
        string playerInfo;
        string distanceInfo;
        if (player != null)
        {
            float dist = Vector3.Distance(transform.position, player.transform.position);
            playerInfo = player.name;
            distanceInfo = $"{dist:F2}m";
        }
        else
        {
            playerInfo = "NULL (chưa gán!)";
            distanceInfo = "---";
        }

        string info =
            $"=== AI Debug: {gameObject.name} ===\n" +
            $"NavMesh: {navInfo}\n" +
            $"Path: {pathInfo}\n" +
            $"Speed: {agent.velocity.magnitude:F2}\n" +
            $"---\n" +
            $"Player: {playerInfo}\n" +
            $"Distance to Player: {distanceInfo}\n" +
            $"AttackRange: 2 | ChaseRange: 10";

        GUIStyle style = new GUIStyle(GUI.skin.box)
        {
            fontSize = 14,
            alignment = TextAnchor.UpperLeft,
            normal = { textColor = Color.white }
        };

        GUI.Box(new Rect(10, 10, 400, 180), info, style);
    }
}
