using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Chapter6
{
    public class RenderingDataManager : MonoBehaviour
    {
        public RenderTargetIdentifier BloomMarkTextureId { get; protected set; }
        public void SetBloomMarkTextureId(RenderTargetIdentifier id)
        {
            BloomMarkTextureId = id;
        }

        [SerializeField]
        private float bloomThreshold = 0.01f;
        public float BloomThreshold { get { return bloomThreshold; } protected set { bloomThreshold = value; } }
        public void SetBloomThreshold(float v)
        {
            BloomThreshold = v;
        }

        [SerializeField]
        private float bloomAmount = 1f;
        public float BloomAmount { get { return bloomAmount; } protected set { bloomAmount = value; } }
        public void SetBloomAmount(float v)
        {
            BloomAmount = v;
        }



        #region singleton
        private static RenderingDataManager instance;
        public static RenderingDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var previous = FindObjectOfType(typeof(RenderingDataManager));
                    if (previous)
                    {
                        Debug.LogWarning("Initialized twice. Don't use RenderingDataManager in the scene hierarchy.");
                        instance = (RenderingDataManager)previous;
                    }
                    else
                    {
                        var go = new GameObject("RenderingDataManager");
                        instance = go.AddComponent<RenderingDataManager>();
                        DontDestroyOnLoad(go);
                        //go.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
                return instance;
            }
        }
        #endregion
    }
}
