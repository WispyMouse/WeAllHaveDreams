using Configuration;
using System.Collections.Generic;

public class RecruitsAbility : StructureConfigurationAbility
{
    public override IEnumerable<PlayerInput> GetPossiblePlayerInputs(MapStructure structure)
    {
        List<PlayerInput> inputs = new List<PlayerInput>();

        foreach (MobConfiguration mob in MobLibrary.MobsWithTags(Arguments))
        {
            inputs.Add(new RecruitsUnitPlayerInput(structure, mob.DevelopmentName));
        }

        return inputs;
    }
}
