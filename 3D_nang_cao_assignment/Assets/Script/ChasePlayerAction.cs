using System;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI; // Cần thêm cái này để dùng NavMeshAgent
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChasePlayer", story: "[Agent] chases [Target]", category: "Action", id: "86eaab5d9321b877dfde8460c0ee4e3d")]
public partial class ChasePlayerAction : Action
{
    // Khai báo biến để nối với Behavior Graph
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    // Thêm biến tốc độ và khoảng cách dừng (có thể chỉnh trong Editor)
    [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(5.0f);
    [SerializeReference] public BlackboardVariable<float> StopDistance = new BlackboardVariable<float>(1.5f);

    private NavMeshAgent navAgent;

    protected override Status OnStart()
    {
        // Kiểm tra xem có đủ dữ liệu chưa
        if (Agent.Value == null || Target.Value == null)
        {
            return Status.Failure;
        }

        // Lấy NavMeshAgent từ con AI
        navAgent = Agent.Value.GetComponent<NavMeshAgent>();
        if (navAgent == null)
        {
            return Status.Failure;
        }

        // Cài đặt tốc độ chạy
        navAgent.speed = Speed.Value;
        navAgent.stoppingDistance = StopDistance.Value;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Agent.Value == null || Target.Value == null || navAgent == null)
        {
            return Status.Failure;
        }

        // --- QUAN TRỌNG: Cập nhật vị trí liên tục ---
        navAgent.SetDestination(Target.Value.transform.position);

        // Kiểm tra nếu đến gần rồi thì báo thành công (để chuyển sang đánh)
        if (!navAgent.pathPending && navAgent.remainingDistance <= StopDistance.Value)
        {
            return Status.Success;
        }

        // Chưa đến nơi thì chạy tiếp
        return Status.Running;
    }

    protected override void OnEnd()
    {
        // Khi thoát khỏi hành động này (ví dụ chuyển sang đánh), thì dừng di chuyển
        if (navAgent != null && navAgent.isActiveAndEnabled)
        {
            navAgent.ResetPath();
        }
    }
}