using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class SoundData
{
    public string name;
    public AudioClip clip;
}

[CreateAssetMenu()]
public class SoundSO : ScriptableObject
{
    public SoundData[] sounds;

    public AudioClip GetClipByName(string soundName)
    {
        return sounds.FirstOrDefault(s => s.name == soundName)?.clip;
    }
}
