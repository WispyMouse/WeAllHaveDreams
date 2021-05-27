using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecruitsUnitPlayerInput : PlayerInput
{
    public MapStructure Builder;
    public string MobToRecruit;

    public int Cost { get; set; }

    public RecruitsUnitPlayerInput(MapStructure builder, string mobToRecruit)
    {
        Builder = builder;
        MobToRecruit = mobToRecruit;

        Cost = MobLibrary.GetMob(mobToRecruit).ResourceCost;
    }

    public override string LongTitle => $"Recruit {MobToRecruit} for {Cost}";

    public override bool IsPossible(WorldContext givenContext)
    {
        return
            TurnManager.CurrentPlayer.TotalResources >= Cost &&
            givenContext.MobHolder.MobOnPoint(Builder.Position) == null;
    }

    public override IEnumerator Execute(WorldContext worldContext, GameplayAnimationHolder animationHolder)
    {
        DebugTextLog.AddTextToLog($"Recruiting {MobToRecruit} at {Builder.Position.ToString()}", DebugTextLogChannel.Verbose);
        TurnManager.CurrentPlayer.TotalResources -= Cost;
        worldContext.MobHolder.CreateNewUnit(Builder.Position, MobLibrary.GetMob(MobToRecruit), Builder.PlayerSideIndex.Value);
        yield return TurnManager.ResolveEffects();
    }
}
