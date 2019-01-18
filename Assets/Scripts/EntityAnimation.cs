using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    public bool looping = false;
    public List<Sprite> sprites = new List<Sprite>();
    public List<float> durations = new List<float>();

    private int curSpr = 0;
    private float curTime = 0;
    private Entity entity = null;

    public float speed { get; set; } = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (entity != null)
        {
            if (curSpr >= sprites.Count || curSpr >= durations.Count)
            {
                if (looping)
                {
                    Begin(entity, speed);
                }
                else
                {
                    Finish();
                    return;
                }
            }

            curTime += Time.deltaTime * speed;

            if (curTime >= durations[curSpr])
            {
                ++curSpr;
                curTime = 0;

                entity.GetComponent<SpriteRenderer>().sprite = sprites[curSpr];
            }
        }
    }

    public void Begin(Entity e, float _speed = 1.0f)
    {
        curSpr = 0;
        curTime = 0;
        entity = e;
        speed = _speed;

        if (sprites.Count > 0)
        {
            entity.GetComponent<SpriteRenderer>().sprite = sprites[curSpr];
        }

        if (sprites.Count == 1)
        {
            Finish();
        }
    }

    public void Finish()
    {
        entity = null;
    }
}
