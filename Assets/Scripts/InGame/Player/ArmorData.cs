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

    [Header("SharedStatus")]
    public float maxDurability = 100f;//최대 내구도
    public float currentDurability;//현재 내구도

    [Header("ArmorType")]
    public ArmorType armorType;//타입 

    [Header("[only RHA status]")]
    public float RHAThickness;//철판 두께 넘으면 데미지 그대로 입음.

    [Header("[only CompositeArmor status]")]
    public float compositeRHAThickness;//두께 넘으면 데미지 그대로 입음. 
    public float explosiveResistanceMultiplier = 1.5f; // 폭발에 1.5배 강하다.

    public bool CalculateHit(float incomingPenetration, bool isExplosive)
    {
        if (armorType == ArmorType.RHA)
        {
            // RHA: 적의 관통력이 철판 두께보다 크면 뚫림, 작거나 같으면 방어 성공
            if (incomingPenetration > RHAThickness)
            {
                Debug.Log($"[RHA 장갑] 관통당함! (탄 관통력: {incomingPenetration} > 장갑 두께: {RHAThickness})");
                return false;
            }
            else
            {
                Debug.Log($"[RHA 장갑] 방어 성공! (탄 관통력: {incomingPenetration} <= 장갑 두께: {RHAThickness})");
                return true;
            }
        }
        else if (armorType == ArmorType.Composite)
        {
            // 복합장갑: 기본 환산 방어력 계산
            float effectiveDefense = compositeRHAThickness;

            // 폭발물 탄환일 경우, 복합장갑의 특수 배율 적용하여 방어력 상승
            if (isExplosive)
            {
                effectiveDefense *= explosiveResistanceMultiplier;
            }

            // 관통력과 유효 방어력 비교
            if (incomingPenetration > effectiveDefense)
            {
                Debug.Log($"[복합장갑] 관통당함! (탄 관통력: {incomingPenetration} > 유효 방어력: {effectiveDefense})");
                return false;
            }
            else
            {
                Debug.Log($"[복합장갑] 방어 성공! (탄 관통력: {incomingPenetration} <= 유효 방어력: {effectiveDefense})");
                return true;
            }
        }

        // 장갑 타입이 None이거나 없는 경우
        return false;
    }
}