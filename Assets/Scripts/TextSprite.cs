using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TextSprite : MonoBehaviour
{
	public enum AlignType { Left, Right };

	[SerializeField]
	SpriteSheet mSpriteSheet;

	[SerializeField]
	AlignType mAlign;

	IList<Sprite> mSprites = new List<Sprite>();

	string _text;
	public string Text
	{
		get {
			return _text;
		}

		set {
			_text = value;
			UpdateTextSprites();
		}
	}

	public void Start()
	{
		UpdateTextSprites();
	}

	void UpdateTextSprites()
	{
		foreach ( var s in mSprites )
			GameObject.Destroy( s.gameObject );
		mSprites.Clear();

		if ( string.IsNullOrEmpty( Text ) )
			return;

		float offset = 0.0f;
		float dir = mAlign == AlignType.Left ? 1.0f : -1.0f;

		var text = Text.ToCharArray();
		if ( mAlign == AlignType.Right )
			text = text.Reverse().ToArray();

		foreach ( var c in text )
		{
			var charKey = c.ToString();

			if ( mSpriteSheet.Sprites.ContainsKey( charKey ) == false )
				continue;

			var newSpriteObj = new GameObject();
			var newSprite = newSpriteObj.AddComponent<Sprite>();
			newSpriteObj.transform.parent = gameObject.transform;
			newSpriteObj.transform.localPosition = Vector3.right * offset;
			newSprite.mSpriteSheet = mSpriteSheet;
			newSprite.mIsAnimated = false;
			newSprite.mSpriteName = charKey;

			var spriteDescr = mSpriteSheet.Sprites[charKey];
			offset += spriteDescr.mSpriteSize.x * dir;

			mSprites.Add( newSprite );
		}
	}
}