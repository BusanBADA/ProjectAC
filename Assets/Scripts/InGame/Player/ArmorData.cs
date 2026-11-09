using UnityEngine;

public enum ArmorType
{
    None,
    RHA,
    Composite,
    Crusher
}


public class ArmorData : MonoBehaviour
{
    public GameObject armorPF;

    [Header("Armor Info")]
    public string armorID;//ID
    public bool isEquipped;//장착여부
    public float armorWeight;//무게

    [Header("Status")]
    public float maxDurability = 100f;//최대 내구도
    public float currentDurability;//현재 내구도
    public float DEF;//기본 방어력 
    public float apDEF; // 관통에 대한 방어력 계수(CompositeArmor = 1.4f, RHA = 1f, )
    public float bombDEF; //폭발에 대한 방어력 계수(CompositeArmor = , RHA = 1f, )

    [Header("ArmorType")]
    public ArmorType armorType;//타입

    [Header("[only Crusher status]")]
    public float damage; // 크러셔가 주는 데미지
    public float knockback; // 죽지 않은 적 밀치기
}