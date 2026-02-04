using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public enum GameState   // 게임 상태 3가지 정의
{
    Ready,
    Playing,
    GameOver
}

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // 싱글톤 인스턴스   
    public GameState State { get; private set; } = GameState.Ready;     // 현재 게임 상태
    
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;     // 플레이어 체력
    [SerializeField] private ZombieSpawner zombieSpawner;       // 좀비 스포너
   
    [Header("UI")]
    [SerializeField] private GameObject readyUI;    // 준비 UI
    [SerializeField] private GameObject gameplayUI;     // 게임 플레이 UI
    [SerializeField] private GameObject gameOverUI;     // 게임 오버 UI

    [Header("Score")]
    [SerializeField] private TMP_Text highScoreText;    // 최고 점수 텍스트
    [SerializeField] private TMP_Text scoreText;        // 현재 점수 텍스트
    private readonly System.Text.StringBuilder scoreSb = new();
    private readonly System.Text.StringBuilder highScoreSb = new();
    public int Score { get; private set; }      // 현재 점수
    public int HighScore { get; private set; }      // 최고 점수

    // =======================  Unity Lifecycle  =======================
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;   // 싱글톤 인스턴스 설정
    }

    void Start()
    {
        LoadHighScore();        // PlayerPrefs에서 저장된 최고점수 불러오기
        SetState(GameState.Ready);      // 초기 상태를 Ready로 전환 (UI/시간/스포너 설정 포함).
        UpdateScoreUI();        // UI에 점수 표시 업데이트
        playerHealth.OnDied += EnterGameOver;   // 플레이어 사망 시 게임 오버 상태로 전환
    }

    // =======================  State Control  =======================
    public void SetState(GameState newState)
    {
        State = newState;
        // 현재 상태에 맞는 UI만 활성화.
        readyUI.SetActive(State == GameState.Ready);
        gameplayUI.SetActive(State == GameState.Playing);
        gameOverUI.SetActive(State == GameState.GameOver);

        switch (State)  // 상태에 따른 게임 시간, 좀비 스포너, 커서 설정
        {
            case GameState.Ready:
                PauseGame();
                zombieSpawner.enabled = false;
                ShowCursor(true);
                break;

            case GameState.Playing:
                ResumeGame();
                zombieSpawner.enabled = true;
                playerHealth.ResetHealth();
                ResetScore();
                ShowCursor(false);
                break;

            case GameState.GameOver:
                PauseGame();
                zombieSpawner.enabled = false;
                ShowCursor(true);
                break;
        }
    }

    // =======================  Score API (외부 사용)  =======================
    public void AddScore(int amount)
    {
        if (State != GameState.Playing) return;     // 플레이 중이 아닐 땐 점수 추가 안함

        Score += amount;

        if (Score > HighScore)
        {
            HighScore = Score;
            SaveHighScore();
        }

        UpdateScoreUI();
    }

    // =======================  Button Events   =======================
    public void OnClickStart()
    {
        SetState(GameState.Playing);      // Start 버튼 클릭 시 게임 시작
    }

    public void OnClickRestart()
    {
        ResumeGame();   // 게임 재개 (일시정지 해제)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);   // 현재 씬 재로드
    }

    // =======================  Internal Helpers  =======================
    void EnterGameOver()    // 게임 오버 상태로 전환
    {
        SetState(GameState.GameOver);
    }

    void ResetScore()   // 점수 초기화
    {
        Score = 0;
        UpdateScoreUI();
    }

    void UpdateScoreUI()    // 점수 UI 업데이트
    {
        if (scoreText != null)
        {
            scoreSb.Clear();
            scoreSb.Append("SCORE : ");
            scoreSb.Append(Score);
            scoreText.text = scoreSb.ToString();
        }

        if (highScoreText != null)
        {
            highScoreSb.Clear();
            highScoreSb.Append("HIGH SCORE : ");
            highScoreSb.Append(HighScore);
            highScoreText.text = highScoreSb.ToString();
        }
    }

    void LoadHighScore()    // PlayerPrefs에서 최고점수 불러오기, 없으면 0으로 초기화.
    {
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    void SaveHighScore()    // PlayerPrefs에 최고점수 저장.
    {
        PlayerPrefs.SetInt("HighScore", HighScore);
        PlayerPrefs.Save();
    }

    void PauseGame() => Time.timeScale = 0f;    // 게임 일시정지
    void ResumeGame() => Time.timeScale = 1f;   // 게임 재개

    void ShowCursor(bool show)  // 커서 보이기/숨기기 설정
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
