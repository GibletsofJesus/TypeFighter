﻿using UnityEngine;
using System.Collections;

public class Shortcut : Word
{
    private void Awake()
    {
        thisWord = "shortcut";
        wordActive = true;
    }

    protected override void Start()
    {

    }

    protected override void TriggerBehavior()
    {
        GameStateManager.instance.cheat = true;
        EnemyManager.instance.ShortCut();
    }

    protected override void Behavior()
    {

    }
}
