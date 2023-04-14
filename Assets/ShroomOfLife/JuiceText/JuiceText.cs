using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JuiceText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
 
    [SerializeField] float duration;
    [Tooltip("Time per character in seconds divided by 10")]
    [SerializeField] float timePerChar;

    private void Start()
    {
        transform.localScale = new Vector3(.2f, .2f, .2f);
        TweenToFullSize();
    }
    private void TweenToFullSize()
    {
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), duration).setEase(LeanTweenType.easeOutBack).setOnComplete(StartTweenToSmall);
    }
    private void StartTweenToSmall()
    {
        StartCoroutine(TweenToSmall());
    }
    private IEnumerator TweenToSmall()
    {
        float waitTime = scoreText.text.Length * timePerChar/10f;
        yield return new WaitForSeconds(waitTime);
        LeanTween.scale(gameObject, new Vector3(.2f, .2f, .2f), duration).setEase(LeanTweenType.easeInBack).setOnComplete(DestroySelf);
    }
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
    public void SetText(string text) => scoreText.text = text;
    public void SetColor(Color color) => scoreText.color = color;
}
public class JuiceTextCreator : MonoBehaviour
{
    public static void CreateJuiceText(string text)
    {
        JuiceText newText = InstantiateJuiceText();
        newText.SetText(text);
    }
    public static void CreateJuiceText(string text, Color textColor)
    {
        JuiceText newText = InstantiateJuiceText();
        newText.SetText(text);
        newText.SetColor(textColor);
    }
    private static JuiceText InstantiateJuiceText()
    {
        GameObject juiceTextPrefab = Resources.Load("PfJuiceText") as GameObject;
        Transform canvas = GameObject.Find("MainCanvas").transform;

        JuiceText newText = Instantiate(juiceTextPrefab,
           canvas.transform.position,
           Quaternion.identity,
           canvas).GetComponent<JuiceText>();
        return newText;
    }
}
