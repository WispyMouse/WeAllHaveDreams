using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReminderFactory : SingletonBase<ReminderFactory>
{
    public Reminder ReminderPF;

    public Sprite MoveReminderSprite;
    public Sprite AttackReminderSprite;

    public static Reminder GetReminder(MapMob onMob, string reminderTag)
    {
        Reminder newReminder = Instantiate(Singleton.ReminderPF, onMob.RemindersParent);

        switch (reminderTag)
        {
            case nameof(MapMob.CanMove):
                newReminder.Renderer.sprite = Singleton.MoveReminderSprite;
                break;
            case nameof(MapMob.CanAttack):
                newReminder.Renderer.sprite = Singleton.AttackReminderSprite;
                break;
        }

        return newReminder;
    }
}
