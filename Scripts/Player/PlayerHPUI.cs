using UnityEngine;
using UnityEngine.UI;
public sealed class PlayerHPUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image[] hearts;
    void Update() { UpdateHearts(); }
    void UpdateHearts()
    {
        int currentHp = playerHealth.Hp;
        for (int i = 0; i < hearts.Length; i++)
        {
            // i<currentHpÀÌ¸é ÇØ´ç ÇÏÆ®´Â ÄÑÁü (enabled = true)
            // ¾Æ´Ï¸é ²¨Áü (enabled = false)
            hearts[i].enabled = i < currentHp; 
        }
    }
}