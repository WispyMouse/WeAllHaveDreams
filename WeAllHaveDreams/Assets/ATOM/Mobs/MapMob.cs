using Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MapMob : MapObject
{
    public int PlayerSideIndex; // TEMPORARY: Can be set within the editor

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

    public UnityEvent<MapMob> MobUpdated;

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
            MobUpdated.Invoke(this);
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
            MobUpdated.Invoke(this);
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
            MobUpdated.Invoke(this);
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

    public bool IsExhausted
    {
        get
        {
            return !CanMove && !CanAttack && !CanCapture;
        }
    }

    /// <summary>
    /// Gets a value indicating if this mob is now "at rest", no longer considering undos and such.
    /// This can be used to determine if vision should be updated.
    /// </summary>
    public bool Settled
    {
        get
        {
            return !CanMove && !CanAttack;
        }
    }
    public MapCoordinates RestingPosition
    {
        get
        {
            if (Settled)
            {
                return Position;
            }

            if (restingPosition.HasValue)
            {
                return restingPosition.Value;
            }

            return Position;
        }
        set
        {
            restingPosition = value;
        }
    }
    MapCoordinates? restingPosition { get; set; }

    public void RefreshForStartOfTurn()
    {
        bool refresh = TurnManager.CurrentPlayer.PlayerSideIndex == PlayerSideIndex;

        CanMove = refresh;
        CanAttack = refresh;
        MobUpdated.Invoke(this);
    }

    public void ExhaustAllOptions()
    {
        CanMove = false;
        CanAttack = false;
        restingPosition = Position;
    }

    public void ClearForEndOfTurn()
    {
        ExhaustAllOptions();
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

        MobUpdated.Invoke(this);
    }

    public void LoadFromConfiguration(Configuration.MobConfiguration configuration)
    {
        Configuration = configuration;
        gameObject.name = Configuration.Name;

        MobStats = configuration.GetAllMobStats();
        Abilities = configuration.GetSaturatedAbilities();

        MobUpdated.Invoke(this);
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

    public MobMapData GetMapData()
    {
        return new MobMapData() { Position = Position, MobName = Name, Ownership = PlayerSideIndex };
    }
}
