using UnityEngine;

public enum ArmorType
{
    None,
    RHA,
    Composite
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

    [Header("areaPerEffectiveArmor")]
    public float area;           // 면적
    public float effectiveArmor; //유효장갑량
    public float areaPerEffectiveArmor;//면적당 유효장갑량

    [Header("ArmorType")]
    public ArmorType armorType;//타입

    // 장갑 종류를 바꿨을 때 기본 계수를 자동으로 잡아주는 편의 기능
    public void SetArmor(ArmorType _type)
    {
        armorType = _type;

        switch (_type)
        {
            case ArmorType.None://X
                def = 0f;
                apDEF = 0f;
                bombDEF = 0f;
                armorWeight = 0f;

                area = 0f;
                effectiveArmor = 0f;
                break;

            case ArmorType.RHA://일반 장갑
                def = 0f;
                apDEF = 1f;
                bombDEF = 1f;
                armorWeight = 30f;

                area = 10f;
                effectiveArmor = 10f;
                break;

            case ArmorType.Composite:
                def = 0f;
                apDEF = 1.6f;   // 관통에 강함
                bombDEF = 1.2f; // 폭발에 약간 강함
                armorWeight = 40f;

                area = 10f;
                effectiveArmor = 20f;
                break;
        }

        //면적당 유효장갑량
        effectiveArmor = area * areaPerEffectiveArmor;
    }
}