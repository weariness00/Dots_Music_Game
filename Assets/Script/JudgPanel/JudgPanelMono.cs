using Unity.Entities;
using UnityEngine;

namespace Script.JudgPanel
{
    [System.Serializable]
    public enum JudgPanelType
    {
        Pistol,
        Rifle,
        Sniper,
    }
    
    public class JudgPanelMono : MonoBehaviour
    {
        public JudgPanelType type;
    }

    public class JudgPanelBaker : Baker<JudgPanelMono>
    {
        public override void Bake(JudgPanelMono authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            switch (authoring.type)
            {
                case JudgPanelType.Pistol:
                    AddComponent<PistolPanelTag>(entity);
                    break;
                case JudgPanelType.Rifle:
                    AddComponent<RiflePanelTag>(entity);
                    break;
                case JudgPanelType.Sniper:
                    AddComponent<SniperPanelTag>(entity);
                    break;
            }
        }
    }
}
