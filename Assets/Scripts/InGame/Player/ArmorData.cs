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

    [Header("ArmorType")]
    public ArmorType armorType;//타입

    [Header("[only CompositeArmor status]")]
    public float apResistance; // AP탄(고관통탄)에 대한 방어력
}