using UnityEngine;
using System.Collections.Generic;

namespace uLipSync
{

[ExecuteAlways]
public class uLipSyncBlendShape : AnimationBakableMonoBehaviour
{
    [System.Serializable]
    public class BlendShapeInfo
    {
        public string phoneme;
        public int index = -1;
        public float maxWeight = 1f;

        public float weight { get; set; } = 0f;
        public float weightVelocity { get; set; } = 0f;
    }

    public UpdateMethod updateMethod = UpdateMethod.LateUpdate;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public List<BlendShapeInfo> blendShapes = new List<BlendShapeInfo>();
    public float maxBlendShapeValue = 100f;
    public float minVolume = -2.5f;
    public float maxVolume = -1.5f;
    [Range(0f, 0.3f)] public float smoothness = 0.05f;
    public bool usePhonemeBlend = false;

    LipSyncInfo _info = new LipSyncInfo();
    bool _lipSyncUpdated = false;
    float _volume = 0f;
    float _openCloseVelocity = 0f;
    protected float volume => _volume;

#if UNITY_EDITOR
    bool _isAnimationBaking = false;
    float _animBakeDeltaTime = 1f / 60;
#endif

    void UpdateLipSync()
    {
        UpdateVolume();
        UpdateVowels();
        _lipSyncUpdated = false;
    }

    public void OnLipSyncUpdate(LipSyncInfo info)
    {
        _info = info;
        _lipSyncUpdated = true;
        if (updateMethod == UpdateMethod.LipSyncUpdateEvent)
        {
            UpdateLipSync();
            OnApplyBlendShapes();
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (_isAnimationBaking) return;
#endif
        if (updateMethod != UpdateMethod.LipSyncUpdateEvent)
        {
            UpdateLipSync();
        }

        if (updateMethod == UpdateMethod.Update)
        {
            OnApplyBlendShapes();
        }
    }

    void LateUpdate()
    {
#if UNITY_EDITOR
        if (_isAnimationBaking) return;
#endif
        if (updateMethod == UpdateMethod.LateUpdate)
        {
            OnApplyBlendShapes();
        }
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR
        if (_isAnimationBaking) return;
#endif
        if (updateMethod == UpdateMethod.FixedUpdate)
        {
            OnApplyBlendShapes();
        }
    }

		/*
    float SmoothDamp(float value, float target, ref float velocity)
    {
#if UNITY_EDITOR
        if (_isAnimationBaking)
        {
            return Mathf.SmoothDamp(value, target, ref velocity, smoothness, Mathf.Infinity, _animBakeDeltaTime);
        }
#endif
        return Mathf.SmoothDamp(value, target, ref velocity, smoothness);
    }
        */

		float SmoothDamp(float value, float target, ref float velocity)
		{
#if UNITY_EDITOR
			if (_isAnimationBaking)
			{
				return Mathf.SmoothDamp(value, target, ref velocity, smoothness, Mathf.Infinity, _animBakeDeltaTime);
			}
#endif
			// Защита от нулевой плавности, чтобы избежать деления на ноль
			float smoothTime = Mathf.Max(0.0001f, smoothness);

			float omega = 2f / smoothTime;
			float x = omega * Time.unscaledDeltaTime;
			float exp = 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);

			float temp = (value - target);
			float to = target;
			float maxChange = Mathf.Infinity;
			to = (temp > 0) ? Mathf.Min(to, target + maxChange) : Mathf.Max(to, target - maxChange);

			if (Mathf.Abs(to - value) > 0.0001f)
			{
				float result = value + (to - value) * exp;
				velocity = (result - value) / Time.unscaledDeltaTime;
				return result;
			}
			else
			{
				velocity = 0.0f;
				return target;
			}
		}

		/*
		void UpdateVolume()
    {
        float normVol = 0f;
        if (_lipSyncUpdated && _info.rawVolume > 0f)
        {
            normVol = Mathf.Log10(_info.rawVolume);
            normVol = (normVol - minVolume) / Mathf.Max(maxVolume - minVolume, 1e-4f);
            normVol = Mathf.Clamp(normVol, 0f, 1f);
        }
        _volume = SmoothDamp(_volume, normVol, ref _openCloseVelocity);
    }
        */
		void UpdateVolume()
		{
			float normVol = 0f;
			if (_lipSyncUpdated && _info.rawVolume > 0f)
			{
				normVol = Mathf.Log10(_info.rawVolume);
				normVol = (normVol - minVolume) / Mathf.Max(maxVolume - minVolume, 1e-4f);
				normVol = Mathf.Clamp01(normVol);
			}
			_volume = SmoothDamp(_volume, normVol, ref _openCloseVelocity);
		}

