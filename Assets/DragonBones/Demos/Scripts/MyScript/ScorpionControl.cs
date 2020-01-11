using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class ScorpionControl : MonoBehaviour
{
    public float moveSpeed;     //移动速度
    public Rigidbody2D rb;      //角色刚体组件
    public List<GameObject> colliderBalloon; //障碍物顶点列表
    public BalloonManager balloonManager;
    private bool isCollided;    //是否与障碍物碰撞
    public SpriteRenderer wall; //变色的障碍物

    private UnityArmatureComponent _armatureComp;   //龙骨骨架
    private Vector3 _helpPointA = new Vector3();    //坐标变换用的临时数据存放点
    private Vector3 _helpPointB = new Vector3();    //坐标变换用的临时数据存放点
    private readonly DragonBones.Point _intersectionPointA = new DragonBones.Point();   //线段从起点到终点与碰撞盒相交的第一个交点
    private readonly DragonBones.Point _intersectionPointB = new DragonBones.Point();   //线段从终点到起点与碰撞盒相交的第一个交点
    private readonly DragonBones.Point _normalRadians = new DragonBones.Point();        //交点边界框切线的法线弧度，即交点处碰撞盒边线的法线方向

    public float attackStun;
    public float chargeTime;
    private float attackTime;
    private bool isStun;

    // Start is called before the first frame update
    void Start()
    {
        _armatureComp = GetComponent<UnityArmatureComponent>(); //定义龙骨骨架
        _armatureComp.animation.Play("idle");   //播放待机动画
        _armatureComp.debugDraw = true;         //显示骨架和包围盒线段
        colliderBalloon = balloonManager.allBalloon;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStun)
        {
            Move();
            Attack();
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            if ((Time.time - attackTime) > chargeTime)
            {
                BoundingBoxCheck();
            }
            if ((Time.time - attackTime) > attackStun)
            {
                isStun = false;
            }
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(x * moveSpeed, y * moveSpeed);
        if (x != 0 || y != 0)
        {
            //_armatureComp.transform.Translate(x * moveSpeed, y * moveSpeed, 0, Space.World);
            _armatureComp.transform.rotation = Quaternion.Euler(0, 0, (Vector2.SignedAngle(Vector2.right, new Vector2(x, y))));
            if (x != 0)
            {
                _armatureComp.transform.localScale = new Vector3(1, x, 1);
            }
            if (_armatureComp.animation.lastAnimationName != "walk")    //按键时过渡到walk动画，每次运行FadeIn方法都会从头播放动画，因此需要加一个判断
            {
                this._armatureComp.animation.FadeIn("walk", 0.2f);
            }
        }
        else
        {
            if (_armatureComp.animation.lastAnimationName != "idle")    //不按键时过渡到walk动画，每次运行FadeIn方法都会从头播放动画，因此需要加一个判断
            {
                this._armatureComp.animation.FadeIn("idle", 0.2f);
            }
        }
    }

    void BoundingBoxCheck()
    {
        isCollided = false;
        for (int i = 0; i < colliderBalloon.Count; i++)
        {
            List<Vector3> colliderPoints = new List<Vector3>();
            colliderPoints.Add(new Vector3(colliderBalloon[i].transform.position.x + 0.42f, colliderBalloon[i].transform.position.y + 0.42f));
            colliderPoints.Add(new Vector3(colliderBalloon[i].transform.position.x - 0.42f, colliderBalloon[i].transform.position.y + 0.42f));
            colliderPoints.Add(new Vector3(colliderBalloon[i].transform.position.x - 0.42f, colliderBalloon[i].transform.position.y - 0.42f));
            colliderPoints.Add(new Vector3(colliderBalloon[i].transform.position.x + 0.42f, colliderBalloon[i].transform.position.y - 0.42f));
            for (int j = 0; j < colliderPoints.Count; j++)  //障碍物顶点两两组成一条边，分别进行判断
            {
                if (j == colliderPoints.Count - 1)
                {
                    _helpPointA = _armatureComp.transform.InverseTransformPoint(colliderPoints[j]);  //将障碍物坐标变换到骨架局部坐标
                    _helpPointB = _armatureComp.transform.InverseTransformPoint(colliderPoints[0]);
                }
                else
                {
                    _helpPointA = _armatureComp.transform.InverseTransformPoint(colliderPoints[j]);
                    _helpPointB = _armatureComp.transform.InverseTransformPoint(colliderPoints[j + 1]);
                }
                //判断线段是否与骨架的所有插槽的自定义包围盒相交
                Slot intersectsSlots = this._armatureComp.armature.IntersectsSegment(this._helpPointA.x, this._helpPointA.y,
                                                                                this._helpPointB.x, this._helpPointB.y,
                                                                                this._intersectionPointA, _intersectionPointB, _normalRadians);
                //判断点是否在所有插槽的自定义包围盒内
                //Slot containsTargetA = this._armatureComp.armature.ContainsPoint(this._helpPointA.x, this._helpPointA.y);
                //Slot containsTargetB = this._armatureComp.armature.ContainsPoint(this._helpPointB.x, this._helpPointB.y);

                if (intersectsSlots != null)
                {
                    isCollided = true;
                    colliderBalloon[i].GetComponent<BalloonAttack>().Hit();
                    balloonManager.RemoveBalloon(i);
                    balloonManager.score += 10;
                    break;
                }
            }
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this._armatureComp.animation.FadeIn("skill_04", 0.2f);
            isStun = true;
            attackTime = Time.time;
        }
    }
}
