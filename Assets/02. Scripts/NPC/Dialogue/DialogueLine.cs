using System;
using UnityEngine;

[Serializable]
public struct DialogueLine
{
    [Tooltip("이 대사를 말하는 NPC의 Data 에셋")]
    public NPCdata speakerData;

    [TextArea(3, 5)]
    [Tooltip("실제 대사 본문 텍스트")]
    public string dialogueText;
}
