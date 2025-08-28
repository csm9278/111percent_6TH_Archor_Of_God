using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Health playerHealth, botHealth;
    public HealthUIBinder playerUI, botUI;

    void Awake()
    {
        if (playerUI) playerUI.health = playerHealth;
        if (botUI) botUI.health = botHealth;
    }
}
