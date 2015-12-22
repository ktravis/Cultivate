using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChoiceController : MonoBehaviour {
    public Text leftChoiceText;
    public Text rightChoiceText;
    public Arrow leftArrow;
    public Arrow rightArrow;
    public RawImage timeoutBar;

    public void Hide() {
        SetArrowVisibility(false, false);
        SetTextVisibility(false, false);
        timeoutBar.canvasRenderer.SetAlpha(0.0f);
        StopAllCoroutines();
    }
    public void SetArrowVisibility(bool left, bool right) {
        leftArrow.SetVisible(left);
        rightArrow.SetVisible(right);
        StopAllCoroutines();
    }
    public void SetTextVisibility(bool left, bool right) {
        Color c = leftChoiceText.color;
        c.a = left ? 1.0f : 0.0f;
        leftChoiceText.color = c;
        c = rightChoiceText.color;
        c.a = right ? 1.0f : 0.0f;
        rightChoiceText.color = c;
        StopAllCoroutines();
    }
    public void SetChoices(Choice left, Choice right) {
        StopAllCoroutines();
        SetArrowVisibility(left != null, right != null);
        SetTextVisibility(left != null, right != null);
        
        rightChoiceText.text = right != null ? right.text : "";
        leftChoiceText.text = left != null ? left.text : "";
    }
    public IEnumerator SetTimeout(float duration) {
        float t = 0;
        timeoutBar.canvasRenderer.SetAlpha(1.0f);
        var scale = timeoutBar.rectTransform.localScale;
        scale.x = 1;
        while (t <= 1.0f) {
            t += Time.deltaTime / duration;
            scale.x = (1 - t);
            timeoutBar.rectTransform.localScale = scale;
            yield return null;
        }
        scale.x = 1;
        timeoutBar.canvasRenderer.SetAlpha(0.0f);
        timeoutBar.rectTransform.localScale = scale;
    }
}
