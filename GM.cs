using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GM : MonoBehaviour
{
    public static Action onBeginGame;
    public static Action onEndGame;
    public static Action onReset;
    public static GameState gameState {  get; private set; }
    public static float scoreThresholdPosX;
    public static int score;
    public static int highScore;

    static GM main;
    static Bird currentBird;
    static float countDownTimer;
    static float pipeSpawnX;

    [SerializeField] Bird birdPrefab;
    [SerializeField] Pipes pipesPrefab;
    [SerializeField] TextMeshProUGUI textUI;

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (main == this)
        {
            pipeSpawnX = Camera.main.ViewportToWorldPoint(Camera.main.rect.max).x;
            PrepareGame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (main == this)
        {
            switch (gameState)
            {
                case GameState.Prepared:
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        BeginGame();
                    }
                    break;
                case GameState.Playing:
                    countDownTimer -= Time.deltaTime;
                    if (countDownTimer < 0)
                    {
                        countDownTimer = 1;
                        var pipeSpawnPos = new Vector2(pipeSpawnX, 2.5f * Random.Range(-1f, 1f));
                        Instantiate(pipesPrefab, pipeSpawnPos, Quaternion.identity);
                    }
                    break;
                case GameState.Ended:
                    countDownTimer -= Time.deltaTime;
                    if (countDownTimer <= 0)
                    {
                        Reset();
                    }
                    break;
            }

            textUI.text = $"High Score: {highScore}\nScore: {score}";
        }
    }

    void OnDestroy()
    {
        if (main == this)
        {
            main = null;
        }
    }

    void PrepareGame()
    {
        countDownTimer = 1;
        currentBird = Instantiate(birdPrefab);
        scoreThresholdPosX = currentBird.GetComponent<Collider2D>().bounds.min.x;
        gameState = GameState.Prepared;
    }

    void BeginGame()
    {
        currentBird.onBirdDied += EndGame;
        gameState = GameState.Playing;
        onBeginGame?.Invoke();
    }

    void EndGame()
    {
        currentBird.onBirdDied -= EndGame;
        currentBird = null;
        countDownTimer = 2;
        gameState = GameState.Ended;
        onEndGame?.Invoke();
    }

    void Reset()
    {
        score = 0;
        onReset?.Invoke();
        PrepareGame();
    }

    public static void Score()
    {
        score++;
        if (score > highScore)
        {
            highScore = score;
        }
    }
}

public enum GameState
{
    Prepared,
    Playing,
    Ended,
}