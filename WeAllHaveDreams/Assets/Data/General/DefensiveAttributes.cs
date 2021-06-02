using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DefensiveAttributes
{
    /// <summary>
    /// The tags that this defensive value applies to.
    /// If this list is empty, it's assumed it applies to everything.
    /// </summary>
    public IEnumerable<string> Tags = Array.Empty<string>();

    /// <summary>
    /// The percentage of damage that you take on this tile.
    /// 1 means you take 100% damage. Essentially no mitigation.
    /// 0 means you would take no damage, regardless of the attack's raw output.
    /// 0.8 means you take 80% of the damage coming in, acting as 20% damage mitigation.
    /// NOTE: JSON requires 0.2, rather than .2, in the text file.
    /// </summary>
    public decimal DefensiveRatio;

    /// <summary>
    /// Determines if the provided set of tags apply to this attribute.
    /// </summary>
    /// <param name="checkedTags">The tags to check.</param>
    /// <returns>True if this should apply, false otherwise.</returns>
    public bool TagsApply(IEnumerable<string> checkedTags)
    {
        // If we have no tags, this applies to everything
        if (Tags == null || !Tags.Any())
        {
            return true;
        }

        // If we weren't provided any tags, then this doesn't apply as long this has any tags
        if (checkedTags == null || !checkedTags.Any())
        {
            return false;
        }

        foreach (string tag in checkedTags)
        {
            if (Tags.Contains(tag, System.StringComparer.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
