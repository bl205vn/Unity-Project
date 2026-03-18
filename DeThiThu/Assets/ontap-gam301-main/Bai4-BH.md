## Phần 2: Behavior Graph

### Cài gói com.unity.behavior
 
### Tạo Graph FSM cho Cube

Chuẩn bị Scene

Tạo các object sau:

- Cube (AI)
- Player (Capsule)
- PointA (Empty)
- PointB (Empty)


Tạo file graph
Gán cho Cube:  Add Component → Behavior Agent --> chọn file graph vừa tạo

Bước 1 — Tạo biến

Trong Graph → Blackboard / Variables

Tạo:

PointA (Transform)

PointB (Transform)

Player (Transform)

Speed (float) = 2

RotateSpeed (float) = 90

Distance (float) = 5

Bước 2 - Tạo node "Navigate to target" , agent là self, target là PointA. Sau đó kéo mũi tên từ Onstart sang node navigate này

Bước 3 - Sang inspector của Cube --> gán các đối tượng vào thuộc tính ở component Behavior Graph

Bước 4: Chạy thử game xem cube có di chuyển không, cube di chuyển đến vị trí A là ok.

Bước 5: Tiếp theo tạo node "Navigate to target" cho tới vị trí PointB, sau đó chạy lại game sẽ thấy cube chạy đi chạy lại giữa 2 point.

Bước 6: Tạo action tuỳ chỉnh tự xoay

Tạo action bằng cách: Kích phải chuột lên màn hình graph --> create new --> action --> Nhập tên cho Action (ví dụ: Xoay Cube) --> nhập mô tả cho action ---> cửa sổ code hiện ra --> 

Khai báo thuộc tính trong phạm vi class: 
```
[SerializeReference] public BlackboardVariable<Transform> Self;
    [SerializeReference] public BlackboardVariable<float> RotateSpeed;

```
Sửa hàm update như sau:
```
  protected override Status OnUpdate()
    {
         Self.Value.Rotate(Vector3.up, RotateSpeed.Value * Time.deltaTime);
        return Status.Running; // giữ Running để Selector không chuyển
     }
```



Bước 7: Sửa lại sơ đồ như sau:
```

[Conditional Branch]
  Condition: Player Is Near
     |              |
  [True]          [False]
     |              |
[Rotate Self]  [Navigate PointA]
                    |
               [Navigate PointB]
```
