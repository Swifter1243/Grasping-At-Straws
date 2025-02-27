using System;
using UnityEngine;

namespace SLC_GameJam_2025_1
{
    public class PuzzlePiece : PuzzleObject
    {
        public Transform m_input1;
        public Transform m_input2;
        public MeshRenderer m_meshRenderer;
        public Collider m_collider;

        private readonly static Color s_input1Color = new(0, 0.5f, 0.4f);
        private readonly static Color s_input2Color = new(1, 0.4f, 0);
        private readonly static int s_fluidProgress = Shader.PropertyToID("_FluidProgress");
        private readonly static int s_flipFluid = Shader.PropertyToID("_FlipFluid");
        private readonly static int s_opacity = Shader.PropertyToID("_Opacity");
        private readonly static int s_outlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int s_hovered = Shader.PropertyToID("_Hovered");

        private void OnDrawGizmos()
        {
            Debug.DrawLine(transform.position, transform.position + Input1DirectionOut, s_input1Color);
            Debug.DrawLine(transform.position, transform.position + Input2DirectionOut, s_input2Color);
        }

        public void SetFluidProgress(float fluidProgress)
        {
            m_meshRenderer.material.SetFloat(s_fluidProgress, fluidProgress);
        }

        public void SetFluidFlipped(bool fluidFlipped)
        {
            m_meshRenderer.material.SetInt(s_flipFluid, fluidFlipped ? 1 : 0);
        }

        public void SetOpacity(float opacity)
        {
            m_meshRenderer.material.SetFloat(s_opacity, opacity);
        }

        public void SetOutlineColor(Color outlineColor)
        {
            m_meshRenderer.material.SetColor(s_outlineColor, outlineColor);
        }

        public void SetHovered(bool hovered)
        {
            m_meshRenderer.material.SetInt(s_hovered, hovered ? 1 : 0);
        }

        public void SetInteractable(bool interactable)
        {
            m_collider.enabled = interactable;
        }

        public Vector3Int Input1DirectionOut => GetChildBoardDirection(m_input1);
        public Vector3Int Input1DirectionIn => GetChildBoardDirection(m_input1) * -1;
        public Vector3Int Input2DirectionOut => GetChildBoardDirection(m_input2);
        public Vector3Int Input2DirectionIn => GetChildBoardDirection(m_input2) * -1;

        public bool AcceptsDirection(Vector3Int direction) => direction == Input1DirectionIn || direction == Input2DirectionIn;

        public Vector3Int GetDirectionOut(Vector3Int directionIn, out bool usingInput1)
        {
            if (!AcceptsDirection(directionIn))
            {
                throw new ArgumentException("Invalid direction");
            }

            usingInput1 = directionIn == Input1DirectionIn;
            return usingInput1 ? Input2DirectionOut : Input1DirectionOut;
        }

        public void Select()
        {
            SetOutlineColor(new Color(0, 1, 1, 0.5f));
            SetInteractable(false);
        }

        public void Deselect()
        {
            SetOutlineColor(new Color(0, 1, 1, 0));
            SetInteractable(true);
        }
    }
}
