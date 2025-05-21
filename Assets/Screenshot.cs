using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
    public float captureInterval = 2f; // �� �ʸ��� ĸó���� ����
    private float timer = 0f;          // �ð� ������ Ÿ�̸�
    private int screenshotCount = 0;   // ����� ���� �̸� ��ȣ

    void Update()
    {
        // �����Ӹ��� �ð� ����
        timer += Time.deltaTime;

        // ������ �ð� ������ ������ ĸó
        if (timer >= captureInterval)
        {
            string fileName = $"Loading{screenshotCount:D2}.png"; // ��: Screenshot_000.png
            ScreenCapture.CaptureScreenshot(fileName);                // ���� ĸó ����
            Debug.Log($"Saved screenshot: {fileName}");

            screenshotCount++; // ��ȣ ����
            timer = 0f;         // Ÿ�̸� ����
        }
    }
}