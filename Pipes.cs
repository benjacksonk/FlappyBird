using System;
using UnityEngine;

public class Pipes : MonoBehaviour
{
    public Action onReachScoreThreshold;
    [SerializeField] float speed = 8;

    bool scored = false;
    float terminalPosX;
    Rigidbody2D physBody;

    // Start is called before the first frame update
    void Start()
    {
        var screenMinX = Camera.main.ViewportToWorldPoint(Camera.main.rect.min).x;
        var pipeSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        var pipeWorldScale = pipeSpriteRenderer.transform.TransformVector(pipeSpriteRenderer.transform.localScale).x;
        var pipeWidth = pipeSpriteRenderer.size.x * pipeWorldScale;
        terminalPosX = screenMinX - pipeWidth - 0.5f;

        physBody = GetComponent<Rigidbody2D>();
        physBody.velocity = speed * Vector2.left;

        GM.onEndGame += Halt;
        GM.onReset += Despawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (!scored && transform.position.x <= GM.scoreThresholdPosX)
        {
            scored = true;
            GM.Score();
        }

        if (transform.position.x <= terminalPosX)
        {
            Despawn();
        }
    }

    void OnDestroy()
    {
        GM.onEndGame -= Halt;
        GM.onReset -= Despawn;
    }

    void Halt()
    {
        physBody.velocity = Vector2.zero;
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
