using System.Collections;
using UnityEngine;
using TMPro;
using DamageNumbersPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI score;
    public TextMeshProUGUI highScore;
    public int scoreValue = 0;
    // public DamageNumber numberPrefab;
    // public RectTransform rectParent;

    void Start()
    {
        score.text = scoreValue.ToString();
        highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    void Update()
    {
    }

    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }

    public IEnumerator ShowText()
    {
        promptText.enabled = true;
        yield return new WaitForSeconds(2f);
        promptText.enabled = false;
    }

    public int AddScore(int scorePoint)
    {
        return scoreValue += scorePoint;
    }
}
