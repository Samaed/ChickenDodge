using System;
using UnityEngine;

public class Rupee : MonoBehaviour
{
    [SerializeField]
    internal SpriteSheet mSpriteSheet;

    public int mValue { get; private set; }

    public void Start()
    {
        tag = Collision.TAG;
        var newSprite = gameObject.AddComponent<Sprite>();
        newSprite.mSpriteSheet = mSpriteSheet;
        newSprite.mIsAnimated = true;
        newSprite.mAnimWait = 45;
        newSprite.mFrameSkip = 2;
        mValue = UnityEngine.Random.Range(0, 3) + 1;
        switch (mValue - 1)
        {
            case 0:
                newSprite.mSpriteName = "G";
                break;
            case 1:
                newSprite.mSpriteName = "B";
                break;
            case 2:
                newSprite.mSpriteName = "R";
                break;
        }
    }
}