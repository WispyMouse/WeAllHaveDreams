using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMob : MonoBehaviour
{
    public int PlayerSideIndex; // TEMPORARY: Can be set within the editor

    public Vector3Int Position { get; set; }

    public int MoveRange => 3; // TEMPORARY: Just a static move value
    public int AttackRange => 1; // TEMPORARY: Again, static value

    public decimal HitPoints { get; set; } = 10.0M;
    public decimal DamageRatio { get; set; } = .4M;

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

    Dictionary<string, Reminder> Reminders { get; set; } = new Dictionary<string, Reminder>();

    public void SettleIntoGrid()
    {
        Vector3Int nearestStartPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.z));
        SetPosition(nearestStartPosition);
    }

    public void SetPosition(Vector3Int toPosition)
    {
        Position = toPosition;
        transform.position = toPosition;
    }

    public void RefreshForStartOfTurn()
    {
        CanMove = true;
        ShowReminder(nameof(CanMove));

        CanAttack = true;
        ShowReminder(nameof(CanAttack));
    }

    public void ClearForEndOfTurn()
    {
        CanMove = false;
        CanAttack = false;
        HideAllReminders();
    }

    public void ShowReminder(string reminderTag)
    {
        if (Reminders.ContainsKey(reminderTag))
        {
            Reminders[reminderTag].Show();
        }
        else
        {
            Reminder newReminder = ReminderFactory.GetReminder(this, reminderTag);
            Reminders.Add(reminderTag, newReminder);
        }
    }

    void HideReminder(string reminderTag)
    {
        if (Reminders.ContainsKey(reminderTag))
        {
            Reminders[reminderTag].Hide();
        }
    }

    void HideAllReminders()
    {
        foreach (string key in Reminders.Keys)
        {
            HideReminder(key);
        }
    }

    public decimal CurrentAttackPower
    {
        get
        {
            return HitPoints * DamageRatio;
        }
    }
}
