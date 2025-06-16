using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerVisual : MonoBehaviour
{
    public Image p1UI;
    public Image p2UI;

    public void Player2TurnTimer()
    {
        // Keep current RGB colors, just change alpha (0 = transparent, 1 = opaque)
        p1UI.color = new Color(p1UI.color.r, p1UI.color.g, p1UI.color.b, 1f);  // Make fully visible
        p2UI.color = new Color(p2UI.color.r, p2UI.color.g, p2UI.color.b, 0.3f);  // Make 50% transparent
    }

    public void Player1TurnTimer()
    {
        p1UI.color = new Color(p1UI.color.r, p1UI.color.g, p1UI.color.b, 0.3f);  // Make 50% transparent
        p2UI.color = new Color(p2UI.color.r, p2UI.color.g, p2UI.color.b, 1f);  // Make fully visible
    }
}
