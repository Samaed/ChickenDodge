using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
	[SerializeField]
	TextSprite mScoreSprite;

	int _value = 0;
	public int Value
	{
		get {
			return _value;
		}

		set {
			_value = value;
			mScoreSprite.Text = Value.ToString();
		}
	}

	public void Start()
	{
		Value = Value;
	}
}