// DramaEventData.cs
// Author: Steph Redwood
// Defines a structured drama event with condition-based logic, ideal for major dynamic events.

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DramaEventData
{
    public string id;
    public string tier; // "Light" or "Major"
    public List<string> requires;
    public List<string> blockedBy;
    public List<PersonalityType> preferredA;
    public List<PersonalityType> preferredB;
    public string template;
    public Action<Character, Character> outcome;

    public DramaEventData(
        string id,
        string tier,
        List<string> requires,
        List<string> blockedBy,
        List<PersonalityType> preferredA,
        List<PersonalityType> preferredB,
        string template,
        Action<Character, Character> outcome)
    {
        this.id = id;
        this.tier = tier;
        this.requires = requires;
        this.blockedBy = blockedBy;
        this.preferredA = preferredA;
        this.preferredB = preferredB;
        this.template = template;
        this.outcome = outcome;
    }
}
