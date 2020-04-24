using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Dialogue Lines")]
public class NarrativeClips : ScriptableObject
{
    public AudioClip BrotherMonologue;
    public AudioClip tutorialSelection, tutorialDropping, tutorialSwapPlant;
    public AudioClip[] narrativeLines;
}

