// Character.cs

// Author: Steph Redwood
// Date: 25/05/2025

// Represents a contestant in Reality House.
// Holds name, personality, and popularity score.

using System.Collections.Generic;

public enum PersonalityType
{
    Romantic,
    Hothead,
    SocialClimber,
    Loyalist,
    Wildcard
}

public class Character
{
    public string name;
    public PersonalityType personality;
    public int popularity;

    // Relationships with others
    public Dictionary<string, List<string>> relationships = new Dictionary<string, List<string>>();

    // Personal flags
    public List<string> personalFlags = new List<string>();

    public Character(string name, PersonalityType personality)
    {
        this.name = name;
        this.personality = personality;
        this.popularity = 50;
    }

    public void SetRelationshipFlag(Character other, string flag)
    {
        if (!relationships.ContainsKey(other.name))
            relationships[other.name] = new List<string>();

        if (!relationships[other.name].Contains(flag))
            relationships[other.name].Add(flag);
    }

    public bool HasRelationshipFlag(Character other, string flag)
    {
        return relationships.ContainsKey(other.name) && relationships[other.name].Contains(flag);
    }

    public void SetPersonalFlag(string flag)
    {
        if (!personalFlags.Contains(flag))
            personalFlags.Add(flag);
    }

    public bool HasPersonalFlag(string flag)
    {
        return personalFlags.Contains(flag);
    }
}
