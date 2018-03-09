using UnityEngine;
using System.Collections;

public class CutsceneActor : MonoBehaviour
{
	private SpriteRenderer mySpriteRenderer;
	private CutsceneCharacter myInfo;
	bool isHidden = true;
	public Cutscene parentCutscene;

	// Use this for initialization
	void Awake()
	{
		mySpriteRenderer = GetComponent<SpriteRenderer>();
	}

	public CutsceneCharacter MyInfo
	{
		get
		{
			return myInfo;
		}

		set
		{
			myInfo = value;
			mySpriteRenderer.sprite = myInfo.sprite;
		}
	}

	public void StartFadeIn(float time)
	{
		StartCoroutine(FadeIn(time));
	}

	public IEnumerator FadeIn(float time)
	{
		float curTime = 0;
		Color col = mySpriteRenderer.color;
		col.a = 0;
		mySpriteRenderer.color = col;
		do
		{
			curTime += Time.deltaTime;
			col.a = curTime / time;
			mySpriteRenderer.color = col;
			yield return null;
		} while (curTime < time);
		isHidden = false;
		parentCutscene.NextElement();
		yield return null;
	}

	public void StartFadeOut(float time)
	{
		StartCoroutine(FadeOut(time));
	}

	public IEnumerator FadeOut(float time)
	{
		float curTime = 0;
		Color col = mySpriteRenderer.color;
		col.a = 1;
		mySpriteRenderer.color = col;
		do
		{
			curTime += Time.deltaTime;
			col.a = 1 - (curTime / time);
			mySpriteRenderer.color = col;
			yield return null;
		} while (curTime < time);
		isHidden = true;
		parentCutscene.NextElement();
		yield return null;
	}

	public void StartFade(float finalAlpha, float fadeTime)
	{

	}

	public IEnumerator Fade(float fa, float time)
	{
		Color col = mySpriteRenderer.color;
		float startAlpha = col.a;
		float alphaDelta = fa - col.a;
		float curTime = 0.0f;

		do
		{
			curTime += Time.deltaTime;
			col.a = startAlpha + alphaDelta * curTime / time;
			mySpriteRenderer.color = col;
			yield return null;
		} while (curTime < time);

		col.a = fa;
		mySpriteRenderer.color = col;
        isHidden = (col.a < float.Epsilon);
		parentCutscene.NextElement();
		yield return null;

	}

	public void MoveTo(Vector2 p, float time)
	{
		Vector2 dif = p - (Vector2)(transform.position);
		StartCoroutine(MoveBy(dif, time));
	}

	public IEnumerator MoveBy(Vector2 p, float time)
	{
		Vector2 startPosition = transform.position;
		if (time > 0)
		{
			float curTime = 0;

			do
			{
				curTime += Time.deltaTime;
				transform.position = startPosition + (p * curTime / time);
				yield return null;
			} while (curTime < time);

		}
		transform.position = startPosition + p;
		parentCutscene.NextElement();
		yield return null;
	}

	public void StartRotation(float ang, float time)
	{
		StartCoroutine(Rotate(ang, time));
	}
	public IEnumerator Rotate(float ang, float time)
	{
		float curRotation = 0;
		if (time > 0)
		{

			float curTime = 0;
			do
			{
				transform.Rotate(Vector3.forward, -curRotation);
				curTime += Time.deltaTime;
				curRotation = ang * curTime / time;
				transform.Rotate(Vector3.forward, curRotation);
				yield return null;
			} while (curTime < time);
		}
		transform.Rotate(Vector3.forward, -curRotation);
		transform.Rotate(Vector3.forward, ang);
		parentCutscene.NextElement();
		yield return null;
	}

	public void StartScale(float sc, float time)
	{
		StartCoroutine(Scale(sc, time));
	}
	public IEnumerator Scale(float sc, float time)
	{
		float scaleDif = sc - transform.localScale.x;
		float curScale = transform.localScale.x;


		if (time > 0)
		{
			float curTime = 0;
			do
			{
				curTime += Time.deltaTime;
				curScale += scaleDif * Time.deltaTime / time;
				transform.localScale = new Vector3(curScale, curScale, 1);
				yield return null;
			} while (curTime < time);
		}
		transform.localScale = new Vector3(sc, sc, 1);
		parentCutscene.NextElement();
		yield return null;
	}

	public void StartScaleX(float sc, float time)
	{
		StartCoroutine(ScaleX(sc, time));
	}

	public IEnumerator ScaleX(float sc, float time)
	{
		float scaleDif = sc - transform.localScale.x;
		float curScale = transform.localScale.x;


		if (time > 0)
		{
			float curTime = 0;
			do
			{
				curTime += Time.deltaTime;
				curScale += scaleDif * Time.deltaTime / time;
				transform.localScale = new Vector3(curScale, transform.localScale.y, 1);
				yield return null;
			} while (curTime < time);
		}
		transform.localScale = new Vector3(sc, transform.localScale.y, 1);
		parentCutscene.NextElement();
		yield return null;
	}

	public void StartScaleY(float sc, float time)
	{
		StartCoroutine(ScaleY(sc, time));
	}

	public IEnumerator ScaleY(float sc, float time)
	{
		float scaleDif = sc - transform.localScale.x;
		float curScale = transform.localScale.x;


		if (time > 0)
		{
			float curTime = 0;
			do
			{
				curTime += Time.deltaTime;
				curScale += scaleDif * Time.deltaTime / time;
				transform.localScale = new Vector3(transform.localScale.x, curScale, 1);
				yield return null;
			} while (curTime < time);
		}
		transform.localScale = new Vector3(transform.localScale.x, sc, 1);
		parentCutscene.NextElement();
		yield return null;
	}

	public void StartScaleXY(Vector3 sc, float time)
	{
		StartCoroutine(ScaleXY(sc, time));
	}

	public IEnumerator ScaleXY(Vector3 sc, float time)
	{
		Vector3 scaleDif = sc - transform.localScale;
		scaleDif.z = 0;

		if (time > 0)
		{
			float curTime = 0;
			do
			{
				curTime += Time.deltaTime;
				transform.localScale += scaleDif * Time.deltaTime / time;
				yield return null;
			} while (curTime < time);
		}

		transform.localScale = sc;
		parentCutscene.NextElement();
		yield return null;


	}
	public string CharacterName
	{
		get
		{
			return myInfo.characterName;
		}
	}

	public Sprite MySprite
	{
		get
		{
			return mySpriteRenderer.sprite;
		}

		set
		{
			mySpriteRenderer.sprite = value;
		}
	}

	public Vector3 Position
	{
		get
		{
			return transform.position;
		}

		set
		{
			transform.position = value;
		}
	}

	public void FlipX()
	{
		mySpriteRenderer.flipX = !mySpriteRenderer.flipX;
	}

	public void FlipY()
	{
		mySpriteRenderer.flipY = !mySpriteRenderer.flipY;
	}
	public bool IsHidden
	{
		get
		{
			return isHidden;
		}

		set
		{
			isHidden = value;

			Color myColor = mySpriteRenderer.color;
			myColor.a = value ? 0 : 1;
			mySpriteRenderer.color = myColor;
		}
	}

	public void DestroySelf()
	{
		Destroy(gameObject);
	}
}