		void UpdateVowels()
    {
        float sum = 0f;
        var ratios = _info.phonemeRatios;

        foreach (var bs in blendShapes)
        {
            float targetWeight = 0f;
            if (usePhonemeBlend)
            {
                if (ratios != null && !string.IsNullOrEmpty(bs.phoneme))
                {
                    ratios.TryGetValue(bs.phoneme, out targetWeight);
                }
            }
            else
            {
                targetWeight = (bs.phoneme == _info.phoneme) ? 1f : 0f;
            }
            float weightVel = bs.weightVelocity;
            bs.weight = SmoothDamp(bs.weight, targetWeight, ref weightVel);
            bs.weightVelocity = weightVel;
            sum += bs.weight;
        }

        foreach (var bs in blendShapes)
        {
            bs.weight = sum > 0f ? bs.weight / sum : 0f;
        }
    }

    public void ApplyBlendShapes()
    {
        if (updateMethod == UpdateMethod.External)
        {
            OnApplyBlendShapes();
        }
    }

		/*
		protected virtual void OnApplyBlendShapes()
		{
			if (!skinnedMeshRenderer) return;

			foreach (var bs in blendShapes)
			{
				if (bs.index < 0) continue;
				skinnedMeshRenderer.SetBlendShapeWeight(bs.index, 0f);
			}

			foreach (var bs in blendShapes)
			{
				if (bs.index < 0) continue;
				float weight = skinnedMeshRenderer.GetBlendShapeWeight(bs.index);
				weight += bs.weight * bs.maxWeight * volume * maxBlendShapeValue;
				skinnedMeshRenderer.SetBlendShapeWeight(bs.index, weight);
			}
		}
		*/
		protected virtual void OnApplyBlendShapes()
		{
			if (!skinnedMeshRenderer) return;

			foreach (var bs in blendShapes)
			{
				if (bs.index < 0) continue;
				skinnedMeshRenderer.SetBlendShapeWeight(bs.index, 0f);
			}

			foreach (var bs in blendShapes)
			{
				if (bs.index < 0) continue;
				float weight = bs.weight * bs.maxWeight * volume * maxBlendShapeValue;
				skinnedMeshRenderer.SetBlendShapeWeight(bs.index, weight);
			}
		}


		public BlendShapeInfo GetBlendShapeInfo(string phoneme)
    {
    foreach (var info in blendShapes)
    {
        if (info.phoneme == phoneme) return info;
    }
    return null;
    }

    public BlendShapeInfo AddBlendShape(string phoneme, string blendShape)
    {
        var bs = GetBlendShapeInfo(phoneme);
        if (bs == null) bs = new BlendShapeInfo() { phoneme = phoneme };

        blendShapes.Add(bs);

        if (!skinnedMeshRenderer) return bs;
        bs.index = Util.GetBlendShapeIndex(skinnedMeshRenderer, blendShape);

        return bs;
    }

#if UNITY_EDITOR
    public override GameObject target => skinnedMeshRenderer?.gameObject;

    public override List<string> GetPropertyNames()
    {
        var names = new List<string>();
        var mesh = skinnedMeshRenderer.sharedMesh;

        foreach (var bs in blendShapes)
        {
            if (bs.index < 0) continue;
            var name = mesh.GetBlendShapeName(bs.index);
            name = "blendShape." + name;
            names.Add(name);
        }

        return names;
    }

    public override List<float> GetPropertyWeights()
    {
        var weights = new List<float>();

        foreach (var bs in blendShapes)
        {
            if (bs.index < 0) continue;
            var weight = bs.weight * bs.maxWeight * volume * maxBlendShapeValue;
            weights.Add(weight);
        }

        return weights;
    }

    public override float maxWeight => 100f;
    public override float minWeight => 0f;

    public override void OnAnimationBakeStart()
    {
        _isAnimationBaking = true;
    }

    public override void OnAnimationBakeUpdate(LipSyncInfo info, float dt)
    {
        _info = info;
        _animBakeDeltaTime = dt;
        _lipSyncUpdated = true;
        UpdateLipSync();
    }

    public override void OnAnimationBakeEnd()
    {
        _isAnimationBaking = false;
    }
#endif
}

}

