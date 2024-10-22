using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Music List", menuName = "ScriptableObject/Music List", order = 0)]
public class MusicList : ScriptableObject
{
    [System.Serializable]
    public class Music
    {
        public AudioClip audio;
        public string name;
        [Range(0f, 1f)]
        public float volume = 1.0f;
    }

    [Header("Music Clips")]
    public List<Music> musicClips = new List<Music>(); //등록된 배경음악들
}