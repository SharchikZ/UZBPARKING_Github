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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A simple example manager for how the car selection scene works. 
/// </summary>
public class RCC_CarSelectionExample : MonoBehaviour
{

    private List<RCC_CarControllerV3> _spawnedVehicles = new List<RCC_CarControllerV3>();       // Our spawned vehicle list. No need to instantiate same vehicles over and over again. 

    public Transform spawnPosition;     // Spawn transform.
    public int selectedIndex = 0;           // Selected vehicle index. Next and previous buttons are affecting this value.

    public RCC_Camera RCCCamera;        // Enabling / disabling camera selection script on RCC Camera if choosen.
    //public string nextScene;        //  Name of the target scene when we select the vehicle.
    //public Text CarName;
    public Button SelectButton;
    public GameObject BuyButton;
    public AudioSource[] eStart;
    public GameObject NoPanel;
    public Text currentCarPrice;

    private void Start()
    {

        //	Getting RCC Camera.
        if (!RCCCamera)
            RCCCamera = RCC_SceneManager.Instance.activePlayerCamera;

        // First, we are instantiating all vehicles and store them in _spawnedVehicles list.
        CreateVehicles();
        StartCoroutine(Checker());

    }

    /// <summary>
    /// Creating all vehicles at once.
    /// </summary>
    private void CreateVehicles()
    {

        for (int i = 0; i < RCC_DemoVehicles.Instance.vehicles.Length; i++)
        {

            // Spawning the vehicle with no controllable, no player, and engine off. We don't want to let player control the vehicle while in selection menu.
            RCC_CarControllerV3 spawnedVehicle = RCC.SpawnRCC(RCC_DemoVehicles.Instance.vehicles[i], spawnPosition.position, spawnPosition.rotation, false, false, false);

            // Disabling spawned vehicle. 
            spawnedVehicle.gameObject.SetActive(false);

            // Adding and storing it in _spawnedVehicles list.
            _spawnedVehicles.Add(spawnedVehicle);

        }

        SpawnVehicle();

        // If RCC Camera is choosen, it wil enable RCC_CameraCarSelection script. This script was used for orbiting camera.
        if (RCCCamera)
        {

            if (RCCCamera.GetComponent<RCC_CameraCarSelection>())
                RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = true;

        }

    }

    /// <summary>
    /// Increasing selected index, disabling all other vehicles, enabling current selected vehicle.
    /// </summary>
    public void NextVehicle()
    {

        selectedIndex++;

        // If index exceeds maximum, return to 0.
        if (selectedIndex > _spawnedVehicles.Count - 1)
            selectedIndex = 0;

        SpawnVehicle();

    }

    /// <summary>
    /// Decreasing selected index, disabling all other vehicles, enabling current selected vehicle.
    /// </summary>
    public void PreviousVehicle()
    {

        selectedIndex--;

        // If index is below 0, return to maximum.
        if (selectedIndex < 0)
            selectedIndex = _spawnedVehicles.Count - 1;

        SpawnVehicle();

    }
    
    /// <summary>
    /// Spawns the current selected vehicle.
    /// </summary>
    public void SpawnVehicle()
    {

        // Disabling all vehicles.
        for (int i = 0; i < _spawnedVehicles.Count; i++)
            _spawnedVehicles[i].gameObject.SetActive(false);        



        // And enabling only selected vehicle.
        _spawnedVehicles[selectedIndex].gameObject.SetActive(true);

        RCC_SceneManager.Instance.activePlayerVehicle = _spawnedVehicles[selectedIndex];
        if (Manager.Instance.myData.availableCars[selectedIndex])
        {
            BuyButton.SetActive(false);
            SelectButton.gameObject.SetActive(true);
        }
        else
        {
            BuyButton.SetActive(true);
            currentCarPrice.text = Manager.Instance.myData.priceList[selectedIndex].ToString("N0");
            Debug.Log("pokupay, rabotaet");
            SelectButton.gameObject.SetActive(false);
        }

        //string objectName = _spawnedVehicles[selectedIndex].name.Replace("(Clone)", "");
        //CarName.text = objectName;

    }

    /// <summary>
    /// Registering the spawned vehicle as player vehicle, enabling controllable.
    /// </summary>
    ///
    public void SpawnExactVehicle(int index)
    {
        selectedIndex = index;
        SpawnVehicle();
    }
    public void SelectVehicle()
    {


        // Save the selected vehicle for instantianting it on next scene.
        PlayerPrefs.SetInt("SelectedRCCVehicle", selectedIndex);

        // If RCC Camera is choosen, it will disable RCC_CameraCarSelection script. This script was used for orbiting camera.
        if (RCCCamera)
        {

            if (RCCCamera.GetComponent<RCC_CameraCarSelection>())
                RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = false;

        }

       // if (!string.IsNullOrEmpty(nextScene))
        //    OpenScene();

    }
    public void BuyVehicle()
    {
        if (Manager.Instance.myData.moneyFinance >= Manager.Instance.myData.priceList[selectedIndex])
        {
            Manager.Instance.myData.moneyFinance -= Manager.Instance.myData.priceList[selectedIndex];
            Manager.Instance.myData.availableCars[selectedIndex] = true;
            //eStart[selectedIndex].Play();
            Manager.Instance.UpdateUI();
            BuyButton.SetActive(false);
            SelectButton.gameObject.SetActive(true);
            Manager.Instance.SaveData();
        }
        else
        {
            NoPanel.SetActive(true);
            SelectButton.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Deactivates selected vehicle and returns to the car selection.
    /// </summary>
    public void DeSelectVehicle()
    {

        // De-registers the vehicle.
        RCC.DeRegisterPlayerVehicle();

        // Resets position and rotation.
        _spawnedVehicles[selectedIndex].transform.position = spawnPosition.position;
        _spawnedVehicles[selectedIndex].transform.rotation = spawnPosition.rotation;

        // Kills engine and disables controllable.
        _spawnedVehicles[selectedIndex].KillEngine();
        _spawnedVehicles[selectedIndex].SetCanControl(false);

        // Resets the velocity of the vehicle.
        _spawnedVehicles[selectedIndex].GetComponent<Rigidbody>().ResetInertiaTensor();
        _spawnedVehicles[selectedIndex].GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        _spawnedVehicles[selectedIndex].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // If RCC Camera is choosen, it wil enable RCC_CameraCarSelection script. This script was used for orbiting camera.
        if (RCCCamera)
        {

            if (RCCCamera.GetComponent<RCC_CameraCarSelection>())
                RCCCamera.GetComponent<RCC_CameraCarSelection>().enabled = true;

        }

    }

    /// <summary>
    /// Loads the target scene.
    /// </summary>

    IEnumerator Checker()
    {
        yield return null;
        yield return new WaitForSeconds(1);
        while (true)
        {
            
            IfSmth();
            yield return new WaitForSeconds(.2f);
            
        }

    }
    private void IfSmth()
    {


        if (selectedIndex == 0)
        {
            SelectButton.interactable = true;
        }

    }

}
