﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UIButtonSoundType
{
    SOUND_TYPE_NONE,
    SOUND_TYPE_CLICK,
    SOUND_TYPE_CLICK_BTN,
}

public class UIButtonSound : MonoBehaviour, IPointerClickHandler
{
    private static Action<string> s_playSoundAction = null;

    public string m_clickSound = "click1";

    public void OnPointerClick(PointerEventData eventData)
    {
        if (s_playSoundAction != null)
        {
            s_playSoundAction(m_clickSound);
        }
    }

    public static void AddPlaySoundAction(Action<string> onClick)
    {
        s_playSoundAction += onClick;
    }

    public void SetClickSound(string sound)
    {
        m_clickSound = sound;
    }
}