using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaggedAttribute
{
    /// <summary>
    /// The tags that this defensive value applies to.
    /// If this list is empty, it's assumed it applies to everything.
    /// </summary>
    public IEnumerable<string> Tags = Array.Empty<string>();

    /// <summary>
    /// The importance of this item in comparison to others.
    /// This is used to determine what to use if there's a conflict.
    /// </summary>
    public int Priority;

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
