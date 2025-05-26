// DramaManager.cs
// Author: Steph Redwood
// Date: 25/05/2025
//
// This class manages the core logic for simulating character drama in a fictional reality TV setting.
// It randomly generates character interactions based on personality types, tracks popularity,
// and updates the UI accordingly.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DramaManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueBox; // Displays the current drama event
    public TextMeshProUGUI popularityBox; // Displays current popularity of all characters
    public Button nextButton; // Button to progress to the next drama event
    public TextMeshProUGUI relationshipBox; // Displays ongoing relationship changes
    private string relationshipLog = ""; // Stores history of changes

    private List<Character> cast = new List<Character>(); // List of characters in the simulation
    private List<DramaEventData> advancedDramaEvents = new List<DramaEventData>(); // List of possible drama events using advanced logic

    int dramaIndex = 0; // Tracks the current round of drama
    int dramaRounds = 10; // Total number of drama events to simulate in this demo

    void Start()
    {
        SetupCharacters();
        InitializeRelationshipFlags();
        PrintRelationshipFlags();
        SetupAdvancedDramaEvents();

        dramaIndex = 0;
        nextButton.onClick.AddListener(NextDramaEvent);

        // Start with the first event
        NextDramaEvent();
    }


    /// <summary>
    /// Initializes a demo cast of characters with sample names and personality types.
    /// </summary>
    void SetupCharacters()
    {
        cast.Add(new Character("Mario", PersonalityType.Hothead));
        cast.Add(new Character("Thanos", PersonalityType.Loyalist));
        cast.Add(new Character("Peanut", PersonalityType.Romantic));
    }

    /// <summary>
    /// Randomly assigns starting relationship flags between characters (e.g. crushes, friendships).
    /// </summary>
    void InitializeRelationshipFlags()
    {
        foreach (Character a in cast)
        {
            foreach (Character b in cast)
            {
                if (a == b) continue;

                // 50% chance they know each other
                bool knowsEachOther = Random.value > 0.5f;

                if (knowsEachOther)
                {
                    a.SetRelationshipFlag(b, "acquaintances");

                    float roll = Random.value;

                    if (roll < 0.2f)
                        a.SetRelationshipFlag(b, "crush");
                    else if (roll < 0.4f)
                        a.SetRelationshipFlag(b, "friends");
                    else if (roll < 0.5f)
                        a.SetRelationshipFlag(b, "rivals");
                    // else: just acquaintances
                }
            }
        }
    }


    /// <summary>
    /// Creates a set of predefined drama events with associated outcomes.
    /// </summary>
    void SetupAdvancedDramaEvents()
    {
        // 1. Confession and Rejection
        advancedDramaEvents.Add(new DramaEventData(
            id: "confession_rejection",
            tier: "Major",
            requires: new List<string> { "crush" },
            blockedBy: new List<string> { "rejected" },
            preferredA: new List<PersonalityType> { PersonalityType.Romantic },
            preferredB: new List<PersonalityType>(),
            template: "{0} finally confessed their feelings to {1}... but the room went quiet.",
            outcome: (a, b) => {
                a.SetRelationshipFlag(b, "rejected");
                a.popularity -= 5;
                b.popularity += 10;
            }
        ));

        // 2. Argument Over Chores
        advancedDramaEvents.Add(new DramaEventData(
            id: "chores_argument",
            tier: "Light",
            requires: new List<string>(),
            blockedBy: new List<string>(),
            preferredA: new List<PersonalityType> { PersonalityType.Hothead },
            preferredB: new List<PersonalityType> { PersonalityType.Loyalist },
            template: "{0} got into an argument with {1} over who left dishes in the sink.",
            outcome: (a, b) => {
                a.popularity -= 10;
                b.popularity += 5;
                EvolveRelationships(a, b);
            }
        ));

        // 3. Rumour Spread
        advancedDramaEvents.Add(new DramaEventData(
            id: "rumour_confessional",
            tier: "Light",
            requires: new List<string> { "acquaintances" },
            blockedBy: new List<string> { "friends" },
            preferredA: new List<PersonalityType> { PersonalityType.SocialClimber },
            preferredB: new List<PersonalityType>(),
            template: "{0} started a rumour about {1} in the confessional booth!",
            outcome: (a, b) => {
                a.SetRelationshipFlag(b, "gossiped_about");
                a.popularity += 5;
                b.popularity -= 10;
                EvolveRelationships(a, b);
            }
        ));

        // 4. Impression for Laughs
        advancedDramaEvents.Add(new DramaEventData(
            id: "funny_impression",
            tier: "Light",
            requires: new List<string>(),
            blockedBy: new List<string>(),
            preferredA: new List<PersonalityType> { PersonalityType.Wildcard },
            preferredB: new List<PersonalityType>(),
            template: "{0} made a hilarious impression of {1}... the house couldn't stop laughing.",
            outcome: (a, b) => {
                a.popularity += 15;
                b.popularity -= 5;
                EvolveRelationships(a, b);
            }
        ));
    }
    /// <summary>
    /// Modifies relationship flags between two characters based on existing flags and random chances.
    /// </summary>
    void EvolveRelationships(Character a, Character b)
    {
        // FRIENDS to CRUSH (30% chance)
        if (a.HasRelationshipFlag(b, "friends") && Random.value < 0.3f)
        {
            a.SetRelationshipFlag(b, "crush");
            LogRelationshipChange($"{a.name} is starting to catch feelings for {b.name}...");
            Debug.Log($"{a.name} has developed a crush on {b.name}");
        }

        // RIVALS to BETRAYED (20% chance)
        if (a.HasRelationshipFlag(b, "rivals") && Random.value < 0.2f)
        {
            a.SetRelationshipFlag(b, "betrayed");
            LogRelationshipChange($"{a.name} has betrayed {b.name} after building tension.");
            Debug.Log($"{a.name} has become rivals with {b.name}");
        }

        // ACQUAINTANCES to FRIENDS (25% chance)
        if (a.HasRelationshipFlag(b, "acquaintances") && !a.HasRelationshipFlag(b, "friends") && Random.value < 0.25f)
        {
            a.SetRelationshipFlag(b, "friends");
            LogRelationshipChange($"{a.name} and {b.name} are getting closer.");
            Debug.Log($"{a.name} has become friends with {b.name}");
        }

        // CRUSH to REJECTED (if not returned and already confessed)
        if (a.HasRelationshipFlag(b, "crush") && b.HasRelationshipFlag(a, "rejected") && Random.value < 0.2f)
        {
            a.SetRelationshipFlag(b, "rejected");
            a.popularity -= 3;
            LogRelationshipChange($"{a.name} is nursing a broken heart from {b.name}.");
            Debug.Log($"{a.name} was rejected by {b.name}");
        }

        // Random wildcard spark to crush (very low chance)
        if (Random.value < 0.05f)
        {
            a.SetRelationshipFlag(b, "crush");
            LogRelationshipChange($"{a.name} suddenly developed a secret crush on {b.name} out of nowhere.");
            Debug.Log($"{a.name} has developed a random crush on {b.name}");
        }
    }

    /// <summary>
    /// Adds a new line to the relationship log box.
    /// </summary>
    void LogRelationshipChange(string message)
    {
        relationshipLog += message + "\n";
        relationshipBox.text = relationshipLog;
    }


    /// <summary>
    /// Randomly simulates a "match score tier" to control drama tier logic.
    /// Currently used for testing (Match-3 game not yet implemented).
    /// </summary>
    /// <returns>A string representing the outcome tier: "Fail", "Pass", or "Excellent".</returns>
    string SimulateMatchScoreTier()
    {
        int roll = Random.Range(0, 100);
        if (roll < 40) return "Fail";
        if (roll < 70) return "Pass";
        return "Excellent";
    }


    /// <summary>
    /// Advances to the next drama event, or concludes the simulation when all events are complete.
    /// </summary>
    void NextDramaEvent()
    {
        if (dramaIndex < dramaRounds)
        {
            TriggerDramaEvent();
            UpdatePopularityBox();
            dramaIndex++;
        }
        else
        {
            ShowFinalPopularity();
            nextButton.interactable = false; // Disable button after final event
        }
    }


    /// <summary>
    /// Triggers a filtered and tiered drama event based on character flags and match tier logic.
    /// </summary>
    void TriggerDramaEvent()
    {
        string matchTier = SimulateMatchScoreTier();
        LogRelationshipChange($"[Match Result: {matchTier}]");
        Debug.Log($"[Match Result: {matchTier}]");

        // Step 1: Choose two different characters
        Character charA = cast[Random.Range(0, cast.Count)];
        Character charB = cast[Random.Range(0, cast.Count)];
        while (charA == charB)
            charB = cast[Random.Range(0, cast.Count)];

        // Step 2: Filter valid events for this pair and match tier
        List<DramaEventData> possibleEvents = FilterValidDramaEvents(charA, charB, matchTier);

        if (possibleEvents.Count == 0)
        {
            dialogueBox.text = $"{charA.name} and {charB.name} just chilled this round. No drama!";
            return;
        }

        // Step 3: Select an event (random for now, weighted later)
        DramaEventData selectedEvent = possibleEvents[Random.Range(0, possibleEvents.Count)];

        // Collect old popularity for debug log
        int aOldPop = charA.popularity;
        int bOldPop = charB.popularity;

        // Step 4: Apply the event logic
        selectedEvent.outcome.Invoke(charA, charB);

        // Step 5: Display the event
        dialogueBox.text = string.Format(selectedEvent.template, charA.name, charB.name);

        // Collect new popularity for debug log
        int aChange = charA.popularity - aOldPop;
        int bChange = charB.popularity - bOldPop;

        // Document popularity change
        string Signed(int num) => (num >= 0 ? "+" : "") + num.ToString();

        LogRelationshipChange($"{charA.name} popularity change: {Signed(aChange)}");
        LogRelationshipChange($"{charB.name} popularity change: {Signed(bChange)}"); 
        Debug.Log($"{charA.name} popularity change: {Signed(aChange)}");
        Debug.Log($"{charB.name} popularity change: {Signed(bChange)}");
    }


    /// <summary>
    /// Filters the list of all advanced events for a given character pair and match tier.
    /// </summary>
    List<DramaEventData> FilterValidDramaEvents(Character a, Character b, string matchTier)
    {
        List<DramaEventData> valid = new List<DramaEventData>();

        foreach (DramaEventData e in advancedDramaEvents)
        {
            // Skip events that don't match tier requirements
            if (matchTier == "Pass" && e.tier != "Light")
                continue;
            if (matchTier == "Excellent" && !(e.tier == "Light" || e.tier == "Major"))
                continue;

            bool meetsRequirements = true;

            // Check 'requires' flags
            foreach (string req in e.requires)
            {
                if (!a.HasRelationshipFlag(b, req) && !b.HasRelationshipFlag(a, req))
                {
                    meetsRequirements = false;
                    break;
                }
            }

            // Check 'blockedBy' flags
            foreach (string block in e.blockedBy)
            {
                if (a.HasRelationshipFlag(b, block) || b.HasRelationshipFlag(a, block))
                {
                    meetsRequirements = false;
                    break;
                }
            }

            // If all requirements met, add it
            if (meetsRequirements)
                valid.Add(e);
        }

        return valid;
    }


    /// <summary>
    /// Determines which character has the lowest popularity and displays a fake elimination message.
    /// </summary>
    void ShowFinalPopularity()
    {
        Character lowest = cast[0];

        foreach (Character c in cast)
        {
            Debug.Log($"{c.name}: {c.popularity} popularity");

            if (c.popularity < lowest.popularity)
            {
                lowest = c;
            }
        }

        dialogueBox.text = $"After 3 episodes... {lowest.name} has been eliminated from the house.";
    }

    void PrintRelationshipFlags()
    {
        foreach (Character a in cast)
        {
            foreach (var entry in a.relationships)
            {
                string bName = entry.Key;
                List<string> flags = entry.Value;
                Debug.Log($"{a.name} to {bName}: [{string.Join(", ", flags)}]");
            }
        }
    }


    /// <summary>
    /// Updates the UI with the current popularity of all characters.
    /// </summary>
    void UpdatePopularityBox()
    {
        string report = "";

        foreach (Character c in cast)
        {
            report += $"{c.name} ({c.personality}): {c.popularity}\n";
        }

        popularityBox.text = report;
    }

}
