using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
    public float captureInterval = 2f; // 몇 초마다 캡처할지 설정
    private float timer = 0f;          // 시간 누적용 타이머
    private int screenshotCount = 0;   // 저장될 파일 이름 번호

    void Update()
    {
        // 프레임마다 시간 누적
        timer += Time.deltaTime;

        // 설정된 시간 간격이 넘으면 캡처
        if (timer >= captureInterval)
        {
            string fileName = $"Loading{screenshotCount:D2}.png"; // 예: Screenshot_000.png
            ScreenCapture.CaptureScreenshot(fileName);                // 실제 캡처 수행
            Debug.Log($"Saved screenshot: {fileName}");

            screenshotCount++; // 번호 증가
            timer = 0f;         // 타이머 리셋
        }
    }
}