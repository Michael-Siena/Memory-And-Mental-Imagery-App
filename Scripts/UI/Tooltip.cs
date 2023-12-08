using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace UI
{
    public class Tooltip : MonoBehaviour
    {
        private Text text;

        private void Awake()
        {
            text = this.gameObject.GetComponent<Text>();
        }

        private void OnEnable()
        {
            // fades text over 3 seconds and then disables self
            StartCoroutine(FadeText(3f));
        }

        private IEnumerator FadeText(float duration)
        {
            Color initialColor = new Color(text.color.r, text.color.g, text.color.b, 1);
            Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                Color currentColor = Color.Lerp(initialColor, targetColor, elapsedTime / duration);
                text.color = currentColor;
                yield return null;
            }
            this.gameObject.SetActive(false);
            text.color = initialColor;
        }
    }
}