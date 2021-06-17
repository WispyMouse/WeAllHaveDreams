using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Renders a Mob. Intended to be attached to a MapMob.
/// </summary>
public class MobVisuals : MonoBehaviour
{
    public SpriteRenderer Renderer;

    public string LastAppearance { get; set; }
    public int LastSideIndex { get; set; }

    /// <summary>
    /// The Color to tint the sprite with if this Mob is in a default state. Should be white, but you can play with it.
    /// </summary>
    public Color DefaultColor;

    /// <summary>
    /// The Color to tint the sprite with if this Mob is exhausted. Conveys that the Mob no longer has actions.
    /// </summary>
    public Color ExhaustedColor;

    // TEMPORARY: This should definitely be in its own class
    public SpriteRenderer HitPointsVisual;
    public Sprite[] HitPointsNumerics;

    public Transform RemindersParent;
    public float ReminderHorizontalSpacing { get; set; } = .35f;

    Dictionary<string, Reminder> Reminders { get; set; } = new Dictionary<string, Reminder>();

    private void Start()
    {
        MobVisualsConfiguration config = ConfigurationLoadingEntrypoint.GetConfigurationData<MobVisualsConfiguration>().FirstOrDefault();
        if (config != null)
        {
            ColorUtility.TryParseHtmlString(config.DefaultColor, out DefaultColor);
            ColorUtility.TryParseHtmlString(config.ExhaustedColor, out ExhaustedColor);
        }
    }

    public void OnMobUpdated(MapMob toDisplay)
    {
        UpdateHitPointVisual(toDisplay);
        SetReminders(toDisplay);
        SettleReminderOrdering();
        SetSpriteColor(toDisplay);

        if (LastAppearance != toDisplay.Configuration.Appearance || LastSideIndex != toDisplay.MyPlayerSide.PlayerSideIndex)
        {
            if (toDisplay.MyPlayerSide != null)
            {
                Renderer.sprite = MobLibrary.GetMobSprite(toDisplay.Configuration.Appearance, toDisplay.MyPlayerSide);

                LastAppearance = toDisplay.Configuration.Appearance;
                LastSideIndex = toDisplay.MyPlayerSide.PlayerSideIndex;
            }
        }
    }

    void SetReminders(MapMob toDisplay)
    {
        SetReminder(nameof(MapMob.CanMove), toDisplay.CanMove);
        SetReminder(nameof(MapMob.CanAttack), toDisplay.CanAttack);
    }

    void SettleReminderOrdering()
    {
        List<Reminder> orderedReminders = Reminders.Values.OrderBy(r => r.ReminderTag).ToList();

        for (int ii = 0; ii < Reminders.Count; ii++)
        {
            float offset = (float)ii * ReminderHorizontalSpacing;
            Reminder thisReminder = orderedReminders[ii];
            thisReminder.transform.localPosition = Vector3.left * offset;
        }
    }

    void UpdateHitPointVisual(MapMob toDisplay)
    {
        if (toDisplay.HitPoints == toDisplay.MaxHitPoints)
        {
            HitPointsVisual.gameObject.SetActive(false);
            return;
        }

        int roundedValue = System.Math.Min(10, (int)System.Math.Ceiling(toDisplay.HitPoints));
        HitPointsVisual.sprite = HitPointsNumerics[roundedValue];
        HitPointsVisual.gameObject.SetActive(true);
    }

    void SetSpriteColor(MapMob toDisplay)
    {
        if (toDisplay.IsExhausted && TurnManager.GameIsInProgress && (TurnManager.CurrentPlayer != null && TurnManager.CurrentPlayer == toDisplay.MyPlayerSide))
        {
            Renderer.color = ExhaustedColor;
        }
        else
        {
            Renderer.color = DefaultColor;
        }
    }

    void SetReminder(string reminderTag, bool active)
    {
        if (active)
        {
            ShowReminder(reminderTag);
        }
        else
        {
            HideReminder(reminderTag);
        }
    }

    void ShowReminder(string reminderTag)
    {
        if (Reminders.ContainsKey(reminderTag))
        {
            Reminders[reminderTag]?.Show();
        }
        else
        {
            Reminder newReminder = ReminderFactory.GetReminder(this, reminderTag);
            Reminders.Add(reminderTag, newReminder);
        }

        SettleReminderOrdering();
    }

    void HideReminder(string reminderTag)
    {
        if (Reminders.ContainsKey(reminderTag))
        {
            Reminders[reminderTag]?.Hide();
        }

        SettleReminderOrdering();
    }
}
