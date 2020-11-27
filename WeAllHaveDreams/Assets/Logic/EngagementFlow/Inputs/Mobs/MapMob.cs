using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapMob : MapObject
{
    public int PlayerSideIndex; // TEMPORARY: Can be set within the editor
    public Transform RemindersParent;
    public float ReminderHorizontalSpacing { get; set; } = .35f; // TEMPORARY: This is a UI thing and should be somewhere else

    public int MoveRange => 4; // TEMPORARY: Just a static move value
    public int AttackRange => 1; // TEMPORARY: Again, static value

    // TEMPORARY: This should definitely be in its own class
    public SpriteRenderer HitPointsVisual;
    public Sprite[] HitPointsNumerics;

    public SpriteRenderer Renderer;
    public Sprite[] SideSprites;

    public decimal HitPoints
    {
        get
        {
            return _hitPoints;
        }
        set
        {
            _hitPoints = value;
            UpdateHitPointVisual();
        }
    }
    private decimal _hitPoints { get; set; } = 10.0M;

    public decimal DamageRatio { get; set; } = .6M;

    public bool CanMove
    {
        get
        {
            return _canMove;
        }
        set
        {
            _canMove = value;

            if (value)
            {
                ShowReminder(nameof(CanMove));
            }
            else
            {
                HideReminder(nameof(CanMove));
            }
        }
    }
    private bool _canMove { get; set; }

    public bool CanAttack
    {
        get
        {
            return _canAttack;
        }
        set
        {
            _canAttack = value;

            if (value)
            {
                ShowReminder(nameof(CanAttack));
            }
            else
            {
                HideReminder(nameof(CanAttack));
            }
        }
    }
    private bool _canAttack { get; set; }

    public bool CanCapture
    {
        get
        {
            return CanMove || CanAttack;
        }
    }

    Dictionary<string, Reminder> Reminders { get; set; } = new Dictionary<string, Reminder>();

    public void RefreshForStartOfTurn()
    {
        CanMove = true;
        ShowReminder(nameof(CanMove));

        CanAttack = true;
        ShowReminder(nameof(CanAttack));
    }

    public void ExhaustAllOptions()
    {
        CanMove = false;
        CanAttack = false;
    }

    public void ClearForEndOfTurn()
    {
        CanMove = false;
        CanAttack = false;
    }

    public void ShowReminder(string reminderTag)
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

    void HideAllReminders()
    {
        foreach (string key in Reminders.Keys)
        {
            HideReminder(key);
        }
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

    public decimal CurrentAttackPower
    {
        get
        {
            return System.Math.Ceiling(HitPoints) * DamageRatio;
        }
    }

    public void UpdateHitPointVisual()
    {
        if (HitPoints == 10)
        {
            HitPointsVisual.gameObject.SetActive(false);
            return;
        }

        int roundedValue = System.Math.Min(10, (int)System.Math.Ceiling(HitPoints));
        HitPointsVisual.sprite = HitPointsNumerics[roundedValue];
        HitPointsVisual.gameObject.SetActive(true);
    }

    public void SetUnitVisuals()
    {
        Renderer.sprite = SideSprites[PlayerSideIndex];
    }

    public int CurrentCapturePoints
    {
        get
        {
            return (int)System.Math.Ceiling(HitPoints);
        }
    }
}
