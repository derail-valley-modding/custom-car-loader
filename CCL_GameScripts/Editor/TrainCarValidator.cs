using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CCL_GameScripts
{
    public class TrainCarValidator : EditorWindow
    {
        private static TrainCarValidator window = null;

        private readonly List<Result> results = new List<Result>();
        private TrainCarSetup trainCarSetup = null;
        private bool validationPassed = false;

        private GameObject prefabObject => trainCarSetup.gameObject;
        private Transform prefabRoot => trainCarSetup.transform;

        private enum ResultStatus
        {
            Pass, Warning, Failed
        }

        private class Result
        {
            public string Name;
            public ResultStatus Status;
            public string Message;

            private Result(string name, ResultStatus status, string message = "")
            {
                Name = name;
                Status = status;
                Message = message;
            }

            public static Result Pass() => new Result(null, ResultStatus.Pass);
            public static Result Warning(string message) => new Result(null, ResultStatus.Warning, message);
            public static Result Failed(string message) => new Result(null, ResultStatus.Failed, message);

            public Color StatusColor
            {
                get
                {
                    switch (Status)
                    {
                        case ResultStatus.Pass: return Color.green;
                        case ResultStatus.Warning: return Color.yellow;
                        case ResultStatus.Failed: return Color.red;
                        default: return Color.black;
                    }
                }
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class CarPrefabTestAttribute : Attribute
        {
            public string TestName;

            public CarPrefabTestAttribute(string testName)
            {
                TestName = testName;
            }
        }

        Vector2 scrollPos = Vector2.zero;

        private void OnGUI()
        {
            Color defaultText = EditorStyles.label.normal.textColor;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginHorizontal();

            // Test Names
            GUILayout.BeginVertical();
            foreach (Result result in results)
            {
                GUILayout.Label($"{result.Name}: ");
            }
            GUILayout.EndVertical();

            // Statuses
            GUILayout.BeginVertical();
            foreach (Result result in results)
            {
                GUI.skin.label.normal.textColor = result.StatusColor;
                GUILayout.Label(Enum.GetName(typeof(ResultStatus), result.Status));
            }
            GUILayout.EndVertical();

            // Messages
            GUI.skin.label.normal.textColor = defaultText;
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            foreach (Result result in results)
            {
                GUILayout.Label(" " + result.Message);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            if (validationPassed)
            {
                GUILayout.Label("Validation passed :)");

                if (GUILayout.Button("Continue Export", GUILayout.Height(GUI.skin.button.lineHeight * 2)))
                {
                    ExportTrainCar.ShowWindow(trainCarSetup);
                    Close();
                }
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                }
            }
            else
            {
                GUILayout.Label("Validation failed - correct errors and try again");
                GUILayout.Label("CCL Wiki: https://github.com/katycat5e/DVCustomCarLoader/wiki");

                if (GUILayout.Button("OK"))
                {
                    Close();
                }
            }
        }


        public static void ValidateExport(TrainCarSetup carSetup)
        {
            window = GetWindow<TrainCarValidator>();
            window.titleContent = new GUIContent("Car Validator");

            if (!carSetup)
            {
                Debug.LogError("Train car setup is null, cannot validate");
                return;
            }

            window.DoValidation(carSetup);

            window.Show();
        }

        private void DoValidation(TrainCarSetup carSetup)
        {
            results.Clear();
            validationPassed = true;
            trainCarSetup = carSetup;

            // run tests
            Run(CheckCarSetup);
            Run(CheckTransform);
            Run(CheckLOD);
            Run(CheckBogies);
            Run(CheckColliders);
            Run(CheckCouplers);
            Run(CheckCabTransform);
        }

        private void Run(Func<Result> test)
        {
            var result = test();

            var attr = test.Method.GetCustomAttributes(typeof(CarPrefabTestAttribute), false)?.FirstOrDefault();
            if (attr is CarPrefabTestAttribute descriptor)
            {
                result.Name = descriptor.TestName;
            }
            else
            {
                result.Name = test.Method.Name;
            }

            results.Add(result);
            
            if (result.Status == ResultStatus.Failed)
            {
                validationPassed = false;
            }
        }

        private void Run(Func<IEnumerator<Result>> test)
        {
            string testName;
            var attr = test.Method.GetCustomAttributes(typeof(CarPrefabTestAttribute), false)?.FirstOrDefault();
            if (attr is CarPrefabTestAttribute descriptor)
            {
                testName = descriptor.TestName;
            }
            else
            {
                testName = test.Method.Name;
            }

            var e = test();
            bool anyFailed = false;

            while (e.MoveNext())
            {
                var result = e.Current;
                result.Name = testName;

                results.Add(result);

                if (result.Status == ResultStatus.Failed)
                {
                    validationPassed = false;
                }

                if (result.Status != ResultStatus.Pass)
                {
                    anyFailed = true;
                }
            }

            if (!anyFailed)
            {
                var pass = Result.Pass();
                pass.Name = testName;
                results.Add(pass);
            }
        }

        [CarPrefabTest("Car Setup")]
        private Result CheckCarSetup()
        {
            if (trainCarSetup.BaseCarType == BaseTrainCarType.NotSet)
            {
                return Result.Failed("Base car type must be set");
            }
            return Result.Pass();
        }

        [CarPrefabTest("Root Transform")]
        private Result CheckTransform()
        {
            if (prefabRoot.position != Vector3.zero)
            {
                return Result.Failed("Not at (0,0,0)");
            }
            if (prefabRoot.eulerAngles != Vector3.zero)
            {
                return Result.Failed("Non-zero rotation");
            }
            if (prefabRoot.localScale != Vector3.one)
            {
                return Result.Failed("Scale is not 1");
            }

            return Result.Pass();
        }

        [CarPrefabTest("LOD Group")]
        private Result CheckLOD()
        {
            var lodGroup = prefabObject.GetComponent<LODGroup>();
            if (lodGroup)
            {
                foreach (var lod in lodGroup.GetLODs())
                {
                    if (lod.renderers.Length == 0)
                    {
                        return Result.Warning("Missing renderers on LOD");
                    }
                }
            }

            return Result.Pass();
        }

        [CarPrefabTest("Bogie Transforms")]
        private IEnumerator<Result> CheckBogies()
        {
            var bogieF = prefabRoot.FindSafe(CarPartNames.BOGIE_FRONT);
            if (!bogieF)
            {
                yield return Result.Failed("Missing front bogie transform");
            }
            else
            {
                if (bogieF.transform.position.y != 0)
                {
                    yield return Result.Failed("BogieF must be at y=0");
                }

                if (trainCarSetup.UseCustomFrontBogie)
                {
                    var bogieCar = bogieF.FindSafe(CarPartNames.BOGIE_CAR);
                    if (!bogieCar)
                    {
                        yield return Result.Failed($"Missing {CarPartNames.BOGIE_CAR} child for custom front bogie");
                    }
                }
            }

            var bogieR = prefabRoot.FindSafe(CarPartNames.BOGIE_REAR);
            if (!bogieR)
            {
                yield return Result.Failed("Missing rear bogie transform");
            }
            else
            {
                if (bogieR.transform.position.y != 0)
                {
                    yield return Result.Failed("BogieR must be at y=0");
                }

                if (trainCarSetup.UseCustomRearBogie)
                {
                    var bogieCar = bogieR.FindSafe(CarPartNames.BOGIE_CAR);
                    if (!bogieCar)
                    {
                        yield return Result.Failed($"Missing {CarPartNames.BOGIE_CAR} child for custom rear bogie");
                    }
                }
            }
        }

        [CarPrefabTest("Colliders")]
        private IEnumerator<Result> CheckColliders()
        {
            // root
            var collidersRoot = prefabRoot.FindSafe(CarPartNames.COLLIDERS_ROOT);
            if (!collidersRoot)
            {
                yield return Result.Failed($"{CarPartNames.COLLIDERS_ROOT} root is missing entirely!");
            }

            // bounding collider
            var collision = collidersRoot.FindSafe(CarPartNames.COLLISION_ROOT);
            var collisionComp = collision ? collision.GetComponent<BoxCollider>() : null;
            if (!(collision && collisionComp))
            {
                yield return Result.Warning($"Bounding {CarPartNames.COLLISION_ROOT} collider will be auto-generated");
            }

            // walkable
            var walkable = collidersRoot.FindSafe(CarPartNames.WALKABLE_COLLIDERS);
            var walkableComp = walkable ? walkable.GetComponentsInChildren<Collider>() : Enumerable.Empty<Collider>();
            if (!walkable || !walkableComp.Any())
            {
                yield return Result.Failed($"No {CarPartNames.WALKABLE_COLLIDERS} colliders set - car has no player collision");
            }

            // bogies
            if (trainCarSetup.UseCustomFrontBogie || trainCarSetup.UseCustomRearBogie)
            {
                var bogies = collidersRoot.FindSafe(CarPartNames.BOGIE_COLLIDERS);

                int expectedNumColliders = trainCarSetup.UseCustomFrontBogie ? 1 : 0;
                expectedNumColliders += trainCarSetup.UseCustomRearBogie ? 1 : 0;

                var bogieColliders = bogies.GetComponentsInChildren<Collider>();
                if (!bogies || bogieColliders.Length != expectedNumColliders)
                {
                    yield return Result.Failed($"One or more {CarPartNames.BOGIE_COLLIDERS} colliders missing for custom bogies");
                }
            }
        }

        [CarPrefabTest("Couplers & Buffers")]
        private IEnumerator<Result> CheckCouplers()
        {
            var frontRig = prefabRoot.FindSafe(CarPartNames.COUPLER_RIG_FRONT);
            if (!frontRig)
            {
                yield return Result.Failed("Missing front coupler rig " + CarPartNames.COUPLER_RIG_FRONT);
            }
            else
            {
                if (frontRig.position.x != 0 || frontRig.position.y != 1.05f)
                {
                    yield return Result.Warning("Front coupler rig should be at x = 0, y = 1.05");
                }

                if (trainCarSetup.UseCustomBuffers)
                {
                    foreach (string name in CarPartNames.BUFFER_FRONT_PADS)
                    {
                        var pad = frontRig.FindSafe(name);
                        if (!pad)
                        {
                            yield return Result.Warning("Missing buffer pad " + name);
                        }
                    }
                }
            }

            var rearRig = prefabRoot.FindSafe(CarPartNames.COUPLER_RIG_REAR);
            if (!rearRig)
            {
                yield return Result.Failed("Missing rear coupler rig " + CarPartNames.COUPLER_RIG_REAR);
            }
            else
            {
                if (rearRig.position.x != 0 || rearRig.position.y != 1.05f)
                {
                    yield return Result.Warning("Rear coupler rig should be at x = 0, y = 1.05");
                }

                if (trainCarSetup.UseCustomBuffers)
                {
                    foreach (string name in CarPartNames.BUFFER_REAR_PADS)
                    {
                        var pad = rearRig.FindSafe(name);
                        if (!pad)
                        {
                            yield return Result.Warning("Missing buffer pad " + name);
                        }
                    }
                }
            }
        }

        [CarPrefabTest("Interior")]
        private Result CheckCabTransform()
        {
            if (trainCarSetup.InteriorPrefab)
            {
                if (trainCarSetup.InteriorPrefab.transform.position != Vector3.zero)
                {
                    return Result.Warning("Interior is not centered at (0,0,0)");
                }
            }

            return Result.Pass();
        }
    }
}
