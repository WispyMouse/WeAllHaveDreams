using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReminderFactory : SingletonBase<ReminderFactory>
{
    public Reminder ReminderPF;

    public static Reminder GetReminder(MapMob onMob, string reminderTag)
    {
        Reminder newReminder = Instantiate(Singleton.ReminderPF, onMob.transform);
        return newReminder;
    }
}
