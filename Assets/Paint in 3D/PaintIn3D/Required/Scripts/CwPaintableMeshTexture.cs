﻿using UnityEngine;
using PaintCore;
using System.Collections.Generic;

namespace PaintIn3D
{
	/// <summary>This component allows you to make one texture on the attached Renderer paintable.
	/// NOTE: If the texture or texture slot you want to paint is part of a shared material (e.g. prefab material), then I recommend you add the CwMaterialCloner component to make it unique.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwPaintableMeshTexture")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Paintable Mesh Texture")]
	public class CwPaintableMeshTexture : CwPaintableTexture
	{
		[System.NonSerialized]
		private CwPaintableMesh parent;

		protected override void ApplyTexture(Texture texture)
		{
			if (parent == null)
			{
				parent = GetComponentInParent<CwPaintableMesh>();
			}

			if (parent != null)
			{
				Debug.Log("texture:" + texture);
				parent.ApplyTexture(Slot, texture);
				Debug.Log("parent OhterRenderers: "+ parent + " ㅇㅇㅇ "+ parent.OtherRenderers);
				foreach (var otherRenderer in parent.OtherRenderers)
				{
					if (otherRenderer != null)
					{
						parent.ApplyTexture(otherRenderer, Slot, texture);
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	using CW.Common;
	using UnityEditor;
	using TARGET = CwPaintableMeshTexture;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwPaintableMeshTexture_Editor : CwPaintableTexture_Editor
	{
	}
}
#endif