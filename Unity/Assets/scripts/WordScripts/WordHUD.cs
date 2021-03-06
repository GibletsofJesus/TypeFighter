using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WordHUD : MonoBehaviour 
{
	[SerializeField] private Text text = null;
	[SerializeField] private Image backgroundImage = null;
	[SerializeField] private Image cooldownImage = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private RectTransform originalIconPosition = null;
    [SerializeField]
    Animator shimmerAnimator;

    private bool displayIcon = true;
    private float lerpValue = 0;
    private float lerpSpeed = 2.0f;

    private void Start()
    {
        //originalIconPosition.position = iconImage.rectTransform.position;
    }

    public void UpdateCooldown(float _fill)
	{
		cooldownImage.fillAmount = _fill;
	}

	public void TriggerSuccess()
	{
		cooldownImage.fillAmount = 0.0f;
        displayIcon = false;
	}

    public void CooldownFinished()
    {
         displayIcon = true;
        shimmerAnimator.Play("shimmer_right");
    }

	public void Deactivate()
	{
        text.text = "";
		text.color = HUDData.instance.deactivateColour;
		backgroundImage.sprite = HUDData.instance.deactivateBackground;
		cooldownImage.sprite = HUDData.instance.deactivateCooldown;
        displayIcon = false;
    }

	public void Activate()
	{
		text.color = HUDData.instance.activateColour;
		backgroundImage.sprite = HUDData.instance.activateBackground;
		cooldownImage.sprite = HUDData.instance.activateCooldown;
        CooldownFinished();
    }

	public void UpdateWord(string _word)
	{
		text.text = _word;
	}

    private void Update()
    {
        if (GameStateManager.instance.state == GameStateManager.GameState.Gameplay)
        {
            if (!displayIcon)
            {
                lerpValue -= Time.deltaTime * lerpSpeed;
                lerpValue = lerpValue <= 0 ? 0 : lerpValue;
            }
            else
            {
                lerpValue += Time.deltaTime * lerpSpeed;
                lerpValue = lerpValue >= 1 ? 1 : lerpValue;
            }
            iconImage.rectTransform.localPosition = Vector3.Lerp((Vector3.left * iconImage.rectTransform.rect.width), Vector3.zero, lerpValue);
        }
    }
}
