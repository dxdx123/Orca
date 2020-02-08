using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public class ConfigComponent : IComponent
{
    public SpriteConfigData spriteConfig;
    public MapConfigData mapConfig;
}
