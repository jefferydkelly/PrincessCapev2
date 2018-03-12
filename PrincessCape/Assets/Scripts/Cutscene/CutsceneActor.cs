using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CutsceneActor : MonoBehaviour
{
	private SpriteRenderer mySpriteRenderer;
	bool isHidden = true;
    string characterName = "Character";

	// Use this for initialization
    public void Init() {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        IsHidden = true;
    }

    Timer CreateTimer(float time) {
		int totalTicks = Mathf.FloorToInt(time / 0.03f);
		Timer timer = new Timer(0.03f, totalTicks);
        timer.OnComplete.AddListener(Cutscene.Instance.NextElement);
        return timer;
    }
	
	public void FadeIn(float time)
	{
        Timer fadeTimer = CreateTimer(time);
        fadeTimer.OnTick.AddListener(()=> {
            mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(fadeTimer.RunPercent);
        });

        fadeTimer.OnComplete.AddListener(() => {
            isHidden = false;
        });

        fadeTimer.Start();
	}

	public void FadeOut(float time)
	{

		Timer fadeTimer = CreateTimer(time);
		fadeTimer.OnTick.AddListener(() => {
			mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(1 - fadeTimer.RunPercent);
		});

		fadeTimer.OnComplete.AddListener(() => {
			isHidden = true;
		});

        fadeTimer.Start();
	}

	public void Fade(float fa, float time)
	{
        Timer fadeTimer = CreateTimer(time);
		Color col = mySpriteRenderer.color;
		float startAlpha = col.a;
		float alphaDelta = fa - col.a;
        fadeTimer.OnTick.AddListener(()=> {
            mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(startAlpha + alphaDelta * fadeTimer.RunPercent);
        });

        fadeTimer.OnComplete.AddListener(()=> {
            mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(fa);
            isHidden = (mySpriteRenderer.color.a < float.Epsilon);
        });

        fadeTimer.Start();
	}

	public void MoveTo(Vector2 p, float time)
	{
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(p.x, p.y, transform.position.z);
        Vector3 dif = endPosition - startPosition;

        Timer moveTimer = CreateTimer(time);
        moveTimer.OnTick.AddListener(() => {
            transform.position = startPosition + dif * moveTimer.RunPercent;
        });

        moveTimer.OnComplete.AddListener(()=> {
            transform.position = endPosition;
        });

        moveTimer.Start();
	}

	public void MoveBy(Vector2 p, float time)
	{
		Vector3 startPosition = transform.position;
        Vector3 dif = new Vector3(p.x, p.y, 0);

        Timer moveTimer = CreateTimer(time);
		moveTimer.OnTick.AddListener(() => {
            transform.position = startPosition + dif * moveTimer.RunPercent;
		});

		moveTimer.OnComplete.AddListener(() => {
            transform.position = startPosition + dif;
		});

        moveTimer.Start();
	}

	public void Rotate(float ang, float time)
    {
        Timer rotateTimer = CreateTimer(time);
        float curRotation = 0;
        rotateTimer.OnTick.AddListener(()=> {
            transform.Rotate(Vector3.forward, -curRotation);
            curRotation = ang * rotateTimer.RunPercent;
            transform.Rotate(Vector3.forward, curRotation);
        });

        rotateTimer.OnComplete.AddListener(()=> {
			transform.Rotate(Vector3.forward, -curRotation);
			transform.Rotate(Vector3.forward, ang);
        });
		
        rotateTimer.Start();
	}

	public void Scale(float sc, float time)
	{
        Timer scaleTimer = CreateTimer(time);

        float startScale = transform.localScale.x;
		float scaleDif = sc - transform.localScale.x;
		
        scaleTimer.OnTick.AddListener(()=> {
            float curScale = startScale + scaleDif * scaleTimer.RunPercent;
			transform.localScale = new Vector3(curScale, curScale, 1);
        });
		
        scaleTimer.OnComplete.AddListener(()=> {
            transform.localScale = new Vector3(sc, sc, 1);
        });

        scaleTimer.Start();
	}

	public void ScaleX(float sc, float time)
	{
		Timer scaleTimer = CreateTimer(time);

		float startScale = transform.localScale.x;
		float scaleDif = sc - transform.localScale.x;

		scaleTimer.OnTick.AddListener(() => {
            transform.localScale = transform.localScale.SetX(startScale + scaleDif * scaleTimer.RunPercent);
		});

		scaleTimer.OnComplete.AddListener(() => {
            transform.localScale = transform.localScale.SetX(sc);
		});


        scaleTimer.Start();
	}

	public void ScaleY(float sc, float time)
	{
		Timer scaleTimer = CreateTimer(time);

		float startScale = transform.localScale.y;
		float scaleDif = sc - transform.localScale.y;

		scaleTimer.OnTick.AddListener(() => {
			transform.localScale = transform.localScale.SetY(startScale + scaleDif * scaleTimer.RunPercent);
		});

		scaleTimer.OnComplete.AddListener(() => {
			transform.localScale = transform.localScale.SetY(sc);
		});

        scaleTimer.Start();
	}


	public void ScaleXY(Vector3 sc, float time)
	{
		Vector3 scaleDif = sc - transform.localScale;
		scaleDif.z = 0;

		Timer scaleTimer = CreateTimer(time);

        Vector3 startScale = transform.localScale;

		scaleTimer.OnTick.AddListener(() => {
            transform.localScale = startScale + scaleDif * scaleTimer.RunPercent;
		});

		scaleTimer.OnComplete.AddListener(() => {
            transform.localScale = sc;
		});

        scaleTimer.Start();
	}
	public string CharacterName
	{
		get
		{
			return characterName;
		}

        set {
            characterName = value;
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

            mySpriteRenderer.color = mySpriteRenderer.color.SetAlpha(value ? 0 : 1);
		}
	}

	public void DestroySelf()
	{
		Destroy(gameObject);
	}
}

