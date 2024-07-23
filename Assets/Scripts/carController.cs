using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class CarController : MonoBehaviour
{
    public enum ControlMode
    {
        Keyboard,
        Buttons
    };

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectObj;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public ControlMode control;

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;

    public List<Wheel> wheels;

    public float moveInput;
    float steerInput;
    bool respawnInput;

    private Rigidbody carRb;
    public Slider FuelBar;
    public float CarSpeed;

    public GameObject CoinCanvas;
    public GameObject GameCanvas;

    public object Keycode { get; private set; }

    // private CarLights carLights;

    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
        FuelBar.value = 1;
        // carLights = GetComponent<CarLights>();
    }

    void Update()
    {
        GetInputs();
        AnimateWheels();
        WheelEffects();
        fuelSystem();
    }

    void LateUpdate()
    {
        Move();
        Steer();
        Brake();
        carReset();
    }

    public void MoveInput(float input)
    {
        moveInput = input;
    }

    public void SteerInput(float input)
    {
        steerInput = input;
    }

    public void Respawn(bool input)
    {
        respawnInput = input;
    }

    void GetInputs()
    {
        if (control == ControlMode.Keyboard)
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
            respawnInput = Input.GetKey(KeyCode.R);
        }
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {
        if (Input.GetKey(KeyCode.Space) || moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }

            //carLights.isBackLightOn = true;
            //carLights.OperateBackLights();
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }

            //carLights.isBackLightOn = false;
            //carLights.OperateBackLights();
        }
    }

    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }

    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            var dirtParticleMainSettings = wheel.smokeParticle.main;

            if (Input.GetKey(KeyCode.Space) && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded == true && carRb.velocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smokeParticle.Emit(1);
            }
            else
            {
                wheel.wheelEffectObj.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }

    void carReset()
    {
        if(respawnInput == true)
        {
            carRb.rotation = Quaternion.identity;
        }
        
    }

    public void fuelSystem()
    {
        CarSpeed = carRb.velocity.magnitude;
        if (CarSpeed > 10)
        {
            FuelBar.value -= 0.001f;
        }
        if (FuelBar.value == 0)
        {
            moveInput = 0;
            GameCanvas.SetActive(false);
            CoinCanvas.SetActive(true);

        }
    }

    #region Coin
    public Coin coin ;
    public TextMeshProUGUI Cointext;
    public TextMeshProUGUI MsgText;

    public void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Coin")
        {
            coin.CoinVal += 10;
            Cointext.text = coin.ToString();
            Debug.Log("Coin Collected");
            Destroy(other.gameObject);
        }
    }
    #endregion

    #region Fuel
    public void BuyFuel()
    {
       if(coin.CoinVal >=100)
        {
            coin.CoinVal -= 100;
            FuelBar.value = 1f;
            Cointext.text = coin.ToString();
            MsgText.text = "Buy Fuel";
            CoinCanvas.SetActive(false);
            GameCanvas.SetActive(true);
        }
        
        else
        {
            MsgText.text = "Not Enough Money !";
        }
    }
    #endregion

}