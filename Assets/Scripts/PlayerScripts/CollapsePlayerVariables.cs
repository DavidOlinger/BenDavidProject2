using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerScript))]
public class CollapsePlayerVariables : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerScript playerScript = (PlayerScript)target;

        // Movement Settings
        playerScript.showMovementSettings = EditorGUILayout.Foldout(playerScript.showMovementSettings, "Movement Settings");
        if (playerScript.showMovementSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("momentLock"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("normalMoveSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wallJumpXForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wallJumpYForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("groundingCountDown"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cantMove"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wallVaultRaycastDistance"));
        }

        // Camera Settings
        playerScript.showCameraSettings = EditorGUILayout.Foldout(playerScript.showCameraSettings, "Camera Settings");
        if (playerScript.showCameraSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cam"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("updateCamera"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookingRight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLookAhead"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookTurnDampening"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerLookAhead"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lookDown"));
        }

        // Slash Settings
        playerScript.showSlashSettings = EditorGUILayout.Foldout(playerScript.showSlashSettings, "Slash Settings");
        if (playerScript.showSlashSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("downSlashPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("upSlashPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("vaultPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("heavySlashPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxSlashCoolDown"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackOnCooldown"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isHeavySlashing"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("heavySlashOnCooldown"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHeavySlashCooldown"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackQueued"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nextSwingIsDownward"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("slashOffX"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("slashOffY"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("knockbackOnHit"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stunOnHit"));
        }

        // Health Settings
        playerScript.showHealthSettings = EditorGUILayout.Foldout(playerScript.showHealthSettings, "Health Settings");
        if (playerScript.showHealthSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currHP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxHP"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("invincible"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("pickupRadius"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lastCampfire"));
        }

        // Sound Settings
        playerScript.showSoundSettings = EditorGUILayout.Foldout(playerScript.showSoundSettings, "Sound Settings");
        if (playerScript.showSoundSettings)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("footstepSounds"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackHitSounds"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("swingSounds"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpLaunchSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpLandSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("vaultSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hpShatterSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerGotHitSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("saveSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moneySound"));
        }

        // Miscellaneous Settings
        playerScript.showMiscellaneousSettings = EditorGUILayout.Foldout(playerScript.showMiscellaneousSettings, "Mischellaneous Settings");
        if (playerScript.showMiscellaneousSettings)
        {
           // EditorGUILayout.LabelField("Miscellaneous Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("boomerang"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("boomerangCooldown"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("grav"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxGravSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("moveDecayCoef"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("isInteracting"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpDust"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vaultDust"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("launchCloud"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("interactFlash"));
        }

        // Apply changes
        serializedObject.ApplyModifiedProperties();
    }
}
