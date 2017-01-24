using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Piafs
{
   [CustomPropertyDrawer(typeof(EnvelopeModulator.EnvelopeCurve))]
    public class EnvelopeCurveDrawer : PropertyDrawer
    {
        static Texture2D line = new Texture2D(2, 2);


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height *= 0.5f;
            GUI.depth = 0;
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty curve = property.FindPropertyRelative("curve");
            SerializedProperty attackPoint = property.FindPropertyRelative("attackPoint");
            SerializedProperty releasePoint = property.FindPropertyRelative("releasePoint");
            SerializedProperty loop = property.FindPropertyRelative("loop");
            curve.animationCurveValue = EditorGUI.CurveField(position, curve.animationCurveValue, Color.cyan,Rect.zero);
            Keyframe[] k = curve.animationCurveValue.keys;
            EditorGUI.EndProperty();

            float min = 0f, max = 1f;
            attackPoint.intValue = Mathf.Clamp(attackPoint.intValue, 0, k.Length - 1);
            releasePoint.intValue = Mathf.Clamp(releasePoint.intValue, -k.Length + 1, 0);
            float envTime = 1f;
            if (k.Length > 0)
            {
                envTime = k[k.Length - 1].time;
                k[0] = new Keyframe(0f, 0f);
                k[k.Length - 1].value = 0f;
                curve.animationCurveValue = new AnimationCurve(k);
                min = k[attackPoint.intValue].time / envTime;
                max = k[k.Length - 1 + releasePoint.intValue].time / envTime;
            }
            position.y += position.height;
            position.height *= 0.2f;
            GUI.color = Color.cyan;
            EditorGUI.MinMaxSlider(position,ref min, ref max, 0f, 1f);
            for(int i = 0; i < k.Length - 2; i++)
            {
                if(min * envTime > k[i].time / envTime && min * envTime < k[i+1].time / envTime)
                {
                    if(min * envTime - k[i].time < k[i+1].time - min * envTime)
                    {
                        min = k[i].time / envTime;
                        attackPoint.intValue = i;
                    }
                    else
                    {
                        min = k[i+1].time / envTime;
                        attackPoint.intValue = i+1;
                    }
                }
                if (max * envTime > k[i].time / envTime && max * envTime < k[i + 1].time / envTime)
                {
                    if (max * envTime - k[i].time < k[i + 1].time - max * envTime)
                    {
                        max = k[i].time / envTime;
                        releasePoint.intValue = -(k.Length - 1 - i);
                    }
                    else
                    {
                        max = k[i + 1].time / envTime;
                        releasePoint.intValue = -(k.Length - 1 - (i+1));
                    }
                }
            }
            position.y += position.height;
            EditorGUI.PropertyField(position,attackPoint);
            position.y += position.height;
            EditorGUI.PropertyField(position, releasePoint);
            position.y += position.height;
            EditorGUI.PropertyField(position, loop);
            position.y += position.height;

            GUI.depth = 20;
            GUI.color = Color.white;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 12f;
        }
    } 
}


