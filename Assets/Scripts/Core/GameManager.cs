using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public Health playerHealth, botHealth;

    [Header("--- UI ---")]
    public Button SkillBtn;
    public Button StatBtn;
    public Button GameStartBtn;

    [Header("---SKillUI---")]
    public SkillUI[] skillUIs;

    private void Start()
    {
        
    }


}
