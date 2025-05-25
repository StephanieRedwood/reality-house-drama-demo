// DramaEvent.cs

// Author: Steph Redwood
// Date: 25/05/2025

// Represents a dramatic event between two characters.
// Stores event description and outcome logic.

using System;

public class DramaEvent
{
    public string description; // Text shown in cutscene
    public Action<Character, Character> applyOutcome; // Logic applied to characters (e.g., popularity changes)

    // Constructor to create a new drama event
    public DramaEvent(string desc, Action<Character, Character> outcome)
    {
        description = desc;
        applyOutcome = outcome;
    }
}
