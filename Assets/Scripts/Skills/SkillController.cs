using UnityEngine;

public class SkillController : MonoBehaviour
{
    public SkillBase skillQ, skillW, skillE;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) skillQ?.TryCast();
        if (Input.GetKeyDown(KeyCode.W)) skillW?.TryCast();
        if (Input.GetKeyDown(KeyCode.E)) skillE?.TryCast();
    }
}
