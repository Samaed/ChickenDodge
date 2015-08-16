using UnityEngine;
using System.Collections;

public class ChickenSpawner : MonoBehaviour
{
	[SerializeField]
	SpriteSheet mSpriteSheet;

	[SerializeField]
	Rect mSourceArea;

	[SerializeField]
	Rect mTargetArea;

	[SerializeField]
	float mSpawnWaitFactor = 1.0f;

	float mSpawnDelay = 30.0f;

	public void Start ()
	{
		Spawn();
	}
	
	public void Update ()
	{
		var spawnDelay = (int) mSpawnDelay;
		if ( ( Time.frameCount % spawnDelay ) == 0 )
		{
			Spawn();
			mSpawnDelay = Mathf.Max( 8.0f, mSpawnDelay * mSpawnWaitFactor );
		}
	}

	void Spawn()
	{
		var newChickenObj = new GameObject();
		var newChicken = newChickenObj.AddComponent<Chicken>();
		newChicken.mSpriteSheet = mSpriteSheet;
		newChickenObj.transform.parent = gameObject.transform;

		Vector2 pos = Vector2.zero;

		switch ( Random.Range( 0, 4 ) )
		{
			case 0:
				pos = new Vector2( Random.Range( mSourceArea.xMin, mSourceArea.xMax ), mSourceArea.yMin );
				break;
			case 1:
				pos = new Vector2( Random.Range( mSourceArea.xMin, mSourceArea.xMax ), mSourceArea.yMax );
				break;
			case 2:
				pos = new Vector2( mSourceArea.xMin, Random.Range( mSourceArea.yMin, mSourceArea.yMax ) );
				break;
			case 3:
				pos = new Vector2( mSourceArea.xMax, Random.Range( mSourceArea.yMin, mSourceArea.yMax ) );
				break;
		}

		newChicken.transform.localPosition = pos;
		newChicken.mTarget = new Vector2( Random.Range( mTargetArea.xMin, mTargetArea.xMax ), Random.Range( mTargetArea.yMin, mTargetArea.yMax ) );
	}
}
