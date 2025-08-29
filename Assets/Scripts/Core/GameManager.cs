using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public bool gameStart = false;
    public Health playerHealth;
    public GameObject botPrefab;
    public Transform botSpawnPos;
    public HealthUIFancy botHpUI;
    public BotController botCtrl;
    public TMP_Text goldText;
    public ResultManager resultManager;

    [Header("--- UI ---")]
    public Canvas damageTextCanvas;
    public Button SkillBtn;
    public Button StatBtn;
    public Button GameStartBtn;

    [Header("---SKillUI---")]
    public SkillUI[] skillUIs;

    [Header("---Objects---")]
    public GameObject skillShop;
    public GameObject lobbyPanel;
    public GameObject hpUIObject;

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        SkillBtn.onClick.AddListener(() =>
        {
            skillShop.gameObject.SetActive(true);
        });

        GameStartBtn.onClick.AddListener(GameStart);
    }

    public void GameStart()
    {
        gameStart = true;
        lobbyPanel.gameObject.SetActive(false);
        GameObject botobj = Instantiate(botPrefab, botSpawnPos.position, botSpawnPos.rotation);
        var bothealth = botobj.GetComponent<Health>();
        if (bothealth)
            botHpUI.health = bothealth;

        hpUIObject.gameObject.SetActive(true);

        var damageable = botobj.GetComponent<Damageable>();
        if (damageable)
            damageable.uiCanvas = damageTextCanvas;

        botCtrl = botobj.GetComponent<BotController>();
        if (botCtrl)
            botCtrl.InitBot();

        PlayerController2D.inst.InitPlayer(botobj);
    }

    public void GameEnd(bool isPlayerDie = false)
    {
        StartCoroutine(GameEndCo(isPlayerDie));
    }

    IEnumerator GameEndCo(bool isPlayerDie = false)
    {
        if (isPlayerDie)
            botCtrl.WinMotion();
        else
            PlayerController2D.inst.WinMotion();

        yield return new WaitForSeconds(2.0f);

        resultManager.gameObject.SetActive(true);
        if (isPlayerDie)
            resultManager.GameOver();
        else
            resultManager.Win();

        yield break;
    }

    public void ResetGame()
    {
        if (botCtrl)
            Destroy(botCtrl.gameObject);

        lobbyPanel.gameObject.SetActive(true);
        hpUIObject.SetActive(false);
        PlayerController2D.inst.ResetPlayer();
        gameStart = false;

    }
}
