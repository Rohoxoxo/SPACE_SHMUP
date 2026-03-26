using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Main : MonoBehaviour {
    static private Main S;

    [Header("Inscribed")]
    public GameObject[] prefabEnemies;
    public bool spawnEnemies = true;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Dynamic")]
    public int score = 0;
    public int highScore = 0;

    private BoundsCheck bndCheck;

    void Awake() {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        highScore = PlayerPrefs.GetInt("SHMUP_HIGHSCORE", 0);

        UpdateScoreUI();

        if (spawnEnemies) {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
        }
    }

    public void SpawnEnemy() {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        if (spawnEnemies) {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
        }
    }

    public void AddScore(int n) {
        score += n;

        if (score > highScore) {
            highScore = score;
            PlayerPrefs.SetInt("SHMUP_HIGHSCORE", highScore);
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI() {
        if (scoreText != null) {
            scoreText.text = "Score: " + score;
        }

        if (highScoreText != null) {
            highScoreText.text = "High Score: " + highScore;
        }
    }

    void DelayedRestart() {
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart() {
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED() {
        S.DelayedRestart();
    }

    static public void ADD_SCORE(int n) {
        S.AddScore(n);
    }
}