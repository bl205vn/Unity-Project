using UnityEngine;
using System.Collections.Generic;

// ====================================================================
// WorldGenerator.cs
// Sinh toàn bộ thế giới: địa hình, nước, texture, cây cối, đá, cỏ
// Áp dụng: Perlin Noise, Random Tree từ Slide 6
// ====================================================================

public class WorldGenerator : MonoBehaviour
{
    // ======================== TERRAIN ========================
    [Header("=== TERRAIN SETTINGS ===")]
    public Terrain terrain;
    [Tooltip("Độ cao tối đa")]
    public float terrainDepth = 40f;

    [Header("Perlin Noise (Địa hình)")]
    [Tooltip("Độ mịn/rộng của noise chính")]
    public float mainScale = 8f;
    [Tooltip("Offset X để thay đổi hình dạng")]
    public float offsetX = 100f;
    [Tooltip("Offset Y để thay đổi hình dạng")]
    public float offsetY = 100f;
    [Tooltip("Thêm octave noise nhỏ cho chi tiết (0 = tắt)")]
    [Range(0f, 0.5f)] public float detailStrength = 0.15f;
    [Tooltip("Scale của octave chi tiết")]
    public float detailScale = 25f;

    // ======================== ZONES ========================
    [Header("=== ZONE THRESHOLDS (0-1) ===")]
    [Tooltip("Dưới ngưỡng này = hồ nước (vùng trũng)")]
    [Range(0f, 1f)] public float waterLevel = 0.3f;
    [Tooltip("Từ waterLevel đến ngưỡng này = đồng bằng")]
    [Range(0f, 1f)] public float plainsLevel = 0.5f;
    [Tooltip("Trên ngưỡng này = đồi núi")]
    [Range(0f, 1f)] public float hillLevel = 0.5f;

    // ======================== WATER ========================
    [Header("=== WATER (Hồ nước) ===")]
    [Tooltip("Material nước (dùng Transparent hoặc URP Water)")]
    public Material waterMaterial;
    [Tooltip("Màu nước nếu không có Material")]
    public Color waterColor = new Color(0.1f, 0.4f, 0.7f, 0.7f);

    // ======================== TEXTURES ========================
    [Header("=== TERRAIN TEXTURES ===")]
    [Tooltip("Texture vùng nước/cát (ven hồ)")]
    public Texture2D sandTexture;
    [Tooltip("Texture vùng đồng bằng (cỏ xanh)")]
    public Texture2D grassTexture;
    [Tooltip("Texture vùng đồi (đất nâu)")]
    public Texture2D dirtTexture;
    [Tooltip("Texture vùng núi cao (đá xám)")]
    public Texture2D rockTexture;
    [Tooltip("Kích thước lặp texture")]
    public Vector2 textureTileSize = new Vector2(10, 10);

    // ======================== VEGETATION ========================
    [Header("=== CÂY TO (Có va chạm - không đi xuyên qua) ===")]
    [Tooltip("Danh sách prefab cây to (kéo nhiều prefab vào đây)")]
    public GameObject[] bigTreePrefabs;
    [Tooltip("Số cây to trên đồi núi")]
    public int bigTreeCount = 80;
    [Tooltip("Bán kính Collider cây to")]
    public float treeColliderRadius = 0.5f;

    [Header("=== ĐÁ (Có va chạm) ===")]
    [Tooltip("Danh sách prefab đá")]
    public GameObject[] rockPrefabs;
    [Tooltip("Số đá trên đồi núi")]
    public int rockCount = 40;

    [Header("=== CỎ / CÂY NHỎ (Đi xuyên qua được) ===")]
    [Tooltip("Danh sách prefab cỏ/cây nhỏ cho đồng bằng")]
    public GameObject[] plainGrassPrefabs;
    [Tooltip("Số cỏ trên đồng bằng")]
    public int plainGrassCount = 200;

    [Tooltip("Danh sách prefab cỏ/cây nhỏ ven hồ nước")]
    public GameObject[] waterGrassPrefabs;
    [Tooltip("Số cỏ ven hồ nước")]
    public int waterGrassCount = 100;

    [Tooltip("Danh sách prefab cây trung bình/nhỏ cho đồi")]
    public GameObject[] hillSmallTreePrefabs;
    [Tooltip("Số cây nhỏ trên đồi")]
    public int hillSmallTreeCount = 60;

    // ======================== PRIVATE ========================
    private GameObject waterPlane;
    private Transform vegetationParent;

    // ============================================================
    //                         START
    // ============================================================
    void Start()
    {
        if (terrain == null)
            terrain = GetComponent<Terrain>();

        if (terrain == null)
        {
            Debug.LogError("WorldGenerator: No Terrain found! Add this script to a Terrain or assign one.");
            return;
        }

        GenerateWorld();
    }

