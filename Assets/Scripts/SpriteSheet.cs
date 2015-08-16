using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteSheet : MonoBehaviour {

	[SerializeField]
	TextAsset mDescription;

	[SerializeField]
	Texture2D mTexture;

	[SerializeField]
	Shader mShader;

	public IDictionary<string, SpriteDescription> Sprites { get; private set; }
	public Shader Shader { get { return mShader; } }
	public Texture2D Texture { get { return mTexture; } }

	public void Awake()
	{
		Sprites = ParseDescription( mDescription.text, mTexture );
	}

	static IDictionary<string, SpriteDescription> ParseDescription( string rawDescription, Texture2D texture )
	{
		float width = texture.width;
		float height = texture.height;

		var description = (IDictionary<string, object>) MiniJSON.Json.Deserialize( rawDescription );
		var framesDescr = (IDictionary<string, object>) description["frames"];

		var sprites = new Dictionary<string, SpriteDescription>();

		foreach ( var item in framesDescr )
		{
			var itemDescr = (IDictionary<string, object>) item.Value;
			var frameDescr = (IDictionary<string, object>) itemDescr["frame"];
			var sourceDescr = (IDictionary<string, object>) itemDescr["spriteSourceSize"];
			var sizeDescr = (IDictionary<string, object>) itemDescr["sourceSize"];
			var frame = ExtractRect( frameDescr );
			var source = ExtractRect( sourceDescr );
			frame.xMin /= width;
			frame.yMin /= height;
			frame.xMax /= width;
			frame.yMax /= height;

			sprites[item.Key] = new SpriteDescription {
				mUV = AdjustUVOrientation( frame ),
				mSpriteSize = ExtractSize( sizeDescr ),
				mPixelOffset = new Vector2( source.xMin, source.yMin ),
				mPixelSize = new Vector2( source.width, source.height )
			};
		}

		return sprites;
	}

	static Vector2 ExtractSize( IDictionary<string, object> descr )
	{
		return new Vector2 {
			x = (float)(long) descr["w"],
			y = (float)(long) descr["h"]
		};
	}

	static Rect ExtractRect( IDictionary<string, object> descr )
	{
		return new Rect {
			xMin = (float)(long) descr["x"],
			yMin = (float)(long) descr["y"],
			width = (float)(long) descr["w"],
			height = (float)(long) descr["h"]
		};
	}

	static Rect AdjustUVOrientation( Rect frame )
	{
		return new Rect {
			xMin = frame.xMin,
			yMin = 1.0f - frame.yMax,
			width = frame.width,
			height = frame.height
		};
	}
}
