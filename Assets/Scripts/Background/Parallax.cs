using UnityEngine;

public class Parallax : MonoBehaviour
{
    float distance;
    [Range(0f, 0.5f)]
    public float speed = 0.2f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        distance += Time.deltaTime * speed;
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
