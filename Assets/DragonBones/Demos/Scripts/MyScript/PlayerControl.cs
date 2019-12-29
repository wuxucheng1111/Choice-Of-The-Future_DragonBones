using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed;     //移动速度
    public Rigidbody2D rb;      //角色刚体组件
    public List<GameObject> colliderPoints; //障碍物顶点列表
    private bool isCollided;    //是否与障碍物碰撞
    public SpriteRenderer wall; //变色的障碍物

    private UnityArmatureComponent _armatureComp;   //龙骨骨架
    private Vector3 _helpPointA = new Vector3();    //坐标变换用的临时数据存放点
    private Vector3 _helpPointB = new Vector3();    //坐标变换用的临时数据存放点
    private readonly DragonBones.Point _intersectionPointA = new DragonBones.Point();   //线段从起点到终点与碰撞盒相交的第一个交点
    private readonly DragonBones.Point _intersectionPointB = new DragonBones.Point();   //线段从终点到起点与碰撞盒相交的第一个交点
    private readonly DragonBones.Point _normalRadians = new DragonBones.Point();        //交点边界框切线的法线弧度，即交点处碰撞盒边线的法线方向
    //---------换装用变量-----------
    private Slot _tailWeaponSlot = null;    //换装武器图片的插槽
    private int _tailWeaponIndex = -1;      //可换装武器的编号
    private static readonly string[] WEAPON_RIGHT_LIST = { "weapon_1004_r", "weapon_1004b_r", "weapon_1004c_r", "weapon_1004d_r", "weapon_1004e_r" };   //换装武器的图片名称数组

    // Start is called before the first frame update
    void Start()
    {
        _armatureComp = GetComponent<UnityArmatureComponent>(); //定义龙骨骨架
        _armatureComp.animation.Play("idle");   //播放待机动画
        _armatureComp.debugDraw = true;         //显示骨架和包围盒线段
        // Load Right Weapon Data
        UnityFactory.factory.LoadDragonBonesData("weapon_1004_show/weapon_1004_show_ske");      //加载武器龙骨数据到缓存
        UnityFactory.factory.LoadTextureAtlasData("weapon_1004_show/weapon_1004_show_tex");     //加载武器龙骨数据到缓存

        _tailWeaponSlot = _armatureComp.armature.GetSlot("weapon");     //定义要换装武器图片的插槽
        _tailWeaponIndex = 0;       //换装武器编号初始化
    }

    // Update is called once per frame
    void Update()
    {
        Move(); //角色移动（播放动画切换相关）
        BoundingBoxCheck(); //碰撞检测
        ChangeWeapon(); //换装武器（插槽图片更换相关）
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        if (x != 0 || y != 0)
        {
            //_armatureComp.transform.Translate(x * moveSpeed, y * moveSpeed, 0, Space.World);
            _armatureComp.transform.rotation = Quaternion.Euler(0, 0, (Vector2.SignedAngle(Vector2.right, new Vector2(x, y))));
            rb.velocity = new Vector2(x * moveSpeed, y * moveSpeed);
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
        for (int i = 0; i < colliderPoints.Count; i++)  //障碍物顶点两两组成一条边，分别进行判断
        {
            if (i == colliderPoints.Count - 1)
            {
                _helpPointA = _armatureComp.transform.InverseTransformPoint(colliderPoints[i].transform.position);  //将障碍物坐标变换到骨架局部坐标
                _helpPointB = _armatureComp.transform.InverseTransformPoint(colliderPoints[0].transform.position);
            }
            else
            {
                _helpPointA = _armatureComp.transform.InverseTransformPoint(colliderPoints[i].transform.position);
                _helpPointB = _armatureComp.transform.InverseTransformPoint(colliderPoints[i + 1].transform.position);
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

    void ChangeWeapon()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _tailWeaponIndex++;
            _tailWeaponIndex %= WEAPON_RIGHT_LIST.Length;
            string weaponDisplayName = WEAPON_RIGHT_LIST[this._tailWeaponIndex];    //换装武器的图片名称
            UnityFactory.factory.ReplaceSlotDisplay("weapon_1004_show", "weapon", "weapon_r", weaponDisplayName, _tailWeaponSlot);  //更换插槽图片，各参数名称分别为1.工程名字 2.骨架名字 3.要替换的插槽名字（或图层名） 4.要替换的图片名字 5.被替换的插槽实例
        }
    }
}
