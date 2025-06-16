using UnityEngine;
using TMPro;

public class UnderlaySwirlEffect : MonoBehaviour
{
    public TMP_Text textMesh;
    public float speed = 2f;  // Speed of movement
    public float range = 5f;  // How far the shadow moves

    private Material textMaterial;
    private int underlayXID;
    private int underlayYID;

    //part2 text swirl
    public TMP_Text textMesh2;
    public Gradient gradient;
    public float speed2 = 2f;

    void Start()
    {
        if (textMesh == null)
            textMesh = GetComponent<TMP_Text>();

        // Get the material instance of the text
        textMaterial = textMesh.fontSharedMaterial;

        // Get the property IDs for underlay offsets
        underlayXID = Shader.PropertyToID("_UnderlayOffsetX");
        underlayYID = Shader.PropertyToID("_UnderlayOffsetY");
    }

    void Update()
    {
        float xOffset = Mathf.Sin(Time.time * speed) * range;
        float yOffset = Mathf.Cos(Time.time * speed) * range;

        // Apply dynamic underlay movement
        textMaterial.SetFloat(underlayXID, xOffset);
        textMaterial.SetFloat(underlayYID, yOffset);

        ////part2
        float t = Mathf.PingPong(Time.time * speed2, 1f);
        textMesh2.color = gradient.Evaluate(t);
    }
}