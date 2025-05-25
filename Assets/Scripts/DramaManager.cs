// DramaManager.cs
// Author: Steph Redwood
// Date: 25/05/2025

// Manages characters and drama events.
// Chooses random characters and events, displays outcome in UI.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DramaManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueBox; // Reference to the dialogue text box
    public TextMeshProUGUI popularityBox; // Reference to the character popularity text box
    public Button nextButton; // Reference to the next button

    // List of characters in the episode
    List<Character> cast = new List<Character>();

    // List of possible drama events
    List<DramaEvent> dramaEvents = new List<DramaEvent>();

    void Start()
    {
        SetupCharacters();
        SetupDramaEvents();
        StartCoroutine(RunDramaSequence());
    }

    // Create fake characters for the demo
    void SetupCharacters()
    {
        cast.Add(new Character("Tasha", PersonalityType.Hothead));
        cast.Add(new Character("Liam", PersonalityType.Loyalist));
        cast.Add(new Character("Zoe", PersonalityType.Romantic));
    }

    // Create a few drama event templates
    void SetupDramaEvents()
    {
        dramaEvents.Add(new DramaEvent(
            "{0} got into an argument with {1} over who left dishes in the sink.",
            (a, b) => {
                a.popularity -= 10;
                b.popularity += 5;
            }
        ));

        dramaEvents.Add(new DramaEvent(
            "{0} confessed feelings to {1}... but got rejected!",
            (a, b) => {
                a.popularity -= 5;
                b.popularity += 10;
            }
        ));

        dramaEvents.Add(new DramaEvent(
            "{0} started a rumor about {1} in the confessional booth!",
            (a, b) => {
                a.popularity += 5;
                b.popularity -= 10;
            }
        ));

        dramaEvents.Add(new DramaEvent(
            "{0} made a hilarious impression of {1}... the house couldn't stop laughing.",
            (a, b) => {
                a.popularity += 15;
                b.popularity -= 5;
            }
        ));
    }

    // Run multiple events one after another
    IEnumerator RunDramaSequence()
    {
        for (int i = 0; i < 3; i++)
        {
            TriggerDramaEvent(); // Do one drama event
            yield return new WaitForSeconds(3); // Wait before next one
        }

        ShowFinalPopularity();
    }

    // Trigger a single drama event
    void TriggerDramaEvent()
    {
        Character charA = cast[Random.Range(0, cast.Count)];
        Character charB = cast[Random.Range(0, cast.Count)];
        while (charA == charB)
        {
            charB = cast[Random.Range(0, cast.Count)];
        }

        DramaEvent selectedEvent = dramaEvents[Random.Range(0, dramaEvents.Count)];
        selectedEvent.applyOutcome(charA, charB);
        dialogueBox.text = string.Format(selectedEvent.description, charA.name, charB.name);
        
        // Show updated popularity
        UpdatePopularityBox();
    }

    // Show who was eliminated
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
