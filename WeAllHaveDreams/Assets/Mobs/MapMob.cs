using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMob : MonoBehaviour
{
    public int PlayerSideIndex; // TEMPORARY: Can be set within the editor

    public Vector3Int Position { get; set; }
    public int MoveRange => 3; // TEMPORARY: Just a static move value

    public bool CanMove { get; set; }
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
    }

    public void ClearForEndOfTurn()
    {
        CanMove = false;
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

    public void HideReminder(string reminderTag)
    {
        if (Reminders.ContainsKey(reminderTag))
        {
            Reminders[reminderTag].Hide();
        }
    }

    public void HideAllReminders()
    {
        foreach (string key in Reminders.Keys)
        {
            HideReminder(key);
        }
    }
}
