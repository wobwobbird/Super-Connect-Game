using System.Linq.Expressions;
using UnityEngine;

public class JeffScript : MonoBehaviour
{

    public GameObject jeff;

    void OnMouseDown()
    {
        jeff.transform.position = new Vector3(-27, 6, 17);
        jeff.transform.localScale = new Vector3(6, 6, 6);
    }
}
