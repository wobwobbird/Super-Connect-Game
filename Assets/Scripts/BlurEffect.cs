using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurController : MonoBehaviour
{
    public Volume blurVolume; // Assign in the Inspector
    public GameObject menuPanel; // Assign your UI menu

    void Update()
    {
        float targetWeight = menuPanel.activeSelf ? 1f : 0f;
        blurVolume.weight = Mathf.Lerp(blurVolume.weight, targetWeight, Time.deltaTime * 5);
    }
}