using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class MyTest : MonoBehaviour
{
    private UnityArmatureComponent _armatureComp;
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        UnityFactory.factory.LoadDragonBonesData("mecha_2903/mecha_2903_ske");
        UnityFactory.factory.LoadTextureAtlasData("mecha_2903/mecha_2903_tex");
        this._armatureComp = UnityFactory.factory.BuildArmatureComponent("mecha_2903d");
        this._armatureComp.animation.Play("idle");
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (x != 0 || y != 0)
        {
            _armatureComp.transform.Translate(x * moveSpeed, y * moveSpeed, 0, Space.World);
            _armatureComp.transform.rotation = Quaternion.Euler(0, 0, (Vector2.SignedAngle(Vector2.right, new Vector2(x, y))));
            Debug.Log(Vector2.SignedAngle(Vector2.right, new Vector2(x, y)));
            if (x != 0)
            {
                //_armatureComp.transform.localScale = new Vector3(x, 1, 1);
            }
            if (_armatureComp.animation.lastAnimationName != "walk")
            {
                this._armatureComp.animation.FadeIn("walk", 0.2f);
            }
        }
        else
        {
            if (_armatureComp.animation.lastAnimationName != "idle")
            {
                this._armatureComp.animation.FadeIn("idle", 0.2f);
            }
        }
    }
}