    // ============================================================
    //                    GENERATE WORLD
    // ============================================================
    [ContextMenu("Generate World")]
    public void GenerateWorld()
    {
        if (terrain == null) return;

        // Xóa thế giới cũ
        CleanupAll();

        // Bước 1: Sinh địa hình Perlin Noise
        GenerateTerrain();

        // Bước 2: Tạo mặt nước
        CreateWaterPlane();

        // Bước 3: Áp dụng texture theo vùng
        ApplyZoneTextures();

        // Bước 4: Spawn cây cối, đá, cỏ theo vùng
        SpawnAllVegetation();

        Debug.Log("=== World Generation Complete! ===");
    }

    // ============================================================
    //     BƯỚC 1: SINH ĐỊA HÌNH (Perlin Noise - từ Slide 6)
    // ============================================================
    void GenerateTerrain()
    {
        TerrainData td = terrain.terrainData;
        int res = td.heightmapResolution;

        td.size = new Vector3(td.size.x, terrainDepth, td.size.z);

        float[,] heights = new float[res, res];

        for (int x = 0; x < res; x++)
        {
            for (int y = 0; y < res; y++)
            {
                // Noise chính: tạo địa hình đồi núi + hồ
                float xCoord = (float)x / res * mainScale + offsetX;
                float yCoord = (float)y / res * mainScale + offsetY;
                float mainNoise = Mathf.PerlinNoise(xCoord, yCoord);

                // Octave chi tiết: thêm mấp mô nhỏ cho tự nhiên
                float detailNoise = 0f;
                if (detailStrength > 0f)
                {
                    float dx = (float)x / res * detailScale + offsetX * 2f;
                    float dy = (float)y / res * detailScale + offsetY * 2f;
                    detailNoise = Mathf.PerlinNoise(dx, dy) * detailStrength;
                }

                heights[x, y] = Mathf.Clamp01(mainNoise + detailNoise);
            }
        }

        td.SetHeights(0, 0, heights);
        Debug.Log("Step 1: Terrain generated with Perlin Noise.");
    }

    // ============================================================
    //     BƯỚC 2: TẠO MẶT NƯỚC
    // ============================================================
    void CreateWaterPlane()
    {
        // Tính chiều cao thực tế của mặt nước
        TerrainData td = terrain.terrainData;
        float waterWorldHeight = waterLevel * td.size.y + terrain.transform.position.y;

        // Tạo plane nước
        waterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        waterPlane.name = "WaterSurface";
        waterPlane.transform.SetParent(this.transform);

        // Đặt vị trí + scale khớp với Terrain
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = td.size;

        waterPlane.transform.position = new Vector3(
            terrainPos.x + terrainSize.x / 2f,
            waterWorldHeight,
            terrainPos.z + terrainSize.z / 2f
        );

        // Plane mặc định là 10x10 units, scale để khớp terrain
        waterPlane.transform.localScale = new Vector3(
            terrainSize.x / 10f,
            1f,
            terrainSize.z / 10f
        );

        // Gán material nước
        Renderer waterRenderer = waterPlane.GetComponent<Renderer>();
        if (waterMaterial != null)
        {
            waterRenderer.material = waterMaterial;
        }
        else
        {
            // Tạo material đơn giản nếu không có
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 0);
            mat.SetFloat("_AlphaClip", 0);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.renderQueue = 3000;
            mat.color = waterColor;
            waterRenderer.material = mat;
        }

        // Tắt collider mặt nước (player có thể đi qua)
        Collider waterCol = waterPlane.GetComponent<Collider>();
        if (waterCol != null) waterCol.enabled = false;

