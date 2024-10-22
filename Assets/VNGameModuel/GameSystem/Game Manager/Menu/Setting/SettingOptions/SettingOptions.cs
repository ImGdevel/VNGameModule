using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingOption : MonoBehaviour
{
    public abstract void LoadSettingsToUI(Settings settings);
    public abstract void ApplyUIToSettings(Settings settings);
}
