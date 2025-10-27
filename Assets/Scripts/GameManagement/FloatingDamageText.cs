using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private Vector3 floatOffset = new Vector3(0, 2f, 0);
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color critColor = Color.yellow;
    [SerializeField] private float critFontScale = 1.4f; // 40% bigger for crits
    [SerializeField] private float normalFontScale = 1f;

    private TextMeshProUGUI text;
    private Transform cam;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        cam = Camera.main.transform;
    }

    public void Initialize(float damage, bool isCrit)
    {
        text.text = Mathf.RoundToInt(damage).ToString();
        text.color = isCrit ? critColor : normalColor;

        // 🔹 Scale up for crits
        float scaleFactor = isCrit ? critFontScale : normalFontScale;
        text.fontSize *= scaleFactor;

        transform.LookAt(transform.position + cam.forward);
        transform.localPosition += floatOffset;
        if (isCrit)
            StartCoroutine(PopEffect());

        StartCoroutine(FloatAndFade());
    }


    private System.Collections.IEnumerator FloatAndFade()
    {
        float elapsed = 0f;
        CanvasGroup group = GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
            group.alpha = Mathf.Lerp(1f, 0f, elapsed / lifetime);
            yield return null;
        }

        Destroy(gameObject);
    }
    private System.Collections.IEnumerator PopEffect()
    {
        float t = 0f;
        float duration = 0.2f;
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }

        // shrink back smoothly
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            yield return null;
        }

        transform.localScale = originalScale;
    }

}
