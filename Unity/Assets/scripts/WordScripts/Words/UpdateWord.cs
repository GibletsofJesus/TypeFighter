﻿using UnityEngine;
using System.Collections;

public class UpdateWord : AbilityWord
{
	private bool updating = false;

	[SerializeField] private float updateRate = 1.0f;
	private float updateProgress = 0.0f;

	private int numUpdates = 0;

	private string[] updateText = new string[]{"updating", "updating.", "updating..", "updating...",};
	private int updateTextIndex = 0;
	[SerializeField] private float updateTextRate = 0.1f;
	private float updateTextCooldown = 0.0f;

    protected override void Start ()
	{
        description = "unlocks and upgrades abilities";
        wordTiers = new string[] {"update.exe"};
		base.Start ();
		wordActive = true;
		wordHUD.Activate ();
        wordHUD.UpdateWord(wordTiers[0]);
	}

	protected override void TriggerBehavior()
    {
		numUpdates = UpdateBehavior.instance.PrepareUpdates ();
		if(numUpdates == 0)
		{
			UpdateBehavior.instance.UnneccessaryUpdate();
		}
		else
        {
            base.TriggerBehavior ();
			updateTextCooldown = 0.0f;
			wordHUD.UpdateWord (updateText[updateTextIndex]);
		}
	}
	
	protected override void Behavior ()
	{
        if(UpdateBehavior.instance.EverythingUnlocked())
        {
            EndBehavior();
            return;
        }
		if(updating)
		{
			UpdateUpdatingText(updateTextIndex + 1);

			updateProgress = Mathf.Min(updateProgress + Time.deltaTime, updateRate);
			float _updatePercent = updateProgress / updateRate;
			wordHUD.UpdateCooldown (((_updatePercent * 1000.0f) - ((_updatePercent * 1000.0f) % pixelCooldown)) / 1000.0f);
			if(updateProgress == updateRate)
			{
				UpdateBehavior.instance.ApplyNextUpdate ();
				--numUpdates;
				updating = false;
			}
		}
		else
		{
			if(numUpdates == 0)
			{
				EndBehavior();
                wordHUD.CooldownFinished();
			}
			else
			{
				updateProgress = 0.0f;
                wordHUD.TriggerSuccess();
				updating = true;
			}
		}
	}
	
	protected override void EndBehavior()
	{
		UpdateBehavior.instance.FinishUpdate ();
		updateTextCooldown = 0.0f; 
		updateTextIndex = 0;
        updating = false;
        updateProgress = 0.0f;
		wordHUD.UpdateWord (wordTiers [currentTier]);
		base.EndBehavior ();
	}

	private void UpdateUpdatingText(int _index)
	{
		updateTextCooldown = Mathf.Min(updateTextCooldown + Time.deltaTime, updateTextRate);
		if(updateTextCooldown == updateTextRate)
		{
			updateTextCooldown = 0.0f; 
			updateTextIndex = _index;
			updateTextIndex = updateTextIndex == updateText.Length ? 0 : updateTextIndex;
			wordHUD.UpdateWord (updateText [updateTextIndex]);
		}
	}

    public override void Reset()
    {
        if (behaviorActive)
        {
            updateTextCooldown = 0.0f;
            updateTextIndex = 0;
            updating = false;
            updateProgress = 0.0f;
            wordHUD.UpdateWord(wordTiers[currentTier]);
            wordHUD.UpdateCooldown(1.0f);
        }
        base.Reset();
    }
}
