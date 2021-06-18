using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GameBerry.UI
{
    [CustomEditor(typeof(IDialogAnimation), true)]
    public class IDialogAnimation_Editor : UnityEditor.Editor
    {
        IDialogAnimation iDialogAnimation { get { return (IDialogAnimation)target; } }

        private float SmallSpace = 5.0f;
        private float LargeSpace = 10.0f;

        private int Indentation = 1;

        //------------------------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            iDialogAnimation.m_AnimationTarget = (Transform)EditorGUILayout.ObjectField("AnimationTarget", iDialogAnimation.m_AnimationTarget, typeof(Transform), true);

            DrawInAnimation();
            GUILayout.Space(LargeSpace);
            DrawOutAnimation();

            if (Application.isPlaying == true)
            {
                if (GUILayout.Button("PlayInAnimaion"))
                {
                    iDialogAnimation.PlayInAnimation();
                }

                if (GUILayout.Button("PlayOutAnimaion"))
                {
                    iDialogAnimation.PlayOutAnimation();
                }
            }

            DrawDefaultInspector();
        }
        //------------------------------------------------------------------------------------
        private void DrawInAnimation()
        {
            iDialogAnimation.m_useInAnimation = EditorGUILayout.BeginToggleGroup("InAnimation", iDialogAnimation.m_useInAnimation);

            if (iDialogAnimation.m_useInAnimation == true)
            {
                DrawMoveEditor(iDialogAnimation.m_InAnimation.m_MoveAni);
                GUILayout.Space(SmallSpace);
                DrawRotateEditor(iDialogAnimation.m_InAnimation.m_RotateAni);
                GUILayout.Space(SmallSpace);
                DrawScaleEditor(iDialogAnimation.m_InAnimation.m_ScaleAni);
                GUILayout.Space(SmallSpace);
                DrawFadeEditor(iDialogAnimation.m_InAnimation.m_FadeAni);
            }

            EditorGUILayout.EndToggleGroup();
        }
        //------------------------------------------------------------------------------------
        private void DrawOutAnimation()
        {
            iDialogAnimation.m_useOutAnimation = EditorGUILayout.BeginToggleGroup("OutAnimation", iDialogAnimation.m_useOutAnimation);

            if (iDialogAnimation.m_useOutAnimation == true)
            {
                DrawMoveEditor(iDialogAnimation.m_OutAnimation.m_MoveAni);
                GUILayout.Space(SmallSpace);
                DrawRotateEditor(iDialogAnimation.m_OutAnimation.m_RotateAni);
                GUILayout.Space(SmallSpace);
                DrawScaleEditor(iDialogAnimation.m_OutAnimation.m_ScaleAni);
                GUILayout.Space(SmallSpace);
                DrawFadeEditor(iDialogAnimation.m_OutAnimation.m_FadeAni);
            }

            EditorGUILayout.EndToggleGroup();
        }
        //------------------------------------------------------------------------------------
        private void DrawMoveEditor(MoveAniStruct animation)
        {
            animation.m_useAnimation = EditorGUILayout.BeginToggleGroup("MoveAnimation", animation.m_useAnimation);

            if (animation.m_useAnimation == true)
            {
                EditorGUI.indentLevel += Indentation;

                DrawBaseStructEditor(animation);

                GUILayout.Width(SmallSpace);
                animation.m_MoveFrom = (MoveAniStruct.MoveDirection)EditorGUILayout.EnumPopup("MoveFrom", animation.m_MoveFrom);

                if(animation.m_MoveFrom == MoveAniStruct.MoveDirection.CustomPosition)
                    animation.m_CustomPosition = EditorGUILayout.Vector3Field("CustomPosition", animation.m_CustomPosition);

                EditorGUI.indentLevel -= Indentation;
            }

            EditorGUILayout.EndToggleGroup();
        }
        //------------------------------------------------------------------------------------
        private void DrawRotateEditor(RotateAniStruct animation)
        {
            animation.m_useAnimation = EditorGUILayout.BeginToggleGroup("RotateAnimation", animation.m_useAnimation);

            if (animation.m_useAnimation == true)
            {
                EditorGUI.indentLevel += Indentation;

                DrawBaseStructEditor(animation);

                animation.m_Rotate = EditorGUILayout.Vector3Field("Rotate", animation.m_Rotate);

                EditorGUI.indentLevel -= Indentation;
            }

            EditorGUILayout.EndToggleGroup();
        }
        //------------------------------------------------------------------------------------
        private void DrawScaleEditor(ScaleAniStruct animation)
        {
            animation.m_useAnimation = EditorGUILayout.BeginToggleGroup("ScaleAnimation", animation.m_useAnimation);

            if (animation.m_useAnimation == true)
            {
                EditorGUI.indentLevel += Indentation;

                DrawBaseStructEditor(animation);

                animation.m_Scale = EditorGUILayout.Vector3Field("Scale", animation.m_Scale);

                EditorGUI.indentLevel -= Indentation;
            }

            EditorGUILayout.EndToggleGroup();
        }
        //------------------------------------------------------------------------------------
        private void DrawFadeEditor(FadeAniStruct animation)
        {
            animation.m_useAnimation = EditorGUILayout.BeginToggleGroup("FadeAnimation", animation.m_useAnimation);

            if (animation.m_useAnimation == true)
            {
                EditorGUI.indentLevel += Indentation;

                DrawBaseStructEditor(animation);

                animation.m_StartAlpha = EditorGUILayout.Slider("StartAlpha", animation.m_StartAlpha, 0.0f, 1.0f);
                animation.m_EndAlpha = EditorGUILayout.Slider("EndAlpha", animation.m_EndAlpha, 0.0f, 1.0f);

                EditorGUI.indentLevel -= Indentation;
            }

            EditorGUILayout.EndToggleGroup();
        }
        //------------------------------------------------------------------------------------
        private void DrawBaseStructEditor(BaseAnimationStruct animation)
        {
            animation.m_StartDelay = EditorGUILayout.FloatField("StartDelay", animation.m_StartDelay);
            animation.m_Duration = EditorGUILayout.FloatField("Duration", animation.m_Duration);

            animation.m_Linear = EditorGUILayout.BeginToggleGroup("Linear", animation.m_Linear);
            EditorGUILayout.EndToggleGroup();

            if (animation.m_Linear == false)
                animation.m_AnimationCurve = EditorGUILayout.CurveField("AnimationCurve", animation.m_AnimationCurve);
        }
        //------------------------------------------------------------------------------------
    }
}