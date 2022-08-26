using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownManager : MonoBehaviour
{
    [SerializeField] private SlotsData slotsData;
    [SerializeField] private List<BarCooldownData> barCooldownData;
    
    private void Awake()
    {
        for (int i = 0; i < slotsData.barItems.Length; i++)
        {
            if (slotsData.barItems[i] != null)
            {
                BarCooldownData newData = new BarCooldownData();

                switch (slotsData.barItems[i].barActionType)
                {
                    case BarActionTypes.Empty:
                        break;
                    case BarActionTypes.Weapon:
                        newData.barName = slotsData.barItems[i].weaponData.weaponType.ToString();
                        newData.barDefaultCooldown = slotsData.barItems[i].weaponData.cooldown;
                        break;
                    case BarActionTypes.Skill:
                        newData.barName = slotsData.barItems[i].skillData.spellType.ToString();
                        newData.barDefaultCooldown = slotsData.barItems[i].skillData.cooldown;
                        break;
                    case BarActionTypes.Item:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                barCooldownData.Add(newData);
            }
        }
    }
    private void Update()
    {
        //barCooldownData[index].barCurrentCooldown = barCooldownData[index].barDefaultCooldown;
        for (int i = 0; i < barCooldownData.Count; i++)
        {
            if (barCooldownData[i].barCurrentCooldown <= 0)
            {
                barCooldownData[i].barCurrentCooldown = 0;
                barCooldownData[i].isReady = true;
            }
            else
            {
                barCooldownData[i].barCurrentCooldown -= Time.deltaTime;
                barCooldownData[i].isReady = false;
            }
        }
    }
}
