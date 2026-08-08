//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for upgradable wheels.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Customization/RCC Vehicle Upgrade Wheel Manager")]
public class RCC_VehicleUpgrade_WheelManager : MonoBehaviour {

    //  Mod applier.
    private RCC_CustomizationApplier modApplier;
    public RCC_CustomizationApplier ModApplier {

        get {

            if (modApplier == null)
                modApplier = GetComponentInParent<RCC_CustomizationApplier>();

            return modApplier;

        }

    }
    public int defaultWheelIndex = 0;
    /// <summary>
    /// Initializing.
    /// </summary>
    public void Initialize()
    {

        int wheelIndex = ModApplier.loadout.wheel;

        // Если wheel = -1 → нет сохранённых данных, используем дефолт
        if (wheelIndex == -1)
        {
            wheelIndex = defaultWheelIndex;   // уникальный дефолт для каждого префаба
            ModApplier.loadout.wheel = wheelIndex;
            ModApplier.SaveLoadout();
        }

        // Применяем колёса
        RCC_Customization.ChangeWheels(
            ModApplier.CarController,
            RCC_ChangableWheels.Instance.wheels[wheelIndex].wheel,
            true
        );

        // Красим все колёса
        PaintAllWheels(ModApplier.loadout.wheelPaint);
    }

    /// <summary>
    /// Changes the wheel with target wheel index.
    /// </summary>
    /// <param name="wheelIndex"></param>
    public void UpdateWheel(int wheelIndex) {

        ModApplier.loadout.wheel = wheelIndex;
        ModApplier.SaveLoadout();
        RCC_Customization.ChangeWheels(ModApplier.CarController, RCC_ChangableWheels.Instance.wheels[wheelIndex].wheel, false);

    }

    public void PaintAllWheels(Color color) {

        RCCP_WheelPaint[] allWheelPaints = transform.root.GetComponentsInChildren<RCCP_WheelPaint>(true);

        for (int i = 0; i < allWheelPaints.Length; i++) {

            allWheelPaints[i].Paint(color);
            

        }

    }

}
