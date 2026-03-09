using Unity.Netcode;
using UnityEngine;

public class PaddleController : NetworkBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float clampY = 4.2f;

    void Update()
    {
        if (!IsOwner) return; // only the owning client moves this paddle

        float v = Input.GetAxisRaw("Vertical"); // input axis mapping
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y + v * speed * Time.deltaTime, -clampY, clampY); // clamp for arena
        transform.position = pos; // NetworkTransform replicates
    }
}