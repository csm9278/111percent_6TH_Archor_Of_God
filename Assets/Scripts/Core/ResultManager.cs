using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public GameObject loseObject;
    public GameObject winObject;

    public Button goLobbyBtn;
    public Button winLobbyBtn;
    public Button nextStageBtn;
    public int earnGold = 0;

    // Start is called before the first frame update
    void Start()
    {
        goLobbyBtn.onClick.AddListener(() =>
        {
            GameManager.inst.ResetGame();
            this.gameObject.SetActive(false);
        });

        winLobbyBtn.onClick.AddListener(() => 
        {
            GameManager.inst.ResetGame();
            this.gameObject.SetActive(false);
        });
    }
    public void GameOver()
    {
        loseObject.gameObject.SetActive(true);
    }

    public void Win()
    {
        winObject.gameObject.SetActive(true);
    }
}
