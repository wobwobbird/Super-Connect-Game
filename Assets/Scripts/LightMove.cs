using UnityEngine;

public class LightMove : MonoBehaviour
{
    //Home screen light change
    public float rotation1 = 0f;
    [SerializeField] float rotation2 = 10f;
    public GameObject directionalLight;

    void Update()
    {

        directionalLight.transform.Rotate(rotation1, rotation2 * Time.deltaTime, 0);
    }
}