using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FCG
{
    public class TrafficCar : MonoBehaviour
    {

        public enum StatusCar
        {
            transitingNormally,
            waitingForAnotherVehicleToPass,
            stoppedAtTrafficLights,
            bloked, // It's on a dead end street, stand still
            Undefined,
            crashed,

        }


        [HideInInspector]
        public StatusCar status;

        public GameObject BreakLight;
        public GameObject LightLeft = null;
        public GameObject LightRight = null;
        private bool lightDirection = false;

        [HideInInspector]
        public Transform mRayC1;
        [HideInInspector]
        public Transform mRayC2;

        private Vector3 mRayCenter;

        [HideInInspector]
        public Transform[] wheel;

        public WheelCollider[] wCollider;

        private int countWays;
        private Transform[] nodes;

        [HideInInspector]
        public int currentNode = 0;

        [HideInInspector]
        public float distanceToNode;

        private float steer = 0.0f;

        private float speed;

        private float brake = 0;

        private float motorTorque = 0;

        private Vector3 steerCurAngle = Vector3.zero;

        private Rigidbody myRigidbody;


        private Vector3 relativeVector;

        public CarWheelsTransform wheelsTransforms;

        private float timeStoped;

        private Transform myReference;
        private float iRC = 0;
        private float brake2 = 0;

        [HideInInspector]
        public Transform atualWay;

        [HideInInspector]
        public int sideAtual = 0;

        [HideInInspector]
        public FCGWaypointsContainer atualWayScript;

        [HideInInspector]
        public bool nodeSteerCarefully = false;     //true if I'm turning to the side that has opposite traffic (Right hand and turning left) Or (Left hand and turning right)

        [HideInInspector]
        public bool nodeSteerCarefully2 = false;    // For double line oneway

        [HideInInspector]
        public Transform myOldWay;

        [HideInInspector]
        public int myOldSideAtual = 0;

        [HideInInspector]
        public FCGWaypointsContainer myOldWayScript = null;

        private Vector3 _avanceNode = Vector3.zero; //private Position where an additional and momentary node can be added

        private float countTimeToSignal = 0;
        bool toSignal = false;
        private bool toSignalLeft = false;
        private bool toSignalRight = false;
        private Transform behind = null;

        //[HideInInspector]
        public Transform player;

        //[HideInInspector]
        public TrafficSystem tSystem;

        //[HideInInspector]
        public float distanceToSelfDestroy = 0; //0 = Do not autodestroy with player distance


        [System.Serializable]
        public class CarWheelsTransform
        {

            public Transform frontRight;
            public Transform frontLeft;

            public Transform backRight;
            public Transform backLeft;

            public Transform backRight2;
            public Transform backLeft2;

        }

        public CarSetting carSetting;

        [System.Serializable]
        public class CarSetting
        {
            public Transform carSteer;

            [Range(10000, 60000)]
            public float springs = 25000.0f;

            [Range(1000, 6000)]
            public float dampers = 1500.0f;

            [Range(60, 200)]
            public float carPower = 120f;

            [Range(5, 10)]
            public float brakePower = 8f;

            [Range(20, 30)]
            public float limitSpeed = 30.0f;

            [Range(30, 72)]
            public float maxSteerAngle = 40.0f; //Maximum wheel curvature angle

            [Range(-1, 1)]
            public float curveAdjustment = 0.0f; // make tighter or more open turns




        }

        private Vector3 shiftCentre = new Vector3(0.0f, -0.05f, 0.0f);

        private Transform GetTransformWheel(string wheelName)
        {
            GameObject[] wt;

            wt = GameObject.FindObjectsOfType(typeof(GameObject)).Select(g => g as GameObject).Where(g => g.name.Equals(wheelName) && g.transform.parent.root == transform).ToArray();

            if (wt.Length > 0)
                return wt[0].transform;
            else
                return null;

        }


        public void Configure()
        {

            if (!wheelsTransforms.frontRight)
                wheelsTransforms.frontRight = GetTransformWheel("FR");

            if (!wheelsTransforms.frontLeft)
                wheelsTransforms.frontLeft = GetTransformWheel("FL");

            if (!wheelsTransforms.backRight)
                wheelsTransforms.backRight = GetTransformWheel("BR");

            if (!wheelsTransforms.backLeft)
                wheelsTransforms.backLeft = GetTransformWheel("BL");

            if (!wheelsTransforms.backRight2)
                wheelsTransforms.backRight2 = transform.Find("BR2");

            if (!wheelsTransforms.backLeft2)
                wheelsTransforms.backLeft2 = transform.Find("BL2");


            if (!transform.GetComponent<Rigidbody>())
                transform.gameObject.AddComponent<Rigidbody>();

            if (transform.gameObject.GetComponent<Rigidbody>().mass < 4000f)
                transform.gameObject.GetComponent<Rigidbody>().mass = 4000f;

            transform.gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

            if (!wheelsTransforms.frontLeft || !wheelsTransforms.frontRight || !wheelsTransforms.backRight || !wheelsTransforms.backLeft)
            {
                Debug.LogError("wheelsTransforms absent in inspector");
                return;
            }

            float p = wheelsTransforms.frontRight.localPosition.z + 0.6f;
            float l = wheelsTransforms.frontRight.localPosition.x;

            //    
            Transform testC = new GameObject("RayTest").transform;
            testC.SetParent(transform);

            testC.localPosition = new Vector3(0, 0f, wheelsTransforms.frontRight.localPosition.z + 4f);
            testC.LookAt(transform);
            testC.position += new Vector3(0, 0.8f, 0);

            if (Physics.Raycast(testC.position, testC.forward, out RaycastHit hit, 4))
            {
                Debug.DrawRay(testC.position, testC.forward * 4, Color.red);
                p = ((wheelsTransforms.frontRight.localPosition.z + 4f) - hit.distance) - 0.15f;
            }
            else
            {
                Debug.LogError("Adicione um collider e então tente novamente");
            }

            DestroyImmediate(testC.gameObject);


            if (!transform.Find("RayC1"))
            {
                mRayC1 = new GameObject("RayC1").transform;
                mRayC1.SetParent(transform);
            }
            else if (!mRayC1)
                mRayC1 = transform.Find("RayC1");

            mRayC1.localRotation = Quaternion.identity;
            mRayC1.localPosition = new Vector3(-l, 0.8f, p);

            if (!transform.Find("RayC2"))
            {
                mRayC2 = new GameObject("RayC2").transform;
                mRayC2.SetParent(transform);
            }
            else if (!mRayC1)
                mRayC2 = transform.Find("RayC2");

            mRayC2.localRotation = Quaternion.identity;
            mRayC2.localPosition = new Vector3(l, 0.8f, p);

            carSetting.maxSteerAngle = (int)Mathf.Clamp(Vector3.Distance(wheelsTransforms.frontRight.transform.position, wheelsTransforms.backRight.transform.position) * 12, 35, 72);

            wheel = new Transform[4];
            wCollider = new WheelCollider[4];


            GameObject center = new GameObject("Center");
            Vector3[] centerPos = new Vector3[4];
            Vector3 nCenter = new Vector3(0, 0, 0);


            wheel[0] = wheelsTransforms.frontRight;
            wheel[1] = wheelsTransforms.frontLeft;
            wheel[2] = wheelsTransforms.backRight;
            wheel[3] = wheelsTransforms.backLeft;

            for (int i = 0; i < 4; i++)
            {
                wCollider[i] = SetWheelComponent(i);
                // Define CenterOfMass
                center.transform.SetParent(wheel[i].transform);
                center.transform.localPosition = new Vector3(0, 0, 0);
                center.transform.SetParent(transform);
                centerPos[i] = center.transform.localPosition -= new Vector3(0, wCollider[i].radius, 0);
                nCenter += centerPos[i];

            }

            shiftCentre = (nCenter / 4);
            DestroyImmediate(center);



        }

        private WheelCollider SetWheelComponent(int w)
        {

            WheelCollider result;

            Transform wheelCol = transform.Find(wheel[w].name + " - WheelCollider");

            if (wheelCol)
                try
                {
                    DestroyImmediate(wheelCol.gameObject);
                }
                catch { }

            if (wheelCol)
                return wheelCol.GetComponent<WheelCollider>();

            wheelCol = new GameObject(wheel[w].name + " - WheelCollider").transform;

            wheelCol.transform.SetParent(transform);
            wheelCol.transform.position = wheel[w].position;
            wheelCol.transform.eulerAngles = transform.eulerAngles;

            WheelCollider col = (WheelCollider)wheelCol.gameObject.AddComponent(typeof(WheelCollider));

            result = wheelCol.GetComponent<WheelCollider>();

            JointSpring js = col.suspensionSpring;

            js.spring = carSetting.springs;
            js.damper = carSetting.dampers;
            col.suspensionSpring = js;

            col.suspensionDistance = 0.05f;
            col.radius = (wheel[w].GetComponent<MeshFilter>().sharedMesh.bounds.size.z * wheel[w].transform.localScale.z) * 0.5f * 0.92f;
            col.mass = 2000;

            return result;

        }


        void Start()
        {

            timeStoped = Time.time;

            float p = wheelsTransforms.frontRight.localPosition.z + 0.6f;

            if (!transform.Find("RayC1"))
            {
                mRayC1 = new GameObject("RayC1").transform;
                mRayC1.SetParent(transform);
                mRayC1.localRotation = Quaternion.identity;
                mRayC1.localPosition = new Vector3(-0.6f, 0.5f, p);
            }
            else if (!mRayC1)
                mRayC1 = transform.Find("RayC1");


            if (!transform.Find("RayC2"))
            {
                mRayC2 = new GameObject("RayC2").transform;
                mRayC2.SetParent(transform);
                mRayC2.localRotation = Quaternion.identity;
                mRayC2.localPosition = new Vector3(0.6f, 0.5f, p);
            }
            else if (!mRayC1)
                mRayC2 = transform.Find("RayC2");


            myReference = new GameObject("myReference").transform;
            myReference.SetParent(transform);
            myReference.localPosition = new Vector3(0, 0, wheelsTransforms.frontRight.localPosition.z * 0.6f);
            myReference.localRotation = Quaternion.identity;


            
            if(player && !FindObjectOfType<TrafficSystem>())
                Debug.LogError("The Traffic System.prefab not found in the Hierarchy");


            if (atualWay)
                Init();


        }

        public void Init()
        {


            atualWayScript = atualWay.GetComponent<FCGWaypointsContainer>();

            myRigidbody = transform.GetComponent<Rigidbody>();

            myRigidbody.centerOfMass = shiftCentre;

            DefineNewPath();

            if (currentNode == 0) currentNode = 1;

            distanceToNode = Vector3.Distance(atualWayScript.Node(sideAtual, currentNode), myReference.position + myReference.forward * (carSetting.curveAdjustment * 0.5f));

            InvokeRepeating(nameof(MoveCar), 0.02f, 0.02f);

            status = StatusCar.transitingNormally;

            lightDirection = (LightLeft && LightRight);
            if (BreakLight) BreakLight.SetActive(false);
            if (LightLeft) LightLeft.SetActive(false);
            if (LightRight) LightRight.SetActive(false);



        }

        public void ActivateSelfDestructWhenAwayFromThePlayer()
        {

            if (tSystem && player)
            {
                if (distanceToSelfDestroy == 0) distanceToSelfDestroy = 200;
                InvokeRepeating(nameof(SelfDestructWhenAwayFromThePlayer), 5f, 5f);
            }

        }


        public float GetSpeed()
        {
            return speed;
        }

        public bool Get_avanceNode()
        {
            return (currentNode == 0 && nodeSteerCarefully && _avanceNode != Vector3.zero);
        }


        public Vector3 GetNodePosition()
        {
            return atualWayScript.Node(sideAtual, currentNode);
        }

        bool CheckBookAllPathOptions(FCGWaypointsContainer wayScript, int side)
        {

            int total;
            int wSide;
            FCGWaypointsContainer wScript;

            total = (side == 0) ? wayScript.nextWay0.Length : wayScript.nextWay1.Length;

            for (int i = 0; i < total; i++)
            {

                if (side == 0)
                {
                    wScript = wayScript.nextWay0[i];
                    wSide = wayScript.nextWaySide0[i];

                }
                else
                {
                    wScript = wayScript.nextWay1[i];
                    wSide = wayScript.nextWaySide1[i];
                }

                if (wScript)
                {
                    if ((wScript.GetNodeZeroCar(wSide) != null && wScript.GetNodeZeroCar(wSide) != transform) && wScript.GetNodeZeroOldWay(wSide) != myOldWay && (!Get_avanceNode() || !wScript.GetNodeZeroCar(wSide).GetComponent<TrafficCar>().Get_avanceNode()))
                        return false;
                }
                else
                {
                    Debug.LogWarning("wScript Error");
                }
            }

            return true;


        }

        bool BookAllPathOptions(FCGWaypointsContainer wayScript, int side, bool book = true)
        {

            int total;
            int wSide;
            FCGWaypointsContainer wScript;

            total = (side == 0) ? wayScript.nextWay0.Length : wayScript.nextWay1.Length;

            for (int i = 0; i < total; i++)
            {

                if (side == 0)
                {
                    wScript = wayScript.nextWay0[i];
                    wSide = wayScript.nextWaySide0[i];

                }
                else
                {
                    wScript = wayScript.nextWay1[i];
                    wSide = wayScript.nextWaySide1[i];
                }

                if (book)
                {
                    bool force = wScript.GetNodeZeroCar(wSide) && wScript.GetNodeZeroCar(wSide).GetComponent<TrafficCar>().Get_avanceNode();
                    if (!wScript.SetNodeZero(wSide, wayScript.transform, transform, force))
                        return false;
                }
                else
                {

                    wScript.UnSetNodeZero(wSide, transform);

                }


            }

            return true;

        }


        void MoveCar()
        {

            if (status == StatusCar.bloked)
                return;

            if (lightDirection)
            {
                countTimeToSignal++;
                if (countTimeToSignal > 16)
                {
                    countTimeToSignal = 0;
                    toSignal = !toSignal;
                    if (toSignalLeft)
                        LightLeft.SetActive(toSignal);
                    else if (toSignalRight)
                        LightRight.SetActive(toSignal);
                    else
                    {
                        LightLeft.SetActive(false);
                        LightRight.SetActive(false);
                    }

                }
            }




            speed = myRigidbody.velocity.magnitude * 3.6f;

            VerificaPoints();



            distanceToNode = Vector3.Distance(atualWayScript.Node(sideAtual, currentNode), myReference.position + myReference.forward * (carSetting.curveAdjustment * 0.5f));

            if (_avanceNode != Vector3.zero)
            {

                //Debug.DrawLine(transform.position + Vector3.up * 2f, _avanceNode + Vector3.up * 2f, Color.cyan);


                relativeVector = transform.InverseTransformPoint(_avanceNode);

                if (Vector3.Distance(_avanceNode, myReference.position) < 4)
                    _avanceNode = Vector3.zero;

            }
            else
            {

                //Debug.DrawLine(transform.position + Vector3.up * 0.2f, atualWayScript.Node(sideAtual, currentNode, (currentNode == 0 && nodeSteerCarefully) ? 3 : 0) + Vector3.up * 0.2f, Color.cyan);

                relativeVector = transform.InverseTransformPoint(atualWayScript.Node(sideAtual, currentNode, (currentNode == 0 && nodeSteerCarefully) ? 3 : 0));
            }

            steer = ((relativeVector.x / relativeVector.magnitude) * carSetting.maxSteerAngle);

            bool b1 = true;
            bool b2;

            iRC++;
            if (iRC >= 4)
            {

                iRC = 0;

                //nodeSteerCarefully :  true if I'm turning to the side that has opposite traffic (Right hand and turning left) Or (Left hand and turning right)

                if (currentNode == 0)
                {
                    // Decide whether I should wait for another car to pass and then proceed
                    if (behind == null && atualWayScript.BookNodeZero(this))
                    {

                        // The way I was was not oneWay, and I'm turning to the side that has opposite traffic
                        // Decide whether I should wait for another car to pass and then proceed
                        if ((nodeSteerCarefully && !myOldWayScript.oneway) || (nodeSteerCarefully2))
                        {

                            //Reserve the node next to mine in my previous lane, so as not to come by car in the opposite direction.
                            if (!nodeSteerCarefully2)
                                b1 = myOldWayScript.SetNodeZero((myOldSideAtual == 1) ? 0 : 1, myOldWay, transform);

                            b2 = CheckBookAllPathOptions(myOldWayScript, myOldSideAtual) && BookAllPathOptions(myOldWayScript, myOldSideAtual, true);
                            brake2 = (b1 && b2) ? 0 : 4000;

                        }
                        else
                            brake2 = 0;

                    }
                    else
                        brake2 = 4000;

                }
                else
                    brake2 = 0;



                if (speed > 2)
                    status = StatusCar.transitingNormally;

                if (brake2 <= 0 || behind)
                    brake = FixedRaycasts();
                else
                    status = StatusCar.waitingForAnotherVehicleToPass;


                if (speed < 2 && (status != StatusCar.stoppedAtTrafficLights || status != StatusCar.waitingForAnotherVehicleToPass))
                {


                    if (Time.time > timeStoped + 50)
                    {
                        Destroy(transform.gameObject);
                        return;
                    }


                }
                else
                    timeStoped = Time.time;



            }

            brake = (brake2 > brake) ? brake2 : brake;

            if (BreakLight) BreakLight.SetActive(brake > 200);

            float bk = 0;

            if (speed > carSetting.limitSpeed)  // Keep the speed at the set limit
                bk = Mathf.Lerp(100, 1000, (speed - carSetting.limitSpeed) / 10);

            if (bk > brake) brake = bk;

            for (int k = 0; k < 4; k++)
            {

                if (brake == 0)
                    wCollider[k].brakeTorque = 0;
                else
                {
                    wCollider[k].motorTorque = 0;
                    wCollider[k].brakeTorque = carSetting.brakePower * brake;
                }


                if (k < 2)
                {
                    motorTorque = Mathf.Lerp(carSetting.carPower * 30, 0, speed / carSetting.limitSpeed);
                    wCollider[k].motorTorque = motorTorque;
                    wCollider[k].steerAngle = steer;
                }

                wCollider[k].GetWorldPose(out Vector3 _pos, out Quaternion _rot);
                wheel[k].position = _pos;
                wheel[k].rotation = _rot;

            }

            if (wheelsTransforms.backRight2)
            {
                wheelsTransforms.backRight2.rotation = wheelsTransforms.backRight.rotation;
                wheelsTransforms.backLeft2.rotation = wheelsTransforms.backRight.rotation;
            }

            //steeringwheel movement
            if (carSetting.carSteer)
                carSetting.carSteer.localEulerAngles = new Vector3(steerCurAngle.x, steerCurAngle.y, steerCurAngle.z - steer);  //carSetting.carSteer.localEulerAngles = new Vector3(steerCurAngle.x, steerCurAngle.y, steerCurAngle.z + ((steer / 180) * -30.0f));


        }


        private void VerificaPoints()
        {

            if (distanceToNode < 5)
            {

                if (currentNode < countWays - 1)
                {
                    currentNode++;

                    if (currentNode == 1)
                    {
                        atualWayScript.UnSetNodeZero(sideAtual, transform);  // Release the node so that the cars that were waiting for me to pass can proceed
                        status = StatusCar.transitingNormally;

                        if (nodeSteerCarefully || nodeSteerCarefully2)
                        {
                            myOldWayScript.UnSetNodeZero((myOldSideAtual == 1) ? 0 : 1, transform);
                            BookAllPathOptions(myOldWayScript, myOldSideAtual, false); //Release others nodes so that the cars that were waiting for me to pass can proceed
                        }

                        nodeSteerCarefully = false;
                        nodeSteerCarefully2 = false;

                        if (lightDirection)
                        {
                            toSignalRight = false;
                            toSignalLeft = false;
                            if (LightLeft) LightLeft.SetActive(false);
                            if (LightRight) LightRight.SetActive(false);
                        }
                    }

                }
                else
                {



                    int t = TestWay();

                    //True if the chosen path was the only option
                    bool verify = (sideAtual == 0) ? atualWayScript.nextWay0.Length == 1 : atualWayScript.nextWay1.Length == 1;

                    myOldWay = atualWay;
                    myOldSideAtual = sideAtual;
                    myOldWayScript = atualWayScript;

                    if (sideAtual == 0 && (!atualWayScript.oneway || atualWayScript.doubleLine))
                    {
                        sideAtual = atualWayScript.nextWaySide0[t];
                        atualWayScript = atualWayScript.nextWay0[t];
                    }
                    else
                    {
                        sideAtual = atualWayScript.nextWaySide1[t];
                        atualWayScript = atualWayScript.nextWay1[t];
                    }

                    atualWay = atualWayScript.transform;

                    //The road I'm on has no exit, so interdict the previous road that had only this road as an option
                    if (verify && atualWayScript.bloked)
                    {
                        myOldWayScript.bloked = true;
                        brake2 = 6000;
                        status = StatusCar.bloked;
                    }

                    DefineNewPath();

                    currentNode = 0;

                    float a = GetAngulo(transform, atualWayScript.Node(sideAtual, 0));

                    //if (myOldWayScript.oneway) 
                    if (myOldWayScript.oneway && !myOldWayScript.doubleLine)
                    {
                        if (myOldWayScript.doubleLine)
                            nodeSteerCarefully2 = (myOldSideAtual == 0 && a > 20 && a < 90) || (myOldSideAtual == 1 && a < 340 && a > 270);
                        else
                            nodeSteerCarefully = false;
                    }
                    else
                        nodeSteerCarefully = (atualWayScript.rightHand == 0 && a < 340 && a > 270) || (atualWayScript.rightHand != 0 && a > 20 && a < 90);



                    if (lightDirection)
                    {
                        toSignalLeft = (a < 340 && a > 270);
                        toSignalRight = (a > 20 && a < 90);
                    }

                    if (nodeSteerCarefully)
                    {
                        _avanceNode = myOldWayScript.AvanceNode(myOldSideAtual, myOldWayScript.waypoints.Count - 1, 7);
                    }


                }

            }



        }



        public Transform GetBehind()
        {
            return behind;
        }


        float FixedRaycasts()
        {

            RaycastHit hit;
            float wdist = 6;
            float wdist2 = (speed < 3) ? (wdist / 1.5f) : wdist;

            float rStop;

            mRayC1.localRotation = Quaternion.Euler(0, steer, 0);
            mRayC2.localRotation = mRayC1.localRotation;

            Debug.DrawRay((mRayC1.position + mRayC2.position) * 0.5f, mRayC1.forward * wdist2, Color.yellow);
            Debug.DrawRay(mRayC1.position, mRayC1.forward * wdist2, Color.yellow);
            Debug.DrawRay(mRayC2.position, mRayC2.forward * wdist2, Color.yellow);

            if (Physics.Raycast((mRayC1.position + mRayC2.position) * 0.5f, mRayC1.forward, out hit, wdist2))
            {
                Debug.DrawRay((mRayC1.position + mRayC2.position) * 0.5f, mRayC1.forward * wdist2, Color.red);
                rStop = hit.distance;
            }
            else if (Physics.Raycast(mRayC1.position, mRayC1.forward, out hit, wdist2))
            {
                Debug.DrawRay(mRayC1.position, mRayC1.forward * wdist2, Color.red);
                rStop = hit.distance;
            }
            else if (Physics.Raycast(mRayC2.position, mRayC2.forward, out hit, wdist2))
            {
                Debug.DrawRay(mRayC2.position, mRayC2.forward * wdist2, Color.red);
                rStop = hit.distance;
            }
            else
                rStop = 0;

            behind = (rStop == 0) ? null : hit.transform;

            if (rStop > 0 && speed < 2)
            {

                if (status == StatusCar.stoppedAtTrafficLights || status == StatusCar.waitingForAnotherVehicleToPass || status == StatusCar.Undefined)
                { }
                else if (hit.transform.name == "Stop")
                    status = StatusCar.stoppedAtTrafficLights;
                else if (hit.transform.GetComponent<TrafficCar>())
                {
                    StatusCar st = hit.transform.GetComponent<TrafficCar>().status;
                    status = (st == StatusCar.stoppedAtTrafficLights || st == StatusCar.waitingForAnotherVehicleToPass) ? st : StatusCar.Undefined;
                }

            }

            if (rStop == 0)
                return 0;
            else
                return (rStop < 1 || speed < 0.5f) ? 20000 : (speed * 6) * ((wdist / rStop) * 6);



        }


        void DefineNewPath()
        {

            nodes = new Transform[atualWay.childCount];
            int n = 0;
            foreach (Transform child in atualWay)
                nodes[n++] = child;

            countWays = nodes.Length;


        }


        int TestWay()
        {

            //Check if the path drawn for me is a good option (based on traffic) "VerifyTraffic"
            //Also check if the selected path has been blocked "CheckStoped"


            int total = 0;

            if (sideAtual == 0)
                total = atualWayScript.nextWay0.Length;
            else
                total = atualWayScript.nextWay1.Length;

            int t = Random.Range(0, total); // Sort one of the available paths


            if (total > 1) // If there are more path options to choose from
            {
                if (CheckStoped(t) || VerifyTraffic(t, 30) < 30 || VerifyNodeSteerCarefully2(t))
                {
                    //Test the paths to see which option is best

                    float maior = 0;
                    for (int i = 0; i < total; i++)
                    {

                        if (!CheckStoped(i)) // && !VerifyNodeSteerCarefully2(i))
                        {
                            float vf = VerifyTraffic(i, 30);

                            if (vf == 30)
                                return i;
                            else if (vf < 30)
                            {
                                if (vf > maior)
                                {
                                    maior = vf;
                                    t = i;
                                }
                            }

                        }


                    }

                }

            }

            return t;

        }

        private Vector3 GetNodeNextWay(int way, int node = 0)
        {
            /*
            Returns the position of the specified node, from a chosen path that is among the available paths (vlinked to my current path)
            */

            if (sideAtual == 0)
                return atualWayScript.nextWay0[way].Node(atualWayScript.nextWaySide0[way], node);
            else
                return atualWayScript.nextWay1[way].Node(atualWayScript.nextWaySide1[way], node);

        }

        private bool CheckStoped(int way)
        {
            //Checks if a path is blocked (due to no exit or other reason)

            if (sideAtual == 0)
                return atualWayScript.nextWay0[way].bloked;
            else
                return atualWayScript.nextWay1[way].bloked;

        }


        /*
        bool VerifyDoubleOneWayOption(int t)
        {

            //Vector3 node0 = GetNodeNextWay(t, 0);
            //nodeSteerCarefully2 = (myOldSideAtual == 0 && a > 20 && a < 90) || (myOldSideAtual == 1 && a < 340 && a > 270);
            return true;    
        }
        */

        bool VerifyNodeSteerCarefully2(int t)
        {
            if (atualWayScript.oneway && atualWayScript.doubleLine)
            {
                float a = GetAngulo(transform, GetNodeNextWay(t, 0));
                return (sideAtual == 0 && a > 20 && a < 90) || (sideAtual == 1 && a < 340 && a > 270);

            }
            return false;

        }


        float VerifyTraffic(int t, float mts = 12)
        {

            //Checks if the specified path is a good choice, it may be congested

            RaycastHit hit2;

            Vector3 node0 = GetNodeNextWay(t, 0) + new Vector3(0, 0.5f, 0);
            Vector3 node1 = GetNodeNextWay(t, 1) + new Vector3(0, 0.5f, 0);


            if (Physics.Raycast(node0, node1 - node0, out hit2, mts))
                if (hit2.transform.GetComponent<TrafficCar>())
                {
                    if (hit2.transform.GetComponent<TrafficCar>().GetSpeed() < 8)
                        return hit2.distance;
                    else
                        return mts - 1;
                }

            return mts;

        }

        private void Pause(Vector3 position)
        {
#if UNITY_EDITOR
            if (GameObject.Find("CubePause"))
            {
                GameObject.Find("CubePause").transform.position = position;
                UnityEditor.EditorApplication.isPaused = true;
            }
#endif
        }

        private int countC = 0;

        void SelfDestructWhenAwayFromThePlayer()
        {

            if (speed < 2 && status != StatusCar.stoppedAtTrafficLights && (Time.time > timeStoped + 30) & InTheFieldOfVision(transform.position, player))
            {

                tSystem.nVehicles--;
                Destroy(this.gameObject);

            }
            else if (Vector3.Distance(transform.position, player.position) < distanceToSelfDestroy || InTheFieldOfVision(transform.position, player))
                countC = 0;
            else
            {
                countC++;

                if (countC >= 2)
                {
                    tSystem.nVehicles--;
                    Destroy(this.gameObject);
                }

            }


        }

        public void SelfDestructWhenAwayFromThePlayerInit()
        {
            if (tSystem && player)
                if (Vector3.Distance(transform.position, player.position) > distanceToSelfDestroy && !InTheFieldOfVision(transform.position, player))
                {
                    tSystem.nVehicles--;
                    Destroy(this.gameObject);
                }
                else
                    ActivateSelfDestructWhenAwayFromThePlayer();


        }
           

        bool InTheFieldOfVision(Vector3 source, Transform target)
        {

            // the IACar wants to disappear without it being seen by the camera/player

            RaycastHit obsRay2;

            if (Physics.Linecast(source + Vector3.up * 1f, target.position + Vector3.up * 1f, out obsRay2)) //, ~(1 << LayerMask.NameToLayer("Lattice"))))
            {

                if (obsRay2.transform == target || obsRay2.transform.root == target)
                {
                    //Debug.DrawLine(source, target.position, Color.red);
                    return true;
                }
                else
                {
                    //Debug.DrawLine(source, target.position, Color.green);
                    return false;
                }

            }
            else
            {
                //Debug.DrawLine(source, target.position, Color.blue);
                return true;
            }


        }




        private float GetAngulo(Transform origem, Vector3 target)
        {
            float r;

            GameObject compass = new GameObject("Compass");
            compass.transform.parent = origem;
            compass.transform.localPosition = new Vector3(0, 0, 0);

            compass.transform.LookAt(target);
            r = compass.transform.localEulerAngles.y;

            DestroyImmediate(compass);
            return r;

        }


    }

}