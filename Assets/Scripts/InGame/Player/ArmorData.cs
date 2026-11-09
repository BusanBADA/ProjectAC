using UnityEngine;

public enum ArmorType
{
    None,
    RHA,
    Composite,
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
    public float def;//기본 방어력 
    public float apDEF; // 관통에 대한 방어력 계수
    public float bombDEF; //폭발에 대한 방어력 계수

    [Header("ArmorType")]
    public ArmorType armorType;//타입

    [Header("[only Crusher status]")]
    public float damage; // 크러셔가 주는 데미지
    public float knockback; // 죽지 않은 적 밀치기

    // 장갑 종류를 바꿨을 때 기본 계수를 자동으로 잡아주는 편의 기능
    public void SetArmor(ArmorType _type)
    {
        armorType = _type;

        switch (_type)
        {
            case ArmorType.RHA:
                apDEF = 1f;   
                bombDEF = 1f;
                break;

            case ArmorType.Composite:
                apDEF = 1.4f;   // 관통에 강함
                bombDEF = 1.8f; // 폭발에 강함
                break;

            case ArmorType.None:
                apDEF = 0f;
                bombDEF = 0f;
                break;
        }
    }
}