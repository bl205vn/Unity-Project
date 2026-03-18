using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Đối thoại")]
    [SerializeField] private DialogueUI dialogueUI; // Tham chiếu đến DialogueUI để hiển thị hội thoại
    public DialogueUI DialogueUI => dialogueUI; // Getter để truy cập DialogueUI từ bên ngoài
    public IInteractiveable interactiveable { get; set; } // Tham chiếu đến đối tượng có thể tương tác

    [Header("Di Chuyển & Nhảy")]
    [SerializeField] private float tocdo = 5f;
    [SerializeField] private float nhay = 25f;
    [SerializeField] private LayerMask matdat;
    [SerializeField] private Transform vitriktra;
    [SerializeField] private float tamktmatdat = 0.3f;
    [SerializeField] private bool xacthuc;

    [Header("Leo Thang")]
    [SerializeField] private LayerMask cauthang;
    [SerializeField] private float tocdoleo = 10f;
    [SerializeField] private Transform vtrict;
    [SerializeField] private float tamktleo = 0.2f;
    [SerializeField] private bool leothang;
    private float gravitystart;
    
    [Header("Bắn Súng")]
    [SerializeField] private Transform diemban;  // Điểm sinh đạn
    [SerializeField] private GameObject danPrefab;  // Prefab viên đạn
    [SerializeField] private GameObject specialDanPrefab; // Prefab đạn đặc biệt
    [SerializeField] private float tocdoDan = 10f;  // Tốc độ đạn
    [SerializeField] private float danLifetime = 3f; // Thời gian tồn tại của đạn
    [SerializeField] private float banCooldown = 0.2f; // Thời gian chờ giữa các lần bắn
    // Lưu vị trí gốc của DiemBan
    private Vector3 diemBanLocalOrigin;
    private float banTimer = 0f;    // Đếm ngược cooldown
    private bool isShooting = false; // Đánh dấu đang giữ phím bắn

    [Header("Grappling Hook")]
    [SerializeField] private float grapplingRange = 5f; // Khoảng cách tối đa có thể móc
    [SerializeField] private float Speed = 20f; // Tốc độ kéo người chơi
    [SerializeField] private float grapplingCooldown = 0.5f; // Thời gian chờ giữa các lần móc
    [SerializeField] private float minGrapplingDistance = 1.5f; // Khoảng cách tối thiểu để móc
    [SerializeField] private LayerMask grapplingLayer; // Layer của các vật thể có thể móc
    [SerializeField] private float autoCancelTime = 1.5f; // Thời gian tự động hủy đu/kéo
    private bool isGrappling = false; // Đang móc hay không
    private Vector2 grapplingPoint; // Điểm móc
    private float grapplingTimer = 0f; // Đếm ngược cooldown
    private LineRenderer grapplingLine; // Đường dây móc
    private bool canGrapple = true; // Có thể móc hay không
    private GameObject pulledObject; // Vật thể đang được kéo
    private bool isPullingObject = false; // Đang kéo vật thể hay không
    [SerializeField] private Transform grapplingOrigin; // Điểm xuất phát dây móc
    [SerializeField] private Transform grapplingDistanceOrigin; // Gốc để tính khoảng cách dừng kéo
    private float grapplingDuration = 0f; // Thời gian đã đu/kéo

    [Header("Component")]
    private Rigidbody2D rb;
    private Collider2D myBodyCollider;
    private Animator ani;
    private bool isAlive = true; // Trạng thái sống của nhân vật

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<Collider2D>();
        ani = GetComponent<Animator>();
        if (diemban != null)
            diemBanLocalOrigin = diemban.localPosition;
        
        grapplingLine = GetComponent<LineRenderer>();
        grapplingLine.startWidth = 0.1f;
        grapplingLine.endWidth = 0.1f;
        grapplingLine.material = new Material(Shader.Find("Sprites/Default"));
        grapplingLine.startColor = Color.white;
        grapplingLine.endColor = Color.white;
        grapplingLine.enabled = false;
        gravitystart = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        // Kiểm tra nếu hộp thoại đang mở
        if (dialogueUI != null && dialogueUI.IsOpen) 
        {
            // Dừng player movement khi hộp thoại mở
            StopPlayerMovement();
            return; // Không cho di chuyển hay bắn
        }
        
        if (!isAlive) return; // Nếu không còn sống thì không cho thực hiện các hành động khác
        kiemtract();
        dichuyen();
        Nhay();
        Leoct();
        Ban();
        GrapplingHook(); // Thêm hàm móc
        Hiemnguy(); // Kiểm tra va chạm nguy hiểm và chạm thì chết
        dialogue(); // Kiểm tra tương tác đối thoại
    }

    // Hàm dừng player movement
    private void StopPlayerMovement()
    {
        if (rb != null)
        {
            // Giữ lại velocity Y (trọng lực) nhưng dừng velocity X (di chuyển ngang)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        
        // Tắt các animation di chuyển
        if (ani != null)
        {
            ani.SetBool("Run", false);
            ani.SetBool("RunShoot", false);
        }
    }

    private void dichuyen()
    {
        // tao van toc cua nhan vat 
        float moveinput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveinput * tocdo, rb.linearVelocity.y);
        //Lat anh nhan vat.
        if (moveinput > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            if (diemban != null) diemban.localPosition = new Vector3(Mathf.Abs(diemBanLocalOrigin.x), diemBanLocalOrigin.y, diemBanLocalOrigin.z);
        }
        else if (moveinput < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            if (diemban != null) diemban.localPosition = new Vector3(-Mathf.Abs(diemBanLocalOrigin.x), diemBanLocalOrigin.y, diemBanLocalOrigin.z);
        }
        // goi animation
        ani.SetBool("Run", moveinput != 0);

    }
    private void Nhay()
    {
        xacthuc = Physics2D.OverlapCircle(vitriktra.position, tamktmatdat, matdat);
        if (Input.GetButtonDown("Jump") && xacthuc)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, nhay);
        }
        ani.SetBool("Jump", !xacthuc);
    }

    private void kiemtract()
    {
        if (Physics2D.OverlapCircle(vtrict.position, tamktleo, cauthang))
        {
            leothang = true;
            rb.gravityScale = 0;
        }
        else
        {
            leothang = false;
            rb.gravityScale = gravitystart;
        }
    }
    private void Leoct()
    {
        if (leothang)
        {
            float moveinput = Input.GetAxis("Vertical");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveinput * tocdoleo);
        }
        ani.SetBool("Ladder", leothang);
    }
    private void Ban()
    {
        bool isRunning = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
        isShooting = Input.GetKey(KeyCode.X);

        // Nếu giữ phím X và hết cooldown
        if (isShooting && banTimer <= 0f)
        {
            if (isRunning)
            {
                ani.SetBool("RunShoot", true); // Bật animation vừa chạy vừa bắn
                ShootBullet(); // Bắn đạn liên tục khi chạy
            }
            else
            {
                ani.SetTrigger("BanDan"); // Animation bắn đứng yên (trigger)
                ShootBullet(); // Bắn đạn liên tục khi đứng
            }
            banTimer = banCooldown;
        }

        // Kiểm tra bắn đạn đặc biệt
        if (Input.GetKeyDown(KeyCode.C) && GameManager.Instance != null && GameManager.Instance.CanShootSpecialBullet())
        {
            ShootSpecialBullet();
            GameManager.Instance.OnSpecialBulletShot();
        }

        // Nếu nhả phím X thì tắt animation RunShoot
        if (!isShooting)
        {
            ani.SetBool("RunShoot", false);
        }

        if (banTimer > 0f) banTimer -= Time.deltaTime;
    }

    // Hàm này sẽ được gọi bởi Animation Event đúng frame bắn
    public void ShootBullet()
    {
        // Tạo đạn tại điểm bắn
        GameObject dan = Instantiate(danPrefab, diemban.position, diemban.rotation);
        // Lấy component Bullet
        Bullet bullet = dan.GetComponent<Bullet>();
        if (bullet != null)
        {
            float direction = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
            bullet.Initialize(direction, tocdoDan, danLifetime);
            SpriteRenderer danSprite = dan.GetComponent<SpriteRenderer>();
            if (danSprite != null)
                danSprite.flipX = GetComponent<SpriteRenderer>().flipX;
        }
    }

    private void ShootSpecialBullet()
    {
        // Tạo đạn đặc biệt tại điểm bắn
        GameObject dan = Instantiate(specialDanPrefab, diemban.position, diemban.rotation);
        // Lấy component SpecialBullet
        SpecialBullet bullet = dan.GetComponent<SpecialBullet>();
        if (bullet != null)
        {
            float direction = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
            bullet.Initialize(direction, tocdoDan, danLifetime);
            SpriteRenderer danSprite = dan.GetComponent<SpriteRenderer>();
            if (danSprite != null)
                danSprite.flipX = GetComponent<SpriteRenderer>().flipX;
        }
    }

    private void GrapplingHook()
    {
        //Cooldown
        if (grapplingTimer > 0f)
        {
            grapplingTimer -= Time.deltaTime;
        }

        //Nếu đang móc
        if (isGrappling)
        {
            //Thời gian đu/kéo
            grapplingDuration += Time.deltaTime;

            //Thời gian tự động hủy
            if (grapplingDuration >= autoCancelTime)
            {
                StopGrappling();
                return;
            }

            if (isPullingObject && pulledObject != null)
            {
                grapplingLine.SetPosition(0, grapplingOrigin != null ? grapplingOrigin.position : transform.position);
                grapplingLine.SetPosition(1, pulledObject.transform.position);

                Rigidbody2D objectRb = pulledObject.GetComponent<Rigidbody2D>();
                if (objectRb != null)
                {
                    Vector2 pullDirection = ((Vector2)(grapplingDistanceOrigin != null ? grapplingDistanceOrigin.position : transform.position) - (Vector2)pulledObject.transform.position).normalized;
                    objectRb.linearVelocity = pullDirection * Speed;
                }

                float distanceToPlayer = Vector2.Distance(
                    grapplingDistanceOrigin != null ? grapplingDistanceOrigin.position : transform.position,
                    pulledObject.transform.position);
                if (distanceToPlayer < minGrapplingDistance)
                {
                    // Tắt Rigidbody2D khi dừng kéo
                    if (objectRb != null)
                    {
                        objectRb.linearVelocity = Vector2.zero;
                    }
                    StopGrappling();
                }
            }
            else
            {
                grapplingLine.SetPosition(0, grapplingOrigin != null ? grapplingOrigin.position : transform.position);
                grapplingLine.SetPosition(1, grapplingPoint);

                Vector2 grappleDir = (grapplingPoint - (Vector2)(grapplingDistanceOrigin != null ? grapplingDistanceOrigin.position : transform.position)).normalized;
                rb.linearVelocity = grappleDir * Speed;

                float distanceToPoint = Vector2.Distance(
                    grapplingDistanceOrigin != null ? grapplingDistanceOrigin.position : transform.position,
                    grapplingPoint);
                if (distanceToPoint < 0.5f)
                {
                    StopGrappling();
                }
            }
        }
        // Nếu không đang móc và có thể móc
        else if (canGrapple && grapplingTimer <= 0f)
        {
            // Kiểm tra phím móc
            if (Input.GetKeyDown(KeyCode.Z))
            {
                // Xác định hướng nhìn của player
                float direction = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
                Vector2 rayDirection = new Vector2(direction, 0);

                // Tìm vật thể có thể móc trong phạm vi theo hướng nhìn
                Vector2 raycastOrigin = grapplingOrigin != null ? grapplingOrigin.position : transform.position;
                RaycastHit2D[] hits = Physics2D.RaycastAll(raycastOrigin, rayDirection, grapplingRange, grapplingLayer);
                float closestDistance = float.MaxValue;
                Vector2 closestPoint = Vector2.zero;
                GameObject closestObject = null;

                foreach (RaycastHit2D hit in hits)
                {
                    float distance = Vector2.Distance(raycastOrigin, hit.point);
                    // Chỉ xét các điểm móc trong khoảng cách cho phép
                    if (distance >= minGrapplingDistance && distance < closestDistance)
                    {
                        if (hit.collider.CompareTag("Duday"))
                        {
                            closestDistance = distance;
                            closestPoint = hit.point;
                            closestObject = null;
                        }
                        else if (hit.collider.CompareTag("Dungdaykeo"))
                        {
                            closestDistance = distance;
                            closestPoint = hit.point;
                            closestObject = hit.collider.gameObject;
                        }
                    }
                }

                // Nếu tìm thấy điểm móc hợp lệ
                if (closestDistance < float.MaxValue)
                {
                    StartGrappling(closestPoint, closestObject);
                }
            }
        }
    }

    private void StartGrappling(Vector2 point, GameObject objectToPull = null)
    {
        isGrappling = true;
        grapplingPoint = point;
        grapplingLine.enabled = true;
        rb.gravityScale = 0f; // Tắt trọng lực khi đang móc
        grapplingDuration = 0f; // Reset thời gian đu/kéo

        if (objectToPull != null)
        {
            isPullingObject = true;
            pulledObject = objectToPull;
        }
        else
        {
            isPullingObject = false;
            pulledObject = null;
        }
    }

    private void StopGrappling()
    {
        isGrappling = false;
        grapplingLine.enabled = false;
        rb.gravityScale = gravitystart; // Khôi phục trọng lực
        grapplingTimer = grapplingCooldown; // Đặt cooldown
        isPullingObject = false;
        grapplingDuration = 0f; // Reset thời gian đu/kéo
        // Không set pulledObject = null ở đây để giữ lại reference nếu cần
    }

    public void Die()
    {
            isAlive = false;
            rb.linearVelocity = Vector2.zero; // Dừng chuyển động
            rb.gravityScale = 0; // Tắt trọng lực
            ani.SetTrigger("Die"); // Gọi animation chết

            myBodyCollider.enabled = false; // Tắt collider của Player

            Destroy(gameObject, 2f); // Hủy sau 2 giây
    }

    void Hiemnguy()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask(layerNames: "Enemies")))
        {
            Die();
        }
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask(layerNames: "Bay")))
        {
            Die();
        }
    }

    void dialogue()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (interactiveable != null)
            {
                interactiveable.interact(this); // Gọi phương thức Interact của đối tượng có thể tương tác
            }
        }
    }

    //Vẽ Gizmos để hiển thị các vùng kiểm tra
    private void OnDrawGizmosSelected()
    {
        // 1. Vẽ vòng tròn KIỂM TRA ĐẤT (Màu vàng)
        if (vitriktra != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(vitriktra.position, tamktmatdat);
        }

        // 2. Vẽ vòng tròn KIỂM TRA THANG (Màu xanh cyan)
        if (vtrict != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(vtrict.position, tamktleo);
        }

        // 3. Vẽ đường thẳng cho TẦM MÓC KÉO (Màu đỏ)
        // Lấy hướng nhìn hiện tại của nhân vật
        // Cần có một SpriteRenderer để chạy được trong Edit Mode, nếu không có sẽ báo lỗi
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float direction = sr.flipX ? -1f : 1f;
            Vector2 rayDirection = new Vector2(direction, 0);

            // Điểm bắt đầu là vị trí của người chơi
            Vector3 startPoint = grapplingOrigin.position;
            // Điểm kết thúc là vị trí người chơi + hướng nhìn * tầm xa
            Vector3 endPoint = startPoint + (Vector3)rayDirection * grapplingRange;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(startPoint, endPoint);

            // (Thêm) Vẽ vòng tròn cho KHOẢNG CÁCH TỐI THIỂU (Màu đỏ nhạt)
            // Dùng màu đỏ nhưng mờ hơn để không bị nhầm lẫn
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(grapplingDistanceOrigin.position, minGrapplingDistance);
        }
    }
}