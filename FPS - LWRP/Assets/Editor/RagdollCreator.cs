using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI;
using UnityEngine;
using System;

public class RagdollCreator : EditorWindow
{
    [Serializable]
    public struct RagdollBoneData
    {
        public enum ColliderDirection { XAxis, YAxis, ZAxis }

        public string Name;
        public Transform BoneTransform;
        public ColliderDirection colliderDirection;

    }
    public List<RagdollBoneData> Bones;

    private bool showBones;
    private List<bool> showBoneInfo;
    private string[] tempNames;
    private Transform[] tempBoneTransforms;

    public Vector3 vec;

    private Vector2 scrollPos;
    private int arraySizeField;

    private void OnEnable()
    {
        showBoneInfo = new List<bool>();
    }

    [MenuItem("Window/RagdollCreator - Goomer")]
    public static void ShowWindow()
    {
        GetWindow<RagdollCreator>("Ragdoll Creator Helper");
    }

    private void OnGUI()
    {
        ////Init
        var serialObj = new SerializedObject(this); //make this object a serializedObject
        var boneDataStructs = serialObj.FindProperty("Bones");
        var tempSelCol = serialObj.FindProperty("tempSelectedColliders");
        var tempSelNames = serialObj.FindProperty("tempSelectedNames");
        var tempCompList = serialObj.FindProperty("tempComponents");
        var tempConstList = serialObj.FindProperty("tempConstraints");
        var vector3 = serialObj.FindProperty("vec");

        //while (showBoneInfo.Count < boneDataStructs.arraySize)
        //{
        //    showBoneInfo.Add(false);
        //}
        //while (showBones.Count < Bones.Count)
        //{
        //    showBones.Add(true);
        //}

        ////ScrollArea
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

        ////Editor Window Structure//////////////////////////////////////////////////////////////////////////////////

        EditorGUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Height(5)); //creator tag on top
        GUILayout.Label("Ragdoll Creator", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        ////Debug Stuff
        //EditorGUILayout.PropertyField(boneDataStructs, true);

        ////on top of list

        EditorGUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Width(1), GUILayout.Height(1));
        GUILayout.Label("Set Bone Count:");
        arraySizeField = EditorGUILayout.IntField(boneDataStructs.arraySize, GUILayout.Width(30), GUILayout.ExpandHeight(true));
        arraySizeField = Mathf.Clamp(arraySizeField, 0, 100);
        boneDataStructs.arraySize = arraySizeField;

        EditorGUILayout.BeginHorizontal();
        var setBoneNamesButtonPressed = GUILayout.Button("Set Bone Names", GUILayout.Width(165), GUILayout.Height(20));
        var revertLastNameChangeButtonPressed = GUILayout.Button("Revert Last Name Changes", GUILayout.Width(165), GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();

        ////list section
        EditorGUILayout.BeginVertical(GUI.skin.button, GUILayout.Height(0));

        showBones = EditorGUILayout.Foldout(showBones, "Bone List", true, EditorStyles.boldLabel);

        if (showBones)
        {
            for (int z = 0; z < boneDataStructs.arraySize; z++)
            {
                var listRef = boneDataStructs.GetArrayElementAtIndex(z);
                var boneName = listRef.FindPropertyRelative("Name");
                var boneTransform = listRef.FindPropertyRelative("BoneTransform");
                var colDir = listRef.FindPropertyRelative("colliderDirection");

                EditorGUILayout.BeginHorizontal(GUI.skin.button);

                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUIUtility.labelWidth = 75;
                EditorGUILayout.PropertyField(boneName, GUILayout.Width(175));
                EditorGUIUtility.labelWidth = 100;
                EditorGUILayout.PropertyField(boneTransform);
                //EditorGUILayout.PropertyField(colDir);
                EditorGUILayout.EndHorizontal();

                var removeIndexButtonPressed = GUILayout.Button("Remove", EditorStyles.boldLabel, GUILayout.Width(55));

                if (removeIndexButtonPressed) RemoveListIndex(Bones, z);

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();

        ////lower
        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        var addConstraintsButtonPressed = GUILayout.Button("Add Constraints To Bones", GUILayout.Width(250), GUILayout.Height(35));
        var revertConstraintsButtonPressed = GUILayout.Button("Revert Last Constraint Additions", GUILayout.Width(250), GUILayout.Height(35));
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(25);

        //EditorGUILayout.PropertyField(vector3, true);
        EditorGUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Height(5));
        GUILayout.Label("Copy/Paste Ragdoll Constraints Data Tool", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        var copyObjectConstraintsButtonPressed = GUILayout.Button("Copy Selected Objects Constraints Data", GUILayout.Width(250), GUILayout.Height(35));
        var pasteObjectConstraintsButtonPressed = GUILayout.Button("Paste Constraints Data", GUILayout.Width(250), GUILayout.Height(35));
        //var clearObjectComponentsListButtonPressed = GUILayout.Button("Clear Temp List Components", GUILayout.Width(250), GUILayout.Height(35));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(tempConstList, true);

        GUILayout.Space(25);

        EditorGUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Height(5));
        GUILayout.Label("Collider Tool", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        var copyColliderDataButtonPressed = GUILayout.Button("Copy Collider Data", GUILayout.Width(250), GUILayout.Height(35));
        var pasteColliderDataButtonPressed = GUILayout.Button("Paste Collider Data", GUILayout.Width(250), GUILayout.Height(35));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(tempSelCol, true);

        GUILayout.Space(25);

        EditorGUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Height(5));
        GUILayout.Label("Copy Object Components", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        var copyObjectComponentsButtonPressed = GUILayout.Button("Copy Selected Objects Components", GUILayout.Width(250), GUILayout.Height(35));
        var pasteObjectComponentsButtonPressed = GUILayout.Button("Paste Components To Selected Objects", GUILayout.Width(250), GUILayout.Height(35));
        var clearObjectComponentsListButtonPressed = GUILayout.Button("Clear Temp List Components", GUILayout.Width(250), GUILayout.Height(35));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(tempCompList, true);

        GUILayout.Space(25);

        EditorGUILayout.BeginHorizontal(GUI.skin.button, GUILayout.Height(5));
        GUILayout.Label("Naming Helper", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUI.skin.box);
        var copyObjectNameDataButtonPressed = GUILayout.Button("Copy Selected Objects Name", GUILayout.Width(250), GUILayout.Height(35));
        var pasteObjectNameDataButtonPressed = GUILayout.Button("Paste Names To Selected Objects", GUILayout.Width(250), GUILayout.Height(35));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(tempSelNames, true);

        GUILayout.EndScrollView();

        ////Functioning
        serialObj.ApplyModifiedProperties();
        if (setBoneNamesButtonPressed) SetNames();
        if (revertLastNameChangeButtonPressed) RevertLastNameChanges();
        if (addConstraintsButtonPressed) AddConstraintsToBones();
        if (revertConstraintsButtonPressed) RevertConstraintAddition();
        if (copyColliderDataButtonPressed) CopyColliderData();
        if (pasteColliderDataButtonPressed) PasteColliderData();
        if (copyObjectNameDataButtonPressed) CopyObjectName();
        if (pasteObjectNameDataButtonPressed) PasteObjectName();
        if (copyObjectComponentsButtonPressed) CopyObjectsComponents();
        if (pasteObjectComponentsButtonPressed) PasteComponentsToObjects();
        if (clearObjectComponentsListButtonPressed) ClearTempComponentsList();
        if (copyObjectConstraintsButtonPressed) CopyConstraintsData();
        if (pasteObjectConstraintsButtonPressed) PasteConstraintsData();

    }

    private void RemoveListIndex(List<RagdollBoneData> list, int index)
    {
        if (list.Count > 0)
            list.RemoveAt(index);
    }

    private void SetNames()
    {
        tempNames = new string[arraySizeField];

        for (int i = 0; i < Bones.Count; i++)
        {
            var boneData = Bones[i];
            tempNames[i] = boneData.BoneTransform.name;

            if (boneData.Name.Equals(string.Empty))
                boneData.BoneTransform.name = boneData.BoneTransform.name;
            else
            {
                boneData.BoneTransform.name = boneData.Name;
            }
        }
    }
    private void RevertLastNameChanges()
    {
        if (tempNames.Length.Equals(0)) return;

        for (int i = 0; i < Bones.Count; i++)
        {
            var boneData = Bones[i];

            boneData.BoneTransform.name = tempNames[i];
        }

        tempNames = new string[0];
    }

    private void AddConstraintsToBones()
    {
        tempBoneTransforms = new Transform[arraySizeField];

        for (int i = 0; i < Bones.Count; i++)
        {
            var boneData = Bones[i];

            //if (boneData.BoneTransform.GetComponent<CharacterJoint>() || !boneData.BoneTransform)
            //{
            //    tempBoneTransforms[i] = boneData.BoneTransform;
            //    break;
            //}

            var joint = boneData.BoneTransform.gameObject.AddComponent<CharacterJoint>();
            var collider = boneData.BoneTransform.gameObject.AddComponent<CapsuleCollider>();
            var rb = boneData.BoneTransform.GetComponent<Rigidbody>();

            collider.radius = 0.05f;
            collider.height = 0.1f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.mass = 2.5f;

            tempBoneTransforms[i] = boneData.BoneTransform;
        }
    }
    private void RevertConstraintAddition()
    {
        if (tempBoneTransforms.Length.Equals(0)) return;

        for (int i = 0; i < tempBoneTransforms.Length; i++)
        {
            DestroyImmediate(tempBoneTransforms[i].gameObject.GetComponent<CharacterJoint>());
            DestroyImmediate(tempBoneTransforms[i].gameObject.GetComponent<Collider>());
            DestroyImmediate(tempBoneTransforms[i].gameObject.GetComponent<Rigidbody>());
        }

        tempBoneTransforms = new Transform[0];
    }

    public List<Collider> tempSelectedColliders;
    public List<string> tempSelectedNames;
    public List<Component> tempComponents;
    public List<CharacterJoint> tempConstraints;

    private void CopyConstraintsData()
    {
        var selectedColliders = Selection.objects;
        tempConstraints.Clear();
        if (selectedColliders.Length.Equals(0)) return;

        for (int i = 0; i < selectedColliders.Length; i++)
        {
            var selectedObject = selectedColliders[i] as GameObject;
            var constraint = selectedObject.GetComponent<CharacterJoint>();

            tempConstraints.Add(constraint);
        }
    }
    private void PasteConstraintsData()
    {
        var selectedColliders = Selection.objects;

        SetConstraintsData(selectedColliders);

        tempConstraints.Clear();
    }
    private void SetConstraintsData(UnityEngine.Object[] objectArr)
    {
        for (int i = 0; i < objectArr.Length; i++)
        {
            var selObj = objectArr[i] as GameObject;
            var charJoint = selObj.GetComponent<CharacterJoint>();
            var tempJoint = tempConstraints[i];

            charJoint.highTwistLimit = tempJoint.highTwistLimit;
            charJoint.lowTwistLimit = tempJoint.lowTwistLimit;
            charJoint.swingLimitSpring = tempJoint.swingLimitSpring;
            charJoint.twistLimitSpring = tempJoint.twistLimitSpring;
            charJoint.swing1Limit = tempJoint.swing1Limit;
            charJoint.swing2Limit = tempJoint.swing2Limit;
        }
    }

    private void CopyColliderData()
    {
        var selectedColliders = Selection.objects;
        tempSelectedColliders.Clear();
        if (selectedColliders.Length.Equals(0)) return;

        for (int i = 0; i < selectedColliders.Length; i++)
        {
            var selectedObject = selectedColliders[i] as GameObject;
            var selectedCollider = selectedObject.GetComponent<Collider>();

            tempSelectedColliders.Add(selectedCollider);
        }
    }
    private void PasteColliderData()
    {
        var selectedColliders = Selection.objects;

        if (selectedColliders.Length.Equals(0)) return;

        SetColliderData(selectedColliders);

        tempSelectedColliders.Clear();
    }
    private void SetColliderData(UnityEngine.Object[] objectArr)
    {
        for (int i = 0; i < objectArr.Length; i++)
        {
            var selectedObject = objectArr[i] as GameObject;

            if (selectedObject.GetComponent<Collider>() is CapsuleCollider)
            {
                var selectedCollider = selectedObject.GetComponent<CapsuleCollider>();
                var tempCollider = tempSelectedColliders[i] as CapsuleCollider;

                selectedCollider.isTrigger = tempCollider.isTrigger;
                selectedCollider.material = tempCollider.material;
                selectedCollider.center = tempCollider.center;
                selectedCollider.radius = tempCollider.radius;
                selectedCollider.height = tempCollider.height;
                selectedCollider.direction = tempCollider.direction;
            }
            else if (selectedObject.GetComponent<Collider>() is BoxCollider)
            {
                var selectedCollider = selectedObject.GetComponent<BoxCollider>();
                var tempCollider = tempSelectedColliders[i] as BoxCollider;

                selectedCollider.isTrigger = tempCollider.isTrigger;
                selectedCollider.material = tempCollider.material;
                selectedCollider.center = tempCollider.center;
                selectedCollider.size = tempCollider.size;
            }
            else if (selectedObject.GetComponent<Collider>() is SphereCollider)
            {
                var selectedCollider = selectedObject.GetComponent<SphereCollider>();
                var tempCollider = tempSelectedColliders[i] as SphereCollider;

                selectedCollider.isTrigger = tempCollider.isTrigger;
                selectedCollider.material = tempCollider.material;
                selectedCollider.center = tempCollider.center;
                selectedCollider.radius = tempCollider.radius;
            }
        }

    }

    private void CopyObjectName()
    {
        var selectedObjects = Selection.objects;
        tempSelectedNames.Clear();
        if (selectedObjects.Length.Equals(0)) return;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            var selectedName = selectedObjects[i].name;

            tempSelectedNames.Add(selectedName);
        }
    }
    private void PasteObjectName()
    {
        var selectedObjects = Selection.objects;

        if (selectedObjects.Length.Equals(0)) return;

        for (int i = 0; i < tempSelectedNames.Count; i++)
        {
            var tempName = tempSelectedNames[i];
            selectedObjects[i].name = tempName;
        }

        tempSelectedNames.Clear();
    }

    private void CopyObjectsComponents()
    {
        tempComponents.Clear();

        var selectedObjects = Selection.objects;

        for (int i = 0; i < selectedObjects.Length; i++)
        {
            var obj = selectedObjects[i] as GameObject;
            var objComponents = obj.GetComponents<Component>();

            for (int z = 0; z < objComponents.Length; z++)
            {
                var objComp = objComponents[z];

                tempComponents.Add(objComp);
            }
        }
        tempComponents.RemoveAt(0);
    }
    private void PasteComponentsToObjects()
    {
        var selectedObjects = Selection.objects;

        for (int i = 0; i < tempComponents.Count; i++)
        {
            var selObj = selectedObjects[0] as GameObject;
            var tempComp = tempComponents[i];

            selObj.AddComponent(tempComp.GetType());
        }
    }
    private void ClearTempComponentsList()
    {
        tempComponents.Clear();
    }
}
