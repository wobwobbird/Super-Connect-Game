using UnityEngine;

public class InputFields : MonoBehaviour
{
    public int column;
    public GameManager gameManager;
    
    [Header("Blur Effect")]
    public Material blurMaterial;  // Assign in Inspector
    public float blurAmount = 3.0f; // Default blur level
    public GameObject blurBox;

    void Start()
    {
        if (blurMaterial != null)
        {
            blurMaterial.SetFloat("_BlurSize", blurAmount);
        }
    }

    void Update()
    {
        if (blurMaterial != null)
        {
            blurMaterial.SetFloat("_BlurSize", blurAmount);
        }
    }

    void OnMouseDown()
    {
        if (gameManager.gameWon)
        {
            return;
        }
        
        //Debug.Log($"Column Number = {column}");
        gameManager.SelectColumn(column);
    }

    void OnMouseEnter()
    {
        Debug.Log("Hover over");
        if (gameManager.gameWon)
        {
            return;
        }

        if (column < 0 || column >= 7)  // Assuming board has 7 columns (0-6)
        {
            //Debug.Log($"❌ [OnMouseOver] Invalid column index detected: {column}");
            return;
        }

        gameManager.HoverColumn(column);

        if (blurBox != null)
        {
            blurBox.SetActive(true);
        }
    }

    void OnMouseOver()
    {
        if (!gameManager.gameWon)
        {
            if (column >= 0 && column < 7)
            {
                gameManager.HoverColumn(column);
            }
        }
    }

    void OnMouseExit()
    {
        gameManager.player1Ghost.SetActive(false);
        gameManager.player2Ghost.SetActive(false);
        if (blurBox != null)
        {
            blurBox.SetActive(false);
        }
    }
}
