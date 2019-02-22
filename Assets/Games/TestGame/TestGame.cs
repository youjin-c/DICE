using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class TestGame : GameInput
{
    [Header("Game")]
    public GameObject[] cubes;
    public float moveSpeed;
    public GameObject explosion;
    public float explosionLifetime;
    public float speed = 10f;

    UnityEvent joystickEvent = new UnityEvent();

    Vector3 p1Start;
    Vector3 p2Start;
    public AudioSource baseline;
    public AudioSource drum2;
    public AudioSource lightdrum;
    int left, center, right;
    int selNum = 0;
    GameObject selected;
    Vector3 large = new Vector3(7.0F, 7.0F, 7.0F);
    Vector3 normal = new Vector3(5.0F, 5.0F, 5.0F);

    Vector2 p1Move, p2Move;

    Color red   = new Color32(255, 76, 158, 255);
    Color green = new Color32(36, 214, 0, 255);
    Color blue  = new Color32(0, 97, 255, 255);

    Vector3 possibility = new Vector3(0.0f,0.0f,0.0f);
    Vector3 onOff = new Vector3(0.0f, 0.0f, 0.0f);

    public ParticleSystem ps;
    public float hSliderValueSpeed = 1.0f;

    Gradient gradient, gradient2;
    GradientColorKey[] colorKey;
    GradientAlphaKey[] alphaKey;

    void Start()
    {
        joystickEvent.AddListener(Joy2);
        //p1Start = player1.transform.position;
        //p2Start = player2.transform.position;
        InvokeRepeating("Music", 2.0f, 8.0f);

        for(int i = 0; i < cubes.Length; i++) {
            cubes[i].GetComponent<Renderer>().material.color = red;
        }

        ///PARTICLE//////////////////////////////////
        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.World;

        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.0f);
        curve.AddKey(1.0f, 1.0f);

        ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve(1.0f, curve);

        velocityOverLifetime.speedModifier = minMaxCurve;
        /////////////////////////////////////////////

        gradient = new Gradient();

        colorKey = new GradientColorKey[2];

        colorKey[0].color = new Color32(255,255,102,255);
        colorKey[0].time = 0.0f;
        colorKey[1].color = new Color32(195, 195, 195, 255);
        colorKey[1].time = 1.0f;


        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;


        gradient.SetKeys(colorKey, alphaKey);

        gradient2 = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];

        colorKey[0].color = new Color32(195, 195, 195, 255);
        colorKey[0].time = 0.0f;
        colorKey[1].color = new Color32(159, 224, 214, 255);
        colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;


        gradient2.SetKeys(colorKey, alphaKey);


    }

    void Update()
    {


        var velocityOverLifetime = ps.velocityOverLifetime;
        //JOYSTICK INPUTS
        p1Move = Vector2.ClampMagnitude(joy1, 1);
        p2Move = Vector2.ClampMagnitude(joy2, 1);

        selected = cubes[selNum];

        if (Input.GetKeyDown(KeyCode.J) && joystickEvent != null)
        {
            selNum -= 1;
            joystickEvent.Invoke();

        }
        if (Input.GetKeyDown(KeyCode.L) && joystickEvent != null)
        {
            selNum += 1;
            joystickEvent.Invoke();
        }
        for (int i = 0; i < cubes.Length; i++)
        {
            if (onOff[i] >= 1.0f) {
                cubes[i].transform.Rotate(Vector3.up, -10*speed * Time.deltaTime);
                cubes[i].transform.Rotate(Vector3.right, -10*speed * Time.deltaTime);
                cubes[i].transform.Rotate(Vector3.forward, -10*speed * Time.deltaTime);
            }
            else {
                cubes[i].transform.Rotate(Vector3.up, speed * Time.deltaTime);
                cubes[i].transform.Rotate(Vector3.right, speed * Time.deltaTime);
                cubes[i].transform.Rotate(Vector3.forward, speed * Time.deltaTime);
            }
        }
        if (hSliderValueSpeed >= 0.6f)
        {
            hSliderValueSpeed += p1Move.x * 0.03f;
        }
            
        if (hSliderValueSpeed < 0.6f)
        {
            hSliderValueSpeed =0.6f;
        }
        velocityOverLifetime.speedModifierMultiplier = hSliderValueSpeed;

    }

    void Joy2()
    {
        if (selNum < 0)
        {
            selNum += 3;
        }
        selNum %= 3;
        switch (selNum)
        {
            case 0:
                cubes[0].transform.localScale = large;
                cubes[1].transform.localScale = normal;
                cubes[2].transform.localScale = normal;
                break;
            case 1:
                cubes[0].transform.localScale = normal;
                cubes[1].transform.localScale = large;
                cubes[2].transform.localScale = normal;
                break;
            case 2:
                cubes[0].transform.localScale = normal;
                cubes[1].transform.localScale = normal;
                cubes[2].transform.localScale = large;
                break;
            default:
                Debug.Log("CUBE " + selNum + " selected");
                break;

        }
    }

    void ExplosionAt(Vector3 position)
    {
        GameObject newExplosion = Instantiate(explosion, position, Quaternion.identity);
        Destroy(newExplosion, explosionLifetime);
    }

    public override void Button1Down()
    {
        var main = ps.main;
        main.startColor = gradient;
    }

    public override void Button2Down()
    {
        var main = ps.main;
        main.startColor = gradient2;
    }

    public override void Button3Down()
    {
        if(selected.GetComponent<Renderer>().material.color == red) {
            //Debug.Log("BTN3:R-B");
            selected.GetComponent<Renderer>().material.color = blue;
            possibility[selNum] = 1;
        }
        else if (selected.GetComponent<Renderer>().material.color == blue)
        {
            //Debug.Log("BTN3:B-G");
            selected.GetComponent<Renderer>().material.color = green;
            possibility[selNum] = 0.5f;
        }
        else if (selected.GetComponent<Renderer>().material.color == green){
            //Debug.Log("BTN3:G-r");
            selected.GetComponent<Renderer>().material.color = red;
            possibility[selNum] = 0f;
        }
        //ExplosionAt(player2.transform.position);
    }

    public override void Button4Down()
    {
        //Debug.Log("BTN4");
        if (selected.GetComponent<Renderer>().material.color == red)
        {
            selected.GetComponent<Renderer>().material.color = green;
            possibility[selNum] = 0.5f;
        }
        else if(selected.GetComponent<Renderer>().material.color == green)
        {
            selected.GetComponent<Renderer>().material.color = blue;
            possibility[selNum] = 1;
        }
        else if(selected.GetComponent<Renderer>().material.color == blue)
        {
            selected.GetComponent<Renderer>().material.color = red;
            possibility[selNum] = 0f;
        }
    }

    //public override void Reset()
    //{
    //    player1.transform.position = p1Start;
    //    player2.transform.position = p2Start;
    //}

    void Music() {
        if(Random.value < 0.5) {
            drum2.Stop();
            drum2.Play(0);
            //Debug.Log("DRUM2");
        }
        if(Random.value < 0.5)
        {
            lightdrum.Stop();
            lightdrum.Play(0);
            //Debug.Log("LIGHT");
        }
        for(int i = 0; i < cubes.Length; i++) {
            if (possibility[i] > Random.value)
            {
                cubes[i].GetComponent<AudioSource>().Stop();
                cubes[i].GetComponent<AudioSource>().Play(0);
                onOff[i] = 1.0f;
            }else { onOff[i] = 0.0f; }

        }
        lightdrum.Stop();
        baseline.Play(0);

    }
}
