using UnityEngine;
using System.Collections.Generic;


namespace GameBerry.Common
{
	public static class ShaderHelper
	{
		static readonly MaterialPropertyBlock emissionPropertyBlock = new MaterialPropertyBlock();

		
		public static void SetupShader(GameObject go)
		{
			var renderers = go.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < renderers.Length; ++i)
			{
				var mats = renderers[i].sharedMaterials;
				for (int k = 0; k < mats.Length; ++k)
				{
					if (mats[k] == null)
						continue;

					if (mats[k].shader != null)
					{
//						Debug.LogFormat("SetupShader) GO: {0}, Mat: {1}, Shader: {2}", go.name, mats[k].name, mats[k].shader.name);
						mats[k].shader = Shader.Find(mats[k].shader.name);
					}
				}
			}
		}

		public static List<Material> GetSharedMaterials<T>(Transform tm) where T : Renderer
		{
			var retMats = new List<Material>();
			
			var renderers = tm.GetComponentsInChildren<T>();
			for (int i = 0; i < renderers.Length; ++i)
			{
				var mats = renderers[i].sharedMaterials;
				for (int k = 0; k < mats.Length; ++k)
				{
					if (mats[k] == null)
						continue;

					if (!retMats.Exists(x => x.Equals(mats[k])))
						retMats.Add(mats[k]);
				}
			}

			return retMats;
		}

		public static List<Material> GetMaterials<T>(Transform tm) where T : Renderer
		{
			var retMats = new List<Material>();

			var renderers = tm.GetComponentsInChildren<T>();
			for (int i = 0; i < renderers.Length; ++i)
			{
				var mats = renderers[i].materials;
				for (int k = 0; k < mats.Length; ++k)
				{
					if (mats[k] == null)
						continue;

					if (!retMats.Exists(x => x.Equals(mats[k])))
						retMats.Add(mats[k]);
				}
			}

			return retMats;
		}

		public static void SetupEmission(GameObject go)
		{
			var renderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			for (int i = 0; i < renderers.Length; ++i)
			{
				var mats = renderers[i].materials;
				for (int k = 0; k < mats.Length; ++k)
				{
					if (mats[k] == null)
						continue;

					mats[k].globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
					mats[k].EnableKeyword("_EMISSION");
				}
			}
		}

		public static void SetEmission(GameObject go, Color emissionColor)
		{
			emissionPropertyBlock.Clear();
			emissionPropertyBlock.SetColor("_EmissionColor", emissionColor);
			
			var renderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			for (int i = 0; i < renderers.Length; ++i)
			{
				var r = renderers[i];
				r.SetPropertyBlock(emissionPropertyBlock);
			}
		}

		public static void ResetEmission(GameObject go)
		{
			emissionPropertyBlock.Clear();
			
			var renderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			for (int i = 0; i < renderers.Length; ++i)
			{
				var r = renderers[i];
				r.SetPropertyBlock(emissionPropertyBlock);
			}
		}
	}
}
