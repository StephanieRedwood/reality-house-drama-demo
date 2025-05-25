// DramaManager.cs

// Author: Steph Redwood
// Date: 25/05/2025

// Manages characters and drama events.
// Chooses random characters and events, displays outcome in UI.

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DramaManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueBox; // Reference to the on-screen text box

    // List of characters in the episode
    List<Character> cast = new List<Character>();

    // List of possible drama events
    List<DramaEvent> dramaEvents = new List<DramaEvent>();

    void Start()
    {
        // Create a few test characters with different personalities
        cast.Add(new Character("Tasha", PersonalityType.Hothead));
        cast.Add(new Character("Liam", PersonalityType.Loyalist));
        cast.Add(new Character("Zoe", PersonalityType.Romantic));

        // Define drama event 1: a heated argument
        dramaEvents.Add(new DramaEvent(
            "{0} got into an argument with {1} over who left dishes in the sink.",
            (a, b) => {
                a.popularity -= 10; // Arguer loses popularity
                b.popularity += 5;  // Other person gains sympathy
            }
        ));

        // Define drama event 2: romantic rejection
        dramaEvents.Add(new DramaEvent(
            "{0} confessed feelings to {1}... but got rejected!",
            (a, b) => {
                a.popularity -= 5; // Rejected character loses popularity
                b.popularity += 10; // Rejector seems confident
            }
        ));

        // Pick two random different characters from the cast
        Character charA = cast[Random.Range(0, cast.Count)];
        Character charB = cast[Random.Range(0, cast.Count)];
        while (charA == charB)
        {
            charB = cast[Random.Range(0, cast.Count)];
        }

        // Pick a random drama event
        DramaEvent selectedEvent = dramaEvents[Random.Range(0, dramaEvents.Count)];

        // Apply event outcome to characters
        selectedEvent.applyOutcome(charA, charB);

        // Format and display the drama text in the UI
        dialogueBox.text = string.Format(selectedEvent.description, charA.name, charB.name);
    }
}
