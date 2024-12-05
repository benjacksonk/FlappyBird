using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public Action onBirdDied;

    [SerializeField] float flapSpeed = 14;

    Rigidbody2D physBody;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        physBody = GetComponent<Rigidbody2D>();
        physBody.simulated = false;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        GM.onBeginGame += EnablePhysics;
        GM.onReset += Despawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (GM.gameState == GameState.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                physBody.velocity = flapSpeed * Vector2.up;
            }
        }
    }

    // FixedUpdate is called once per physics tick
    void FixedUpdate()
    {
        spriteRenderer.transform.rotation = 
            Quaternion.AngleAxis(physBody.velocity.y * (GM.gameState == GameState.Playing ? 1 : -5), Vector3.forward);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var direction = collision.contacts.Select(c => c.normal).Aggregate((v1, v2) => v1 + v2) / collision.contactCount;
        var impulse = 0.75f * collision.contacts.Select(c => c.normalImpulse).Average() * direction;
        var fakeImpulse = collision.relativeVelocity.x * Vector2.left;
        physBody.AddForce(impulse + fakeImpulse, ForceMode2D.Impulse);
        Die();
    }

    void OnDestroy()
    {
        GM.onReset -= Despawn;
        GM.onBeginGame -= EnablePhysics;
    }

    void EnablePhysics()
    {
        physBody.simulated = true;
    }

    void Die()
    {
        onBirdDied?.Invoke();
    }

    void Despawn()
    {
        Destroy(gameObject);
    }
}
