﻿using UnityEngine;
using System.Collections;

public class Adblocker : Word
{
    private void Awake()
    {
        thisWord = "adblocker";
        wordActive = true;
    }

    protected override void Start()
    {

    }

    protected override void TriggerBehavior()
    {
        //Player.instance.IncreaseScore(-1337);
        AdManager.instance.EnableAdBlock();
    }

    protected override void Behavior()
    {

    }
}
