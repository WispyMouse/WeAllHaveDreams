using Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapMob : MapObject
{
    public int PlayerSideIndex; // TEMPORARY: Can be set within the editor
    public Transform RemindersParent;
    public float ReminderHorizontalSpacing { get; set; } = .35f; // TEMPORARY: This is a UI thing and should be somewhere else

    public MobConfiguration Configuration { get; set; }
    public string Name => Configuration.Name;
    public string DevelopmentName => Configuration.DevelopmentName;

    public int ResourceCost => (int)(GetCurrentMobStat(nameof(ResourceCost)));
    public int MoveRange => (int)(GetCurrentMobStat(nameof(MoveRange)));
    public int AttackRange => (int)(GetCurrentMobStat(nameof(AttackRange)));
    public int SightRange => (int)(GetCurrentMobStat(nameof(SightRange)));
    public decimal DamageOutputRatio => GetCurrentMobStat(nameof(DamageOutputRatio));
    public decimal DamageReductionRatio => GetCurrentMobStat(nameof(DamageReductionRatio));
    Dictionary<string, MobStat> MobStats { get; set; } = new Dictionary<string, MobStat>();
    public IEnumerable<MobConfigurationAbility> Abilities { get; private set; } = new List<MobConfigurationAbility>();

    public IEnumerable<StatAdjustment> ActiveStatAdjustments { get; private set; } = new List<StatAdjustment>();

    // TEMPORARY: This should definitely be in its own class
    public SpriteRenderer HitPointsVisual;
    public Sprite[] HitPointsNumerics;

    public SpriteRenderer Renderer;

    public decimal MaxHitPoints { get; set; } = 10.0M;
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
            return AttackPowerAtHitPoints(HitPoints);
        }
    }

    public decimal AttackPowerAtHitPoints(decimal hitPoints)
    {
        return System.Math.Ceiling(hitPoints) * DamageOutputRatio;
    }

    public void UpdateHitPointVisual()
    {
        if (HitPoints == MaxHitPoints)
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
        Renderer.sprite = MobLibrary.GetMobSprite(Configuration.Appearance, PlayerSideIndex);
    }

    public int CurrentCapturePoints
    {
        get
        {
            return (int)System.Math.Ceiling(HitPoints);
        }
    }

    public void SetOwnership(int side)
    {
        PlayerSideIndex = side;
        Renderer.sprite = MobLibrary.GetMobSprite(Configuration.Appearance, PlayerSideIndex);
    }

    public void LoadFromConfiguration(Configuration.MobConfiguration configuration)
    {
        Configuration = configuration;
        gameObject.name = Configuration.Name;

        MobStats = configuration.GetAllMobStats();
        Abilities = configuration.GetSaturatedAbilities();
    }

    public void CalculateStandingStatAdjustments(MapFeature onFeature)
    {
        // HACK: Temporarily, the only adjustments possible are from Features, so just calculate from that
        if (onFeature == null)
        {
            ActiveStatAdjustments = new List<StatAdjustment>();
            return;
        }

        ActiveStatAdjustments = onFeature.StatAdjustmentsForMob(this);
    }

    public decimal GetCurrentMobStat(string statName)
    {
        MobStat foundStat;

        if (!MobStats.TryGetValue(statName, out foundStat))
        {
            DebugTextLog.AddTextToLog($"{statName} was asked for as a MobStat, but it is not in the dictionary.", DebugTextLogChannel.RuntimeError);
            return 0;
        }

        foreach (StatAdjustment curAdjustment in ActiveStatAdjustments.Where(adjustment => adjustment.StatToChange == statName))
        {
            foundStat = foundStat.ApplyAdjustment(curAdjustment);
        }

        return foundStat.StatValue;
    }
}