        Debug.Log($"Step 2: Water plane created at height {waterWorldHeight:F1}.");
    }

    // ============================================================
    //     BƯỚC 3: ÁP DỤNG TEXTURE THEO VÙNG
    // ============================================================
    void ApplyZoneTextures()
    {
        TerrainData td = terrain.terrainData;

        // Đếm số texture thực sự có
        int layerCount = 4; // sand, grass, dirt, rock
        TerrainLayer[] layers = new TerrainLayer[layerCount];

        // Layer 0: Cát/bùn (ven hồ)
        layers[0] = new TerrainLayer();
        layers[0].diffuseTexture = sandTexture;
        layers[0].tileSize = textureTileSize;

        // Layer 1: Cỏ xanh (đồng bằng)
        layers[1] = new TerrainLayer();
        layers[1].diffuseTexture = grassTexture;
        layers[1].tileSize = textureTileSize;

        // Layer 2: Đất nâu (đồi)
        layers[2] = new TerrainLayer();
        layers[2].diffuseTexture = dirtTexture;
        layers[2].tileSize = textureTileSize;

        // Layer 3: Đá xám (núi cao)
        layers[3] = new TerrainLayer();
        layers[3].diffuseTexture = rockTexture;
        layers[3].tileSize = textureTileSize;

        td.terrainLayers = layers;

        // Setup alphamap
        int aw = td.alphamapWidth;
        int ah = td.alphamapHeight;
        float[,,] alphamap = new float[aw, ah, layerCount];

        for (int x = 0; x < aw; x++)
        {
            for (int y = 0; y < ah; y++)
            {
                float normX = (float)x / (aw - 1);
                float normY = (float)y / (ah - 1);
                float h = td.GetInterpolatedHeight(normX, normY) / td.size.y;
                float steepness = td.GetSteepness(normX, normY);

                float[] w = new float[layerCount];

                if (h <= waterLevel + 0.03f)
                {
                    // Vùng nước + viền hồ -> Cát/bùn
                    w[0] = 1f;
                }
                else if (h <= plainsLevel)
                {
                    // Đồng bằng -> Cỏ xanh (với blend nhẹ ven hồ)
                    float blend = Mathf.InverseLerp(waterLevel + 0.03f, waterLevel + 0.08f, h);
                    w[0] = 1f - blend; // Cát mờ dần
                    w[1] = blend;       // Cỏ rõ dần
                }
                else if (h <= hillLevel + 0.15f)
                {
                    // Đồi -> Đất nâu (pha cỏ)
                    float blend = Mathf.InverseLerp(plainsLevel, hillLevel + 0.15f, h);
                    w[1] = 1f - blend;
                    w[2] = blend;
                }
                else
                {
                    // Núi cao -> Đá xám (pha đất)
                    float blend = Mathf.InverseLerp(hillLevel + 0.15f, 0.85f, h);
                    w[2] = 1f - blend;
                    w[3] = blend;
                }

                // Nếu dốc nhiều -> thêm đá bất kể độ cao
                if (steepness > 30f && h > waterLevel + 0.05f)
                {
                    float slopeBlend = Mathf.InverseLerp(30f, 60f, steepness);
                    for (int i = 0; i < layerCount; i++) w[i] *= (1f - slopeBlend);
                    w[3] += slopeBlend;
                }

                // Normalize
                float total = 0f;
                for (int i = 0; i < layerCount; i++) total += w[i];
                if (total > 0f)
                    for (int i = 0; i < layerCount; i++) w[i] /= total;

                for (int i = 0; i < layerCount; i++)
                    alphamap[x, y, i] = w[i];
            }
        }

        td.SetAlphamaps(0, 0, alphamap);
        Debug.Log("Step 3: Zone textures applied.");
    }

    // ============================================================
    //     BƯỚC 4: SPAWN CÂY CỐI, ĐÁ, CỎ THEO VÙNG
    //     (Áp dụng Random Tree từ Slide 6)
    // ============================================================
    void SpawnAllVegetation()
    {
        // Tạo parent chứa tất cả vegetation
        vegetationParent = new GameObject("Vegetation").transform;
        vegetationParent.SetParent(this.transform);

        // 4a: Cây to trên đồi núi (CÓ VA CHẠM)
        SpawnObjects(
            bigTreePrefabs, bigTreeCount,
            hillLevel, 1f,             // Chỉ spawn trên đồi núi
            "BigTrees", true,          // CÓ Collider
            0.7f, 1.3f                 // Random scale
        );

        // 4b: Đá trên đồi núi (CÓ VA CHẠM)
        SpawnObjects(
            rockPrefabs, rockCount,
            hillLevel - 0.05f, 0.9f,   // Đồi núi
            "Rocks", true,
            0.5f, 1.5f
        );

        // 4c: Cây nhỏ trên đồi (KHÔNG VA CHẠM - đi xuyên qua)
        SpawnObjects(
            hillSmallTreePrefabs, hillSmallTreeCount,
            hillLevel - 0.05f, 0.85f,
            "HillSmallTrees", false,
            0.6f, 1.0f
        );

        // 4d: Cỏ/cây nhỏ trên đồng bằng (KHÔNG VA CHẠM)
        SpawnObjects(
            plainGrassPrefabs, plainGrassCount,
            waterLevel + 0.04f, plainsLevel + 0.05f,
            "PlainGrass", false,
            0.5f, 1.2f
        );

        // 4e: Cỏ ven hồ nước (KHÔNG VA CHẠM)
        SpawnObjects(
            waterGrassPrefabs, waterGrassCount,
            waterLevel - 0.02f, waterLevel + 0.06f,
            "WaterGrass", false,
            0.4f, 0.9f
        );

        Debug.Log("Step 4: Vegetation spawned.");
    }

    // ============================================================
    //     HÀM SPAWN VẬT THỂ CHUNG
    // ============================================================
    void SpawnObjects(
        GameObject[] prefabs, int count,
        float minHeight, float maxHeight,
        string groupName, bool hasCollider,
        float minScale, float maxScale)
    {
        if (prefabs == null || prefabs.Length == 0 || count <= 0) return;

        TerrainData td = terrain.terrainData;
        Vector3 terrainSize = td.size;
        Vector3 terrainPos = terrain.transform.position;
        int hmRes = td.heightmapResolution;

        // Tạo sub-parent
        Transform group = new GameObject(groupName).transform;
        group.SetParent(vegetationParent);

        int placed = 0;
        int maxAttempts = count * 3; // Tránh loop vô hạn

        for (int attempt = 0; attempt < maxAttempts && placed < count; attempt++)
        {
            float normX = Random.Range(0.02f, 0.98f);
            float normZ = Random.Range(0.02f, 0.98f);

            // Lấy độ cao
            int hmX = Mathf.FloorToInt(normX * (hmRes - 1));
            int hmZ = Mathf.FloorToInt(normZ * (hmRes - 1));
            float normalizedHeight = td.GetHeight(hmX, hmZ) / terrainSize.y;

            // Kiểm tra vùng
            if (normalizedHeight < minHeight || normalizedHeight > maxHeight) continue;

            // Tính world position
            float worldX = normX * terrainSize.x + terrainPos.x;
            float worldZ = normZ * terrainSize.z + terrainPos.z;
            float worldY = normalizedHeight * terrainSize.y + terrainPos.y;

            // Chọn prefab ngẫu nhiên
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            if (prefab == null) continue;

            Vector3 pos = new Vector3(worldX, worldY, worldZ);
            Quaternion rot = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            GameObject obj = Instantiate(prefab, pos, rot, group);

            // Random scale
            float s = Random.Range(minScale, maxScale);
            obj.transform.localScale = prefab.transform.localScale * s;

            if (hasCollider)
            {
                // CÂY TO / ĐÁ: Đảm bảo có Collider để player không đi xuyên qua
                EnsureCollider(obj);
            }
            else
            {
                // CỎ / CÂY NHỎ: Xóa tất cả Collider để player đi xuyên qua
                RemoveAllColliders(obj);
            }

            placed++;
        }

        Debug.Log($"  -> {groupName}: Placed {placed}/{count}");
    }

    // ============================================================
    //     COLLIDER HELPERS
    // ============================================================

    // Đảm bảo object có Collider (cho cây to, đá)
    void EnsureCollider(GameObject obj)
    {
        // Kiểm tra trên object chính và children
        Collider[] existingColliders = obj.GetComponentsInChildren<Collider>();
        if (existingColliders.Length > 0)
        {
            // Đã có collider -> giữ nguyên, đảm bảo enabled
            foreach (var col in existingColliders)
                col.enabled = true;
            return;
        }

        // Nếu chưa có collider -> thêm CapsuleCollider (phù hợp cây)
        CapsuleCollider capsule = obj.AddComponent<CapsuleCollider>();
        
        // Tự tính kích thước dựa trên bounds của mesh
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            // Chuyển center về local space
            capsule.center = obj.transform.InverseTransformPoint(bounds.center);
            capsule.radius = Mathf.Max(bounds.extents.x, bounds.extents.z) * 0.4f;
            capsule.height = bounds.size.y;
        }
        else
        {
            capsule.radius = treeColliderRadius;
            capsule.height = 5f;
            capsule.center = new Vector3(0, 2.5f, 0);
        }
    }

    // Xóa tất cả Collider (cho cỏ, cây nhỏ)
    void RemoveAllColliders(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            if (Application.isPlaying)
                Destroy(col);
            else
                DestroyImmediate(col);
        }
    }

    // ============================================================
    //     CLEANUP
    // ============================================================
    [ContextMenu("Cleanup World")]
    public void CleanupAll()
    {
        // Xóa nước
        if (waterPlane != null)
        {
            if (Application.isPlaying) Destroy(waterPlane);
            else DestroyImmediate(waterPlane);
        }

        // Xóa WaterSurface nếu còn sót
        Transform existingWater = this.transform.Find("WaterSurface");
        if (existingWater != null)
        {
            if (Application.isPlaying) Destroy(existingWater.gameObject);
            else DestroyImmediate(existingWater.gameObject);
        }

        // Xóa vegetation
        Transform existingVeg = this.transform.Find("Vegetation");
        if (existingVeg != null)
        {
            if (Application.isPlaying) Destroy(existingVeg.gameObject);
            else DestroyImmediate(existingVeg.gameObject);
        }

        Debug.Log("World cleaned up.");
    }

    // ============================================================
    //     RESET
    // ============================================================
    [ContextMenu("Reset Terrain Flat")]
    public void ResetTerrain()
    {
        if (terrain == null) return;

        CleanupAll();

        TerrainData td = terrain.terrainData;
        int res = td.heightmapResolution;
        td.SetHeights(0, 0, new float[res, res]);

        Debug.Log("Terrain reset to flat.");
    }
}
