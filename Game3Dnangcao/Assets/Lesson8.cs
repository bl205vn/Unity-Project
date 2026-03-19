using UnityEngine;
using System.Collections;
using System.IO;

public class Lesson8 : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(CaptureScreenshot());
        }
    }

    IEnumerator CaptureScreenshot()
    {
        // Chờ cho đến khi kết thúc Frame
        yield return new WaitForEndOfFrame();
        // Lấy kích thước của màn hình
        int width = Screen.width;
        int height = Screen.height;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        // Thiết lập thông tin pixel
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();
        // Mã hóa dữ liệu sang dạng PNG
        byte[] bytes = texture.EncodeToPNG();
        Destroy(texture);
        // Sử dụng Application.persistentDataPath để có quyền ghi trên các nền tảng khác nhau
        string folderPath = Path.Combine(Application.persistentDataPath, "Screenshots");
        string filename = "Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string filePath = Path.Combine(folderPath, filename);
        Debug.Log($"Ảnh đã được lưu tại: {filePath}");
        // Tạo thư mục nếu nó chưa tồn tại
        Directory.CreateDirectory(folderPath);
        File.WriteAllBytes(filePath, bytes);
    }
}
