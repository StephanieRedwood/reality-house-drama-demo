// Character.cs

// Author: Steph Redwood
// Date: 25/05/2025

// Represents a contestant in Reality House.
// Holds name, personality, and popularity score.

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

    // Constructor to create a new character
    public Character(string name, PersonalityType personality)
    {
        this.name = name;
        this.personality = personality;
        this.popularity = 50; // Start with neutral popularity
    }
}
