using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// FSM bổ sung chức năng cho Quái (Bài tập 3 + 4 - Lab 4).
/// - Bài 3: Khi di chuyển được 1/3 quãng đường: Random (hoặc test: Luôn nhảy / Luôn tăng tốc) 1 bước nhảy HOẶC tăng tốc gấp đôi trong 2 giây.
/// - Bài 4: NavMesh Link (Area Type: Jump): nếu Custom Jump bật, quái nhảy parabol qua link; không thì dùng Auto Traverse (chạy thẳng). Gọi SetDestination chỉ khi agent enabled và on NavMesh để tránh lỗi.
/// Gắn vào Sphere (1), gán Target. Có thể tắt AINavigationDemo2.
/// </summary>
public class AIFSM : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Bài 3 - Test (khi đạt 1/3 quãng đường)")]
    [Tooltip("Tỉ lệ quãng đường để kích hoạt (0.33 = 1/3, 0.5 = 1/2)")]
    [Range(0.1f, 0.9f)]
    public float triggerRatio = 0.333f;

    [Tooltip("Random = 50% nhảy / 50% tăng tốc. Always Jump / Always SpeedBoost để test từng loại.")]
    public TriggerMode triggerMode = TriggerMode.Random;

    [Header("Tùy chọn nhảy (Bài 3: khi Random chọn Jump)")]
    [Tooltip("Độ cao nhảy (đơn vị world)")]
    public float jumpHeight = 2f;
    [Tooltip("Thời gian nhảy (giây)")]
    public float jumpDuration = 1f;

    [Header("Bài 4 - NavMesh Link (nhảy qua khe)")]
    [Tooltip("Bật: quái nhảy parabol qua NavMesh Link. Cần tắt 'Auto Traverse Off Mesh Link' trên NavMeshAgent (script sẽ tắt giúp nếu bật).")]
    public bool customJumpOverNavMeshLink = true;
    [Tooltip("Thời gian bay từ Start tới End của link (giây)")]
    public float linkTraverseDuration = 0.6f;

    public enum TriggerMode { Random, AlwaysJump, AlwaysSpeedBoost }

    private NavMeshAgent agent;
    private float defaultSpeed;
    
    // Bài 3: Lưu tổng quãng đường ban đầu (chỉ tính 1 lần khi có path hợp lệ đầu tiên)
    private float initialTotalDistance;
    private bool initialDistanceComputed;
    private float realDistanceTraveled;
    private Vector3 lastPosition;
    private bool hasTriggeredOneThird;

    // FSM: Move, Jump, SpeedBoost, TraverseLink (Bài 4: đang nhảy qua NavMesh Link)
    private enum State { Move, Jump, SpeedBoost, TraverseLink }
    private State currentState;

    private float speedBoostTimer;

    private float debugTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("AIFSM: Cần NavMeshAgent trên cùng GameObject.");
            return;
        }
        defaultSpeed = agent.speed;

        initialTotalDistance = 0f;
        initialDistanceComputed = false;
        realDistanceTraveled = 0f;
        lastPosition = transform.position;
        hasTriggeredOneThird = false;
        currentState = State.Move;

        if (customJumpOverNavMeshLink)
            agent.autoTraverseOffMeshLink = false;

        if (target != null) SafeSetDestination(target.position);
    }

    void Update()
    {
        if (agent == null || target == null) return;

        // Chỉ cập nhật quãng đường khi đang ở state Move (không tính khi đang nhảy qua khe - Bài 4)
        if (currentState == State.Move)
        {
            float step = Vector3.Distance(transform.position, lastPosition);
            // Chỉ cộng nếu step hợp lý (tránh teleport quá xa gây sai số lớn)
            if (step > 0.0001f && step < 10f) 
            {
                realDistanceTraveled += step;
            }
        }
        lastPosition = transform.position;

        // Debug info every 1 second
        debugTimer += Time.deltaTime;
        if (debugTimer > 1f)
        {
            debugTimer = 0f;
            if (initialDistanceComputed)
            {
                float ratio = (initialTotalDistance > 0.001f) ? (realDistanceTraveled / initialTotalDistance) : 0f;
                Debug.Log($"[AIFSM] Traveled: {realDistanceTraveled:F2} / {initialTotalDistance:F2} = {ratio:P0}, TriggerAt: {triggerRatio:P0}, Triggered: {hasTriggeredOneThird}");
            }
        }

        switch (currentState)
        {
            case State.Move:
                OnUpdate_Move();
                break;
            case State.Jump:
                // Coroutine xử lý; không gọi SetDestination vì agent.enabled = false → tránh lỗi
                break;
            case State.TraverseLink:
                // Coroutine xử lý nhảy qua NavMesh Link
                break;
            case State.SpeedBoost:
                OnUpdate_SpeedBoost();
                break;
        }
    }

    /// <summary> Gọi SetDestination chỉ khi agent enabled và on NavMesh để tránh lỗi "can only be called on an active agent that has been placed on a NavMesh". </summary>
    void SafeSetDestination(Vector3 pos)
    {
        if (agent == null || !agent.enabled) return;
        if (!agent.isOnNavMesh) return;
        agent.SetDestination(pos);
    }

    void OnUpdate_Move()
    {
        // Kiểm tra an toàn: Agent phải đang bật và nằm trên NavMesh mới tính toán được
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        // Bài 4: Đang trên NavMesh Link → nhảy parabol (nếu bật) hoặc bỏ qua logic 1/3
        if (agent.isOnOffMeshLink)
        {
            if (customJumpOverNavMeshLink)
                TransitionTo(State.TraverseLink);
            return;
        }

        if (target != null) SafeSetDestination(target.position);

        if (agent.pathPending) return;

        // Bài 3: Tính tổng quãng đường ban đầu CHỈ MỘT LẦN khi có path hợp lệ đầu tiên
        if (!initialDistanceComputed && agent.hasPath && agent.path.status == NavMeshPathStatus.PathComplete)
        {
            if (agent.path.corners != null && agent.path.corners.Length >= 2)
            {
                initialTotalDistance = 0f;
                for (int i = 0; i < agent.path.corners.Length - 1; i++)
                    initialTotalDistance += Vector3.Distance(agent.path.corners[i], agent.path.corners[i + 1]);
                initialDistanceComputed = true;
                Debug.Log($"[AIFSM] Initial Total Distance calculated: {initialTotalDistance:F2}. Will trigger at {triggerRatio:P0} = {initialTotalDistance * triggerRatio:F2} units.");
            }
        }

        // Chỉ kiểm tra trigger khi đã tính được tổng quãng đường ban đầu
        if (!initialDistanceComputed || initialTotalDistance < 0.1f) return;

        // Điều kiện chuyển: đã đi >= triggerRatio quãng đường và chưa kích hoạt lần nào
        if (realDistanceTraveled >= initialTotalDistance * triggerRatio && !hasTriggeredOneThird)
        {
            hasTriggeredOneThird = true;
            Debug.Log($"[AIFSM] TRIGGER! Traveled {realDistanceTraveled:F2} >= {initialTotalDistance * triggerRatio:F2} ({triggerRatio:P0} of {initialTotalDistance:F2})");
            switch (triggerMode)
            {
                case TriggerMode.AlwaysJump:    TransitionTo(State.Jump); break;
                case TriggerMode.AlwaysSpeedBoost: TransitionTo(State.SpeedBoost); break;
                default: if (Random.value < 0.5f) TransitionTo(State.Jump); else TransitionTo(State.SpeedBoost); break;
            }
        }
    }

    void OnUpdate_SpeedBoost()
    {
        if (target != null) SafeSetDestination(target.position);
        speedBoostTimer -= Time.deltaTime;
        if (speedBoostTimer <= 0f)
        {
            agent.speed = defaultSpeed;
            TransitionTo(State.Move);
        }
    }

    void TransitionTo(State next)
    {
        currentState = next;

        switch (next)
        {
            case State.Jump:
                OnEnter_Jump();
                break;
            case State.SpeedBoost:
                OnEnter_SpeedBoost();
                break;
            case State.TraverseLink:
                OnEnter_TraverseLink();
                break;
            case State.Move:
                break;
        }
    }

    void OnEnter_Jump()
    {
        Debug.Log("AI FSM: Enter Jump State");
        StartCoroutine(JumpCoroutine());
    }

    void OnEnter_SpeedBoost()
    {
        Debug.Log("AI FSM: Enter SpeedBoost State");
        agent.speed = defaultSpeed * 2f;
        speedBoostTimer = 2f;
    }

    void OnEnter_TraverseLink()
    {
        StartCoroutine(TraverseLinkCoroutine());
    }

    IEnumerator TraverseLinkCoroutine()
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 endPos = data.endPos;
        Vector3 startPos = transform.position;
        agent.enabled = false;

        float elapsed = 0f;
        while (elapsed < linkTraverseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / linkTraverseDuration);
            // Parabol: lerp start->end, cao độ 4*t*(1-t)*jumpHeight
            Vector3 pos = Vector3.Lerp(startPos, endPos, t) + Vector3.up * (4f * t * (1f - t) * jumpHeight);
            transform.position = pos;
            yield return null;
        }

        transform.position = endPos;
        agent.enabled = true;
        agent.Warp(endPos);
        agent.CompleteOffMeshLink();
        if (target != null) SafeSetDestination(target.position);
        TransitionTo(State.Move);
    }

    IEnumerator JumpCoroutine()
    {
        Vector3 startPos = transform.position;

        agent.enabled = false;

        float elapsed = 0f;
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / jumpDuration);
            // Parabol: đỉnh tại t=0.5
            float height = 4f * t * (1f - t) * jumpHeight;
            transform.position = startPos + Vector3.up * height;
            yield return null;
        }

        transform.position = startPos;
        agent.enabled = true;
        agent.Warp(transform.position);
        if (target != null) SafeSetDestination(target.position);
        TransitionTo(State.Move);
    }
}
