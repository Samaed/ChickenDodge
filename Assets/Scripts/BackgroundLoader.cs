using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundLoader : MonoBehaviour
{
    class EntryDetails
    {
        public string spriteName;
        public bool isAnimated;
        public int frameSkip;
    }

    static readonly IDictionary<char, EntryDetails> EntryMap = new Dictionary<char, EntryDetails> {
		{ 'T', new EntryDetails{ spriteName = "T1", isAnimated = false, frameSkip = 1 } },
		{ 'F', new EntryDetails{ spriteName = "F", isAnimated = true, frameSkip = 8 } },
		{ 'G', new EntryDetails{ spriteName = "P1", isAnimated = false, frameSkip = 1 } },
		{ '\\', new EntryDetails{ spriteName = "FE2", isAnimated = false, frameSkip = 1 } },
		{ '-', new EntryDetails{ spriteName = "FE1", isAnimated = false, frameSkip = 1 } },
		{ '|', new EntryDetails{ spriteName = "FE3", isAnimated = false, frameSkip = 1 } }
	};

    [SerializeField]
    TextAsset mDescription;

    [SerializeField]
    SpriteSheet mSpriteSheet;

    [SerializeField]
    float mScale = 16.0f;

    public void Start()
    {
        float y = 0;
        foreach (var line in mDescription.text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            float x = 0;
            foreach (var c in line)
            {
                if (EntryMap.ContainsKey(c))
                {
                    var entry = EntryMap[c];
                    var spriteGO = new GameObject();

                    if (c == '\\' || c == '-' || c == '|')
                    {
                        spriteGO.tag = Collision.TAG;
                    }

                    var newSprite = spriteGO.AddComponent<Sprite>();
                    newSprite.mSpriteSheet = mSpriteSheet;
                    newSprite.mSpriteName = entry.spriteName;
                    newSprite.mIsAnimated = entry.isAnimated;
                    newSprite.mFrameSkip = entry.frameSkip;
                    newSprite.transform.parent = transform;
                    newSprite.transform.localPosition = new Vector3(x, y, y * 0.01f);
                }
                x += mScale;
            }
            y -= mScale;
        }
    }
}
