//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2023 BoneCracker Games
// https://www.bonecrackergames.com
// Buğra Özdoğanlar
//
//----------------------------------------------

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using Photon.Pun;

/// <summary>
/// Streaming player input, or receiving data from server. And then feeds the RCC.
/// </summary>
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(RCC_CarControllerV3))]
[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Network/Photon/RCC Photon Network")]
public class RCC_PhotonNetwork : Photon.Pun.MonoBehaviourPunCallbacks, IPunObservable {

    public bool connected = false;
    public bool isMine = false;

    // Main RCC, Rigidbody, and Main RCC Camera of the scene. 
    private RCC_CarControllerV3 carController;
    private RCC_WheelCollider[] wheelColliders;
    private Rigidbody rigid;

    // Vehicle position and rotation. Will send these to server.
    private Vector3 correctPlayerPos;
    private Quaternion correctPlayerRot;

    // Used for projected (interpolated) position.
    private Vector3 currentVelocity;
    private float updateTime = 0;

    private RCC_CustomizationApplier customizationApplier;
    private bool customizationSynced = false;

    // Данные кастомизации
    private string customizationJson = "";
    private string plateJson = "";

    // All inputs for RCC. We will send these values if we own this vehicle. If this vehicle is owned by other player, receives all these inputs from server.
    private float gasInput = 0f;
    private float brakeInput = 0f;
    private float steerInput = 0f;
    private float handbrakeInput = 0f;
    private float boostInput = 0f;
    private float clutchInput = 0f;
    private int gear = 0;
    private int direction = 1;
    private bool changingGear = false;
    private bool semiAutomaticGear = false;
    private float fuelInput = 1f;
    private bool engineRunning = false;

    // Lights.
    private bool lowBeamHeadLightsOn = false;
    private bool highBeamHeadLightsOn = false;
    float fRadius = 1f, fWidth = 1f, rRadius = 1f, rWidth = 1f;

    // For Indicators.
    private RCC_CarControllerV3.IndicatorsOn indicatorsOn;

    // For Nickname Text
    private TextMesh nicknameText;

    private void Awake()
    {
        carController = GetComponent<RCC_CarControllerV3>();
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        wheelColliders = GetComponentsInChildren<RCC_WheelCollider>();
        customizationApplier = GetComponent<RCC_CustomizationApplier>();

        // Создаем никнейм
        GameObject nicknameTextGO = new GameObject("NickName TextMesh");
        nicknameTextGO.transform.SetParent(transform, false);
        nicknameTextGO.transform.localPosition = new Vector3(0f, 2f, 0f);
        nicknameTextGO.transform.localScale = new Vector3(.25f, .25f, .25f);
        nicknameText = nicknameTextGO.AddComponent<TextMesh>();
        nicknameText.anchor = TextAnchor.MiddleCenter;
        nicknameText.fontSize = 15;

        if (!gameObject.GetComponent<PhotonView>().ObservedComponents.Contains(this))
            gameObject.GetComponent<PhotonView>().ObservedComponents.Add(this);

        gameObject.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Unreliable;

        if (photonView.IsMine)
        {
            // 1. Собираем данные колес
            float fRad = 1f, fWid = 1f, rRad = 1f, rWid = 1f;
            CustomWheelScaler[] scalers = GetComponentsInChildren<CustomWheelScaler>();
            foreach (var s in scalers)
            {
                if (s.isFrontWheel) { fRad = s.radiusScale; fWid = s.wheelWidthScale; }
                else { rRad = s.radiusScale; rWid = s.wheelWidthScale; }
            }

            // 2. Собираем данные кастомизации RCC
            string myCustomJson = JsonUtility.ToJson(customizationApplier.loadout);

            // 3. Собираем данные номеров (ВАЖНО: собираем ДО отправки)
            string myPlateJson = "";
            UzbDigit digit = GetComponentInChildren<UzbDigit>();
            UZBBukva bukva = GetComponentInChildren<UZBBukva>();
            RegionalPlateController regional = GetComponentInChildren<RegionalPlateController>();

            if (digit != null && bukva != null && regional != null)
            {
                PlateLoadout plate = new PlateLoadout();
                plate.digits = digit.GetDigits();
                plate.letters = bukva.GetDigits();
                plate.regionalIndex = regional.currentRegionalIndex;

                // Сохраняем позиции для синхронизации
                Vector3[] dPositions = digit.GetPositions();
                plate.digitPosX = new float[dPositions.Length];
                plate.digitPosY = new float[dPositions.Length];
                plate.digitPosZ = new float[dPositions.Length];
                for (int i = 0; i < dPositions.Length; i++)
                {
                    plate.digitPosX[i] = dPositions[i].x;
                    plate.digitPosY[i] = dPositions[i].y;
                    plate.digitPosZ[i] = dPositions[i].z;
                }
                Vector3 lPos = bukva.GetLetterPos();
                plate.letterPosX = lPos.x; plate.letterPosY = lPos.y; plate.letterPosZ = lPos.z;

                myPlateJson = JsonUtility.ToJson(plate);
            }

            // ОТПРАВЛЯЕМ ВСЕ 6 ПАРАМЕТРОВ
            photonView.RPC("RPC_ApplyCustomization", RpcTarget.AllBuffered, myCustomJson, myPlateJson, fRad, fWid, rRad, rWid);
        }

        GetValues();
        gameObject.name = gameObject.name + photonView.ViewID;
    }

    private void GetValues() {

        correctPlayerPos = transform.position;
        correctPlayerRot = transform.rotation;

        gasInput = carController.throttleInput;
        brakeInput = carController.brakeInput;
        steerInput = carController.steerInput;
        handbrakeInput = carController.handbrakeInput;
        boostInput = carController.boostInput;
        clutchInput = carController.clutchInput;
        gear = carController.currentGear;
        direction = carController.direction;
        changingGear = carController.changingGear;
        semiAutomaticGear = carController.semiAutomaticGear;

        fuelInput = carController.fuelInput;
        engineRunning = carController.engineRunning;
        lowBeamHeadLightsOn = carController.lowBeamHeadLightsOn;
        highBeamHeadLightsOn = carController.highBeamHeadLightsOn;
        indicatorsOn = carController.indicatorsOn;

    }

    private void FixedUpdate() {

        if (!PhotonNetwork.IsConnectedAndReady)
            return;

        if (!carController)
            return;


        isMine = photonView.IsMine;


        if (photonView.OwnershipTransfer == OwnershipOption.Fixed) {

            // If we are the owner of this vehicle, disable external controller and enable controller of the vehicle. Do opposite if we don't own this.
            carController.externalController = !isMine;
            carController.canControl = isMine;

        }

        // If we are not owner of this vehicle, receive all inputs from server.
        if (!isMine) {

            Vector3 projectedPosition = this.correctPlayerPos + currentVelocity * (Time.time - updateTime);

            if (Vector3.Distance(transform.position, correctPlayerPos) < 15f) {

                transform.position = Vector3.Lerp(transform.position, projectedPosition, Time.deltaTime * 5f);
                transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5f);

            } else {

                transform.position = correctPlayerPos;
                transform.rotation = correctPlayerRot;

            }

            carController.throttleInput = gasInput;
            carController.brakeInput = brakeInput;
            carController.steerInput = steerInput;
            carController.handbrakeInput = handbrakeInput;
            carController.boostInput = boostInput;
            carController.clutchInput = clutchInput;
            carController.currentGear = gear;
            carController.direction = direction;
            carController.changingGear = changingGear;
            carController.semiAutomaticGear = semiAutomaticGear;

            carController.fuelInput = fuelInput;
            carController.engineRunning = engineRunning;
            carController.lowBeamHeadLightsOn = lowBeamHeadLightsOn;
            carController.highBeamHeadLightsOn = highBeamHeadLightsOn;
            carController.indicatorsOn = indicatorsOn;

            if (nicknameText && photonView.Owner != null)
                nicknameText.text = photonView.Owner.NickName;
            else
                nicknameText.text = "";

            if (RCC_SceneManager.Instance.activeMainCamera) {

                nicknameText.transform.LookAt(RCC_SceneManager.Instance.activeMainCamera.transform);
                nicknameText.transform.rotation = Quaternion.Euler(nicknameText.transform.eulerAngles.x, nicknameText.transform.eulerAngles.y + 180f, nicknameText.transform.eulerAngles.z);

            }

        } else {

            if (nicknameText)
                nicknameText.text = "";

        }

    }

    [PunRPC]
    public void RPC_ApplyCustomization(string customJson, string plateJson, float fRad, float fWid, float rRad, float rWid)
    {
        // 1. Инициализация апплайера
        if (customizationApplier == null)
            customizationApplier = GetComponent<RCC_CustomizationApplier>();

        customizationApplier.autoLoadLoadout = false;

        // 2. Применяем визуальный тюнинг RCC
        if (!string.IsNullOrEmpty(customJson))
        {
            customizationApplier.loadout = JsonUtility.FromJson<RCC_CustomizationLoadout>(customJson);
            if (customizationApplier.PaintManager) customizationApplier.PaintManager.Initialize();
            if (customizationApplier.WheelManager) customizationApplier.WheelManager.Initialize();
            if (customizationApplier.UpgradeManager) customizationApplier.UpgradeManager.Initialize();
            if (customizationApplier.SpoilerManager) customizationApplier.SpoilerManager.Initialize();
            if (customizationApplier.SirenManager) customizationApplier.SirenManager.Initialize();
        }

        // 3. Применяем тюнинг колес (стенс/радиус)
        CustomWheelScaler[] scalers = GetComponentsInChildren<CustomWheelScaler>();
        foreach (var s in scalers)
        {
            if (s.isFrontWheel) s.ApplyNetworkedScale(fRad, fWid);
            else s.ApplyNetworkedScale(rRad, rWid);
        }

        // 4. Применяем номера
        if (!string.IsNullOrEmpty(plateJson) && plateJson != "{}")
        {
            PlateLoadout plate = JsonUtility.FromJson<PlateLoadout>(plateJson);
            UzbDigit digit = GetComponentInChildren<UzbDigit>();
            UZBBukva bukva = GetComponentInChildren<UZBBukva>();
            RegionalPlateController regional = GetComponentInChildren<RegionalPlateController>();

            if (digit != null && plate.digits != null)
            {
                Vector3[] positions = new Vector3[plate.digitPosX.Length];
                for (int i = 0; i < positions.Length; i++)
                    positions[i] = new Vector3(plate.digitPosX[i], plate.digitPosY[i], plate.digitPosZ[i]);
                digit.Restore(plate.digits, positions);
            }
            if (bukva != null && plate.letters != null)
            {
                Vector3 lPos = new Vector3(plate.letterPosX, plate.letterPosY, plate.letterPosZ);
                bukva.Restore(plate.letters, lPos);
            }
            if (regional != null)
            {
                regional.currentRegionalIndex = plate.regionalIndex;
                regional.UpdateRegionalDisplay();
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        Debug.Log("SERIALIZE CALLED");

        if (!carController)
            return;

        // Sending all inputs, position, rotation, and velocity to the server.
        if (stream.IsWriting)
        {

            //We own this player: send the others our data
            stream.SendNext(carController.throttleInput);
            stream.SendNext(carController.brakeInput);
            stream.SendNext(carController.steerInput);
            stream.SendNext(carController.handbrakeInput);
            stream.SendNext(carController.boostInput);
            stream.SendNext(carController.clutchInput);
            stream.SendNext(carController.currentGear);
            stream.SendNext(carController.direction);
            stream.SendNext(carController.changingGear);
            stream.SendNext(carController.semiAutomaticGear);

            stream.SendNext(carController.fuelInput);
            stream.SendNext(carController.engineRunning);
            stream.SendNext(carController.lowBeamHeadLightsOn);
            stream.SendNext(carController.highBeamHeadLightsOn);
            stream.SendNext(carController.indicatorsOn);

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rigid.linearVelocity);




        }
        else
        {

            // Network player, receiving all inputs, position, rotation, and velocity from server. 
            gasInput = (float)stream.ReceiveNext();
            brakeInput = (float)stream.ReceiveNext();
            steerInput = (float)stream.ReceiveNext();
            handbrakeInput = (float)stream.ReceiveNext();
            boostInput = (float)stream.ReceiveNext();
            clutchInput = (float)stream.ReceiveNext();
            gear = (int)stream.ReceiveNext();
            direction = (int)stream.ReceiveNext();
            changingGear = (bool)stream.ReceiveNext();
            semiAutomaticGear = (bool)stream.ReceiveNext();

            fuelInput = (float)stream.ReceiveNext();
            engineRunning = (bool)stream.ReceiveNext();
            lowBeamHeadLightsOn = (bool)stream.ReceiveNext();
            highBeamHeadLightsOn = (bool)stream.ReceiveNext();
            indicatorsOn = (RCC_CarControllerV3.IndicatorsOn)stream.ReceiveNext();

            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            currentVelocity = (Vector3)stream.ReceiveNext();



            
        }

            updateTime = Time.time;

        }

    }


#endif