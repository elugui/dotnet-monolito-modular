using FluentValidation;

namespace MonolitoModular.Slices.Cadastrados.Estruturas.Features.CreateEstrutura;

public class CreateEstruturaValidator : AbstractValidator<CreateEstruturaCommand>
{
    public CreateEstruturaValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200);

        RuleFor(x => x.Descricao)
            .MaximumLength(500);
    }
}