﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelText : MonoBehaviour
{
    private static LevelText levelText = null;
    public static LevelText instance { get { return levelText; } }

    [SerializeField] private Text displayText = null;

    private Vector2 aboveScreen;
    private Vector2 belowScreen;
    private Vector2 tutorialScreen;

    private bool showText = false;

    private bool tutorialText = true;

    [SerializeField] private float fallRate = 10.0f;
    [SerializeField] private float fadeRate = 2.0f;

    private string textBuffer = string.Empty;
    private int fontBuffer = 32;

    private bool iconsVisible = true;
    [SerializeField] private Image buttonIcon = null;
    [SerializeField] private Image joystickIcon = null;
    [SerializeField] private Image[] joystickBalls = null;

    private bool fadeText = false;

    private void Awake()
    {
        levelText = this;
    }

    private void Start () 
    {
        aboveScreen = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.15f, 0.5f));
        displayText.rectTransform.position = aboveScreen;
        belowScreen = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, -0.25f,0.5f));
        tutorialScreen = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.95f, 0.5f));
        //OffScreen.z = displayText.rectTransform.position.z; 
    }

    private void Update ()
    {
        if (showText)
        {
            if (tutorialText)
            {
                if (displayText.rectTransform.position.y >= tutorialScreen.y)
                {
                    displayText.rectTransform.position = new Vector3(displayText.rectTransform.position.x, displayText.rectTransform.position.y - (fallRate * Time.deltaTime), displayText.rectTransform.position.z);
                }
                if (fadeText)
                {
                    FadeText();
                }
            }
            else
            {
                if (displayText.rectTransform.position.y >= belowScreen.y)
                {
                    displayText.rectTransform.position = new Vector3(displayText.rectTransform.position.x, displayText.rectTransform.position.y - (fallRate * Time.deltaTime), displayText.rectTransform.position.z);
                }
                else
                {
                    showText = false;
                    if (textBuffer != string.Empty)
                    {
                        displayText.rectTransform.position = aboveScreen;
                        SetText(textBuffer, fontBuffer);
                        textBuffer = string.Empty;
                    }
                    showText = true;
                }
            }
        }
	}

    public void TutorialElementFinished(bool _tutorialCompleted)
    {
        fadeText = true;
        if(_tutorialCompleted)
        {
            SetText("LEVEL 1");
        }
    }

    private void FadeText()
    {
        Color _colour = displayText.color;
        _colour.a -= (Time.deltaTime*fadeRate);
        if (_colour.a <= 0.0f)
        {
            fadeText = false;
            displayText.rectTransform.position = aboveScreen;
            _colour.a = 1.0f;
            showText = false;
            if (textBuffer != string.Empty)
            {
                SetText(textBuffer, fontBuffer);
                textBuffer = string.Empty;
                showText = true;
            }
            DisableIcons();
        }
        if(iconsVisible)
        {
            buttonIcon.color = _colour;
            joystickIcon.color = _colour;
            Color _ballColour = joystickBalls[0].color;
            _ballColour.a = _colour.a;
            for (int i = 0; i < joystickBalls.Length; ++i)
            {
                joystickBalls[i].color = _ballColour;
            }
        }
        displayText.color = _colour;
    }

    public void DisableIcons()
    {
        iconsVisible = false;
        buttonIcon.gameObject.SetActive(false);
        joystickIcon.gameObject.SetActive(false);
    }

    public void SetText(string _text, int _font = 128)
    {
        if (showText)
        {
            textBuffer = _text;
            fontBuffer = _font;
        }
        else
        {
            if (_text == "LEVEL 1")
            {
                tutorialText = false;
            }
            displayText.text = _text;
            displayText.fontSize = _font;
        }
    }

    public void ShowText()
    {
        if (!showText)
        {
            displayText.rectTransform.position = aboveScreen;
            showText = true;
        }
    }
}
