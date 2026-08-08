using UnityEngine;

public class FPSController : MonoBehaviour
{
    void Awake() {
        QualitySettings.vSyncCount = 0;  // VSync o‘chiq
        Application.targetFrameRate = 120; // desired FPS
    }
}