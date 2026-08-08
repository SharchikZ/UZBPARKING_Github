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
using UnityEngine.UI;

/// <summary>
/// RCC Canvas for modification.
/// </summary>
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/UI/RCC Canvas Modification")]
public class RCC_UI_Canvas_Modification : MonoBehaviour {

    //UI Panels.
    [Header("Modify Panels")]
    public GameObject colorClass;
    public GameObject wheelClass;
    public GameObject wheelColorClass;
    public GameObject modificationClass;
    public GameObject upgradesClass;
    public GameObject decalsClass;
    public GameObject neonsClass;
    public GameObject spoilerClass;
    public GameObject nomerClass;
    public GameObject nomerPUTTClass;
    public GameObject lightsFrontClass;
    public GameObject lightsBackClass;
    public GameObject FrontFaceliftClass;
    public GameObject sirenClass;

    //UI Buttons.
    [Header("Modify Buttons")]
    public Button bodyPaintButton;
    public Button rimButton;
    public Button rimColorButton;
    public Button customizationButton;
    public Button upgradeButton;
    public Button decalsButton;
    public Button neonsButton;
    public Button spoilersButton;
    public Button sirensButton;
    public Button nomerButton;
    public Button frontLightButton;
    public Button backlightButton;
    public Button FrontFaceLiftButton;
    public Button NomerPUTTTT;

    [Header("MoneyWindows")]
    public GameObject colorBuyWindow;
    public GameObject WheelColorBuyWindow;
    public GameObject NomerBuyWindow;
    public GameObject OblisvokaBuyWindow;
    public GameObject FLightBuyWindow;
    public GameObject BlightBuyWindow;
    public GameObject MigalkaBuyWindow;
    public GameObject WheelBuyWindow;


    private Color orgButtonColor;

    //UI Texts.
    [Header("Upgrade Levels Texts")]
    public Text speedUpgradeLevel;
    public Text handlingUpgradeLevel;
    public Text brakeUpgradeLevel;

    [Header("My managers")]
    public frontLightManager frontLightManager;
    public BackLightManager backLightManager;
    public OblisManager oblisManager;
    public NomerManager setActiveNomer;

    GameObject currentClass = null;
    GameObject currentBuyWindow = null;
    private void Awake() {

        //Getting original color of the button.
        orgButtonColor = bodyPaintButton.image.color;

    }

    private void Update() {

        RCC_CustomizationApplier currentApplier = RCC_CustomizationManager.Instance.vehicle;

        // If no any player vehicle, disable all buttons and return.
        if (!currentApplier) {

            if (upgradeButton)
                upgradeButton.interactable = false;

            if (spoilersButton)
                spoilersButton.interactable = false;

            if (customizationButton)
                customizationButton.interactable = false;

            if (sirensButton)
                sirensButton.interactable = false;

            if (rimButton)
                rimButton.interactable = false;
                

            if (rimColorButton)
                rimButton.interactable = false;

            if (bodyPaintButton)
                bodyPaintButton.interactable = false;

            return;

        }

        // Setting interactable states of the buttons depending on upgrade managers. 
        //	Ex. If spoiler manager not found, spoiler button will be disabled.
        if (upgradeButton)
        {
            upgradeButton.gameObject.SetActive(currentApplier.UpgradeManager);
            if (upgradeButton.gameObject.activeSelf) {

                if (speedUpgradeLevel)
                    speedUpgradeLevel.text = currentApplier.UpgradeManager.engineLevel.ToString("F0");
                if (handlingUpgradeLevel)
                    handlingUpgradeLevel.text = currentApplier.UpgradeManager.handlingLevel.ToString("F0");
                if (brakeUpgradeLevel)
                    brakeUpgradeLevel.text = currentApplier.UpgradeManager.brakeLevel.ToString("F0");

            }
        }
            

        if (spoilersButton)
            spoilersButton.gameObject.SetActive(currentApplier.SpoilerManager);





        if (sirensButton)
            sirensButton.gameObject.SetActive(currentApplier.SirenManager);

        if (rimButton)
        {
            rimButton.gameObject.SetActive(currentApplier.WheelManager);
            rimColorButton.gameObject.SetActive(currentApplier.WheelManager);
        }
            

        if (bodyPaintButton)
            bodyPaintButton.gameObject.SetActive(currentApplier.PaintManager);


        // Feeding upgrade level texts for engine, brake, and handling.


    }

