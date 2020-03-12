using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public class ConfigComponent : IComponent
{
    public SpriteConfigData spriteConfig;
    public AnimatorRunConfigData animatorRunConfig;
    public EffectConfigData effectConfig;
    
    public MapConfigData mapConfig;
}
