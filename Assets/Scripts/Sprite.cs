using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class Sprite : MonoBehaviour
{
	struct Vertex
	{
		public Vector3 position;
		public Vector2 uv;
	}

	public Action AnimationEndedEvent = delegate {};

	[SerializeField]
	internal SpriteSheet mSpriteSheet;

	[SerializeField]
	internal string mSpriteName;

	[SerializeField]
	internal bool mIsAnimated = false;

	[SerializeField]
	internal int mFrameSkip = 2;

	[SerializeField]
	internal int mAnimWait = 0;

	internal int mAnimationFrame = 1;
	int mAnimWaitCounter;

	MeshFilter mMeshFilter;
	MeshRenderer mMeshRender;
	UnityEngine.Mesh mMesh;

	Vertex[] mVertex;
	int[] mIndices;

	public Vector2 SpriteSize { get; private set; }

	public void Awake()
	{
		mMesh = new UnityEngine.Mesh();
		mMesh.MarkDynamic();

		mMeshFilter = gameObject.AddComponent<MeshFilter>();
		mMeshFilter.mesh = mMesh;

		mMeshRender = gameObject.AddComponent<MeshRenderer>();
		mMeshRender.castShadows = false;
		mMeshRender.useLightProbes = false;
		mMeshRender.receiveShadows = false;
	}

	public void Start ()
	{
		mMeshRender.material = new Material( mSpriteSheet.Shader );
		mMeshRender.material.mainTexture = mSpriteSheet.Texture;
		mAnimWaitCounter = mAnimWait;
		UpdateMesh();
	}
	
	public void Update ()
	{
		if ( !mIsAnimated )
			return;

		if ( mAnimWaitCounter > 0 )
		{
			mAnimWaitCounter--;
			return;
		}

		if ( Time.frameCount % mFrameSkip == 0 )
			UpdateMesh();
	}

	public void UpdateMesh()
	{
		mMesh.Clear();
		var spriteName = mIsAnimated ? FindNextFrameName() : mSpriteName;

		if ( mSpriteSheet.Sprites.ContainsKey( spriteName ) )
		{
			var descr = mSpriteSheet.Sprites[spriteName];
			UpdateComponents( descr );
			mMesh.vertices = mVertex.Select( v => v.position ).ToArray();
			mMesh.uv = mVertex.Select( v => v.uv ).ToArray();
			mMesh.triangles = mIndices;
			SpriteSize = descr.mSpriteSize;
		}
	}

	string FindNextFrameName()
	{
		var animationSprite = string.Format( "{0}{1}", mSpriteName, mAnimationFrame );
		if ( mSpriteSheet.Sprites.ContainsKey( animationSprite ) )
		{
			mAnimationFrame++;
			return animationSprite;
		}
		else if ( mAnimationFrame == 1 )
		{
			return mSpriteName;
		}
		else
		{
			mAnimationFrame = 1;
			mAnimWaitCounter = mAnimWait;
			AnimationEndedEvent();
			return FindNextFrameName();
		}
	}

	void UpdateComponents( SpriteDescription descr )
	{
		mVertex = new [] {
			new Vertex {
				position = descr.mPixelOffset,
				uv = new Vector2( descr.mUV.xMin, descr.mUV.yMin )
			},
			new Vertex {
				position = descr.mPixelOffset + descr.mPixelSize.x * Vector2.right,
				uv = new Vector2( descr.mUV.xMax, descr.mUV.yMin )
			},
			new Vertex {
				position = descr.mPixelOffset + descr.mPixelSize,
				uv = new Vector2( descr.mUV.xMax, descr.mUV.yMax )
			},
			new Vertex {
				position = descr.mPixelOffset + descr.mPixelSize.y * Vector2.up,
				uv = new Vector2( descr.mUV.xMin, descr.mUV.yMax )
			},
		};
		mIndices = new[]{ 2, 1, 0, 3, 2, 0 };
	}
}
