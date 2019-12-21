using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed;
    public List<GameObject> colliderPoints;
    private bool isCollided;
    public SpriteRenderer wall;

    private UnityArmatureComponent _armatureComp;
    private Vector3 _helpPointA = new Vector3();
    private Vector3 _helpPointB = new Vector3();
    private readonly DragonBones.Point _intersectionPointA = new DragonBones.Point();
    private readonly DragonBones.Point _intersectionPointB = new DragonBones.Point();
    private readonly DragonBones.Point _normalRadians = new DragonBones.Point();

    // Start is called before the first frame update
    void Start()
    {
        _armatureComp = GetComponent<UnityArmatureComponent>();
        _armatureComp.animation.Play("idle");
        _armatureComp.debugDraw = true;
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
                _armatureComp.transform.localScale = new Vector3(1, x, 1);
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
        BoundingBoxCheck();
    }

    void BoundingBoxCheck()
    {
        isCollided = false;
        for (int i = 0; i < colliderPoints.Count; i++)
        {
            if (i == colliderPoints.Count - 1)
            {
                _helpPointA = _armatureComp.transform.InverseTransformPoint(colliderPoints[i].transform.position);
                _helpPointB = _armatureComp.transform.InverseTransformPoint(colliderPoints[0].transform.position);
            }
            else
            {
                _helpPointA = _armatureComp.transform.InverseTransformPoint(colliderPoints[i].transform.position);
                _helpPointB = _armatureComp.transform.InverseTransformPoint(colliderPoints[i + 1].transform.position);
            }
            Slot intersectsSlots = this._armatureComp.armature.IntersectsSegment(this._helpPointA.x, this._helpPointA.y,
                                                                            this._helpPointB.x, this._helpPointB.y,
                                                                            this._intersectionPointA, _intersectionPointB, _normalRadians);
            if (intersectsSlots != null)
            {
                isCollided=true;
                Debug.Log(isCollided);
            }
        }
        if (isCollided)
        {
            wall.color = Color.red;
        }
        else
        {
            wall.color = Color.green;
        }
    }
}