    /// <summary>
    /// Opens up the target class panel.
    /// </summary>
    /// <param name="activeClass"></param>
    public void ChooseClass(GameObject activeClass, GameObject newBuyWindow = null) {

        if (currentClass != null && currentClass != activeClass)
        {
            currentClass.SetActive(false);
        }

        // Активируем новый класс
        if (activeClass != null)
            activeClass.SetActive(true);

        // Запоминаем текущие
        currentClass = activeClass;
        currentBuyWindow = newBuyWindow; // оставлено только для обратной совместимости со старым инспектором





    }

    /// <summary>
    /// Checks colors of the UI buttons. Ex. If paint class is enabled, color of the button will be green. 
    /// </summary>
    /// <param name="activeButton"></param>
    /// 
    public void SelectColor() => ChooseClass(colorClass, colorBuyWindow);
    public void SelectWheels() => ChooseClass(wheelClass, WheelBuyWindow);
    public void SelectWheelColor() => ChooseClass(wheelColorClass, WheelColorBuyWindow);
    public void SelectModification() => ChooseClass(modificationClass);     // ← твоя новая
    public void SelectUpgrades() => ChooseClass(upgradesClass);
    public void SelectDecals() => ChooseClass(decalsClass);
    public void SelectNeons() => ChooseClass(neonsClass);
    public void SelectSpoiler() => ChooseClass(spoilerClass);
    public void SelectSiren() => ChooseClass(sirenClass, MigalkaBuyWindow);
    public void SelectFrontLights() => ChooseClass(lightsFrontClass, FLightBuyWindow);
    public void SelectBackLights() => ChooseClass(lightsBackClass, BlightBuyWindow);
    public void SelectFrontFacelift() => ChooseClass(FrontFaceliftClass, OblisvokaBuyWindow);
    public void SelectNomer() => ChooseClass(nomerClass, NomerBuyWindow);
    public void SelectNomerPUTT() => ChooseClass(nomerPUTTClass);
    public void CheckButtonColors(Button activeButton) {

        if (bodyPaintButton)
            bodyPaintButton.image.color = orgButtonColor;

        if (rimButton)
            rimButton.image.color = orgButtonColor;

        if (customizationButton)
            customizationButton.image.color = orgButtonColor;

        if (upgradeButton)
            upgradeButton.image.color = orgButtonColor;

        if (decalsButton)
            decalsButton.image.color = orgButtonColor;

        if (neonsButton)
            neonsButton.image.color = orgButtonColor;

        if (spoilersButton)
            spoilersButton.image.color = orgButtonColor;

        if (sirensButton)
            sirensButton.image.color = orgButtonColor;

        activeButton.image.color = new Color(0f, 1f, 0f);

    }

    /// <summary>
    /// Sets auto rotation of the showrooom camera.
    /// </summary>
    /// <param name="state"></param>
    public void ToggleAutoRotation(bool state) {

        RCC_ShowroomCamera showroomCamera = FindObjectOfType<RCC_ShowroomCamera>();

        // If no any showroom camera, return.
        if (!showroomCamera)
            return;

        showroomCamera.ToggleAutoRotation(state);

    }

    /// <summary>
    /// Sets horizontal angle of the showroom camera.
    /// </summary>
    /// <param name="hor"></param>
    public void SetHorizontal(float hor) {

        RCC_ShowroomCamera showroomCamera = FindObjectOfType<RCC_ShowroomCamera>();

        // If no any showroom camera, return.
        if (!showroomCamera)
            return;

        showroomCamera.orbitX = hor;

    }
    /// <summary>
    /// Sets vertical angle of the showroom camera.
    /// </summary>
    /// <param name="ver"></param>
    public void SetVertical(float ver) {

        RCC_ShowroomCamera showroomCamera = FindObjectOfType<RCC_ShowroomCamera>();

        // If no any showroom camera, return.
        if (!showroomCamera)
            return;

        showroomCamera.orbitY = ver;

    }

    public void DisableCustomization() {

        if (RCC_CustomizationDemo.Instance)
            RCC_CustomizationDemo.Instance.DisableCustomization();

    }

}
