namespace ProjectMER.Features.Components;

public interface IBlockComponent
{
    public ComponentData Compile();
    public void Decompile(ComponentData componentData);
}