using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	private static int MAX_ID = 0;

	enum FacingType { Up, Down, Left, Right };

	static readonly IDictionary<FacingType, string> FacingSpriteCode = new Dictionary<FacingType, string> {
		{FacingType.Up, "B"},
		{FacingType.Down, "F"},
		{FacingType.Left, "L"},
		{FacingType.Right, "R"},
	};

    Collision mCollision;
    Vector3 lastPosition;

	[SerializeField]
	SpriteSheet mSpriteSheet;

    // Let's add some serialized parameters to the player
    // Now we can determine the keys independently.
    [SerializeField]
    KeyCode PlayerUP;
    [SerializeField]
    KeyCode PlayerDOWN;
    [SerializeField]
    KeyCode PlayerLEFT;
    [SerializeField]
    KeyCode PlayerRIGHT;
    [SerializeField]
    KeyCode PlayerATTACK;

	[SerializeField]
	string mPrefix;

	[SerializeField]
	Score mScore;

	public int ID {
		get;
		private set;
	}

    Sprite mSprite;
    FacingType mFacing = FacingType.Down;
    public bool mIsAttacking { get; private set; }
    bool mIsMoving;

	Vector2 _moveDelta;
	bool _fromServer;
    Vector3 _oldPosition;
    bool _attackNetwork;

	public void Awake() {
		ID = MAX_ID++;
	}

	public void Start ()
	{
        _attackNetwork = false;
        _fromServer = false;
        _oldPosition = Vector3.zero;
		_moveDelta = Vector2.zero;

        tag = Collision.TAG;
		mSprite = gameObject.AddComponent<Sprite>();
		mSprite.mSpriteSheet = mSpriteSheet;
		mSprite.AnimationEndedEvent += delegate {
			mIsAttacking = false;
			mSprite.mFrameSkip = 2;
			UpdateSprite();
			mSprite.UpdateMesh();
		};

        // We can handle collisions as we have a collision component 
        mCollision = gameObject.AddComponent<Collision>();
        // We register a listener on our collision component
        mCollision.OnCollided += HandleOnCollided;

		UpdateSprite();
	}

    // We chose to add the value of the Rupee and delete it (Rupee is a passive collider)
    // We also chose to let the Chicken suicide once he is hit (Chicken in an active collider, it listens to its own collisions)
    void HandleOnCollided(GameObject collider)
    {
        if (collider.GetComponent<Rupee>() != null)
        {
            mScore.Value += collider.GetComponent<Rupee>().mValue;
            Destroy(collider);
        }
        else if (collider.GetComponent<Chicken>() == null)
        {
            // We cancel our move once we have a collision
            // The event is fired at the same frame as the collision occurs as Collision does LateUpdate, so the cancel is not too late
            transform.position = lastPosition;
        }
    }

	public void Move(Vector2 delta, Vector3 oldPosition, bool fromServer) {
		_moveDelta = delta;
		_fromServer = fromServer;
        _oldPosition = oldPosition;
	}

    public void Attack()
    {
        _attackNetwork = true;
    }
	
	public void Update ()
	{
        if (mIsAttacking == false && (Input.GetKeyDown(PlayerATTACK) || _attackNetwork))
		{
            if (_attackNetwork)
            {
                _attackNetwork = false;
            }
            else
            {
                GameplayTranslator.Send(GameplayMessage.MessageValue.ATTACK, ID, Vector2.zero, transform.position);
            }

			mIsAttacking = true;
			mSprite.mAnimationFrame = 1;
			mSprite.mFrameSkip = 1;
		}

		Vector2 delta = Vector2.zero;
        if (Input.GetKey(PlayerUP))
		{
			delta += Vector2.up;
		}
        if (Input.GetKey(PlayerDOWN))
		{
			delta -= Vector2.up;
		}
        if (Input.GetKey(PlayerLEFT))
		{
			delta -= Vector2.right;
		}
        if (Input.GetKey(PlayerRIGHT))
		{
			delta += Vector2.right;
		}

		if (_fromServer || (delta.Equals(Vector2.zero) && !_moveDelta.Equals(Vector2.zero))) {
            if (!_oldPosition.Equals(transform.position))
            {
                transform.position.Set(_oldPosition.x, _oldPosition.y, _oldPosition.z);
                transform.Translate(_moveDelta);
                delta = Vector2.zero;
            }
            else
            {
                delta = _moveDelta;
            }

            UpdateFacing(_moveDelta);

            _moveDelta = Vector2.zero;
            _fromServer = false;
		} else {
			delta *= 3.0f;
            if (!delta.Equals(Vector2.zero))
            {
                GameplayTranslator.Send(GameplayMessage.MessageValue.MOVE, ID, delta, transform.position);
            }

            UpdateFacing(delta);
		}

        lastPosition = transform.position;
		transform.Translate( delta );

		UpdateSprite();
	}

    void UpdateFacing(Vector2 delta)
    {
        if (delta.y > 0)
        {
            mFacing = FacingType.Up;
        }
        else if (delta.y < 0)
        {
            mFacing = FacingType.Down;
        }
        else if (delta.x > 0)
        {
            mFacing = FacingType.Right;
        }
        else if (delta.x < 0)
        {
            mFacing = FacingType.Left;
        }

        if (!delta.Equals(Vector2.zero))
            mIsMoving = true;
        else
            mIsMoving = false;
    }

	void UpdateSprite()
	{
		mSprite.mIsAnimated = mIsMoving || mIsAttacking;
		mSprite.mSpriteName = string.Format( "{0}{1}{2}{3}",
			mPrefix,
			mIsAttacking ? "A" : "M",
			FacingSpriteCode[mFacing],
			mSprite.mIsAnimated ? "" : "1"
		);
	}
}