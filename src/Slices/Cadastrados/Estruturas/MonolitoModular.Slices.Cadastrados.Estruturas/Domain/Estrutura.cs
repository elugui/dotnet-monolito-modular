namespace MonolitoModular.Slices.Cadastrados.Estruturas.Domain;

public class Estrutura
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}