using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel.Design;
using UnityEngine.UI;

public class JuiceTextCreator : MonoBehaviour
{
    public static void CreateJuiceText(Vector2 position, string text, string juiceTextTypeName)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
        GameObject juiceTextPrefab = Resources.Load("PfJuiceText") as GameObject;
        Transform canvas = GameObject.Find("MainCanvas").transform;
        JuiceTextType juiceTextTypeSO = Resources.Load("Types/" + juiceTextTypeName) as JuiceTextType;

        JuiceTextNew newText = Instantiate(juiceTextPrefab,
           screenPos,
           Quaternion.identity,
           canvas).GetComponent<JuiceTextNew>();

        newText.SetText(text);
        newText.TextTypeSO = juiceTextTypeSO;
    }
}
public class JuiceTextNew : MonoBehaviour
{
    public JuiceTextType TextTypeSO
    {
        private get => TextTypeSO;
        set
        {
            textTypeSO = value;
            SetDefaultValues();
        }
    }
    private JuiceTextType textTypeSO;
    private TextMeshProUGUI scoreText;
    private float duration;
    private float timeElapsed;
    private Vector2 startPosition, endPosition;

    public delegate float LerpFunctionDelegate(float t);
    public LerpFunctionDelegate LerpFunction;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }
    public void SetText(string text) => scoreText.text = text;
    private void SetDefaultValues()
    {
        scoreText.color = textTypeSO.textColor;
        scoreText.fontSize = textTypeSO.fontSize;
        duration = textTypeSO.duration;

        startPosition = transform.position;
        endPosition = new Vector2(transform.position.x, transform.position.y + textTypeSO.heightLimit);

        if (textTypeSO.choosen == JuiceTextType.type.exponential) LerpFunction = delegate (float t) { return t * t; };
        else if (textTypeSO.choosen == JuiceTextType.type.easeIn) LerpFunction = delegate (float t) { return 1f - Mathf.Cos(t * Mathf.PI * 0.5f); };
        else if (textTypeSO.choosen == JuiceTextType.type.easeOut) LerpFunction = delegate (float t) { return Mathf.Sin(t * Mathf.PI * 0.5f); };
        else LerpFunction = delegate (float t) { return t; };
    }
    private void Update()
    {
        TransformLerp();
        AlphaLerp();
        CountDown();
    }
    private void TransformLerp()
    {
        float t = timeElapsed / duration;
        t = LerpFunction(t);
        
        transform.position = Vector2.Lerp(startPosition, endPosition, t);    
    }
    private void AlphaLerp() => scoreText.alpha = Mathf.Lerp(1, 0, timeElapsed / duration);
    private void CountDown()
    {
        if (timeElapsed < duration) timeElapsed += Time.deltaTime;
        else Destroy(gameObject);
    }
}
