using DevXpert.Academy.Alunos.Domain.Alunos.Interfaces;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.Exceptions;
using DevXpert.Academy.Core.Domain.Messages.CommonMessages.Notifications;
using DevXpert.Academy.Core.Domain.Services;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevXpert.Academy.Alunos.Domain.Alunos.Services
{
    public class AlunoService : DomainService, IAlunoService
    {
        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(
            IAlunoRepository alunoRepository,
            IMediatorHandler mediator,
            INotificationHandler<DomainNotification> notifications)
            : base(alunoRepository.UnitOfWork, mediator, notifications)
        {
            _alunoRepository = alunoRepository;
        }

        public async Task<bool> Cadastrar(Aluno aluno)
        {
            if (!EntidadeValida(aluno))
                return false;

            await _alunoRepository.Cadastrar(aluno);

            return await CommitAsync();
        }
        public async Task<bool> ExcluirNovoCadastro(Guid id)
        {
            await _alunoRepository.Excluir(id);

            return await CommitAsync(ignoreNoChangeUpdated: true);
        }

        public async Task<Matricula> Matricular(Guid alunoId, Guid cursoId)
        {
            var aluno = await _alunoRepository.ObterPorId(alunoId, true) ?? throw new BusinessException("Aluno não encontrado.");

            if (aluno.EstaMatriculado(cursoId))
            {
                var matriculado = aluno.Matriculas.FirstOrDefault(p => p.CursoId == cursoId);
                if (matriculado.Ativa)
                    return matriculado;
                else
                    throw new BusinessException("O aluno já está matriculado neste curso, favor realizar o pagamento.");
            }

            var matricula = new Matricula(Guid.NewGuid(), aluno.Id, cursoId);

            aluno.Matricular(matricula);

            if (!EntidadeValida(aluno))
                return null;

            if (await CommitAsync())
                return matricula;

            return null;
        }
        public async Task<bool> AprovarMatricula(Guid matriculaId)
        {
            var aluno = await _alunoRepository.ObterAtravesDaMatricula(matriculaId, true) ?? throw new BusinessException("Aluno não encontrado.");

            var matricula = aluno.Matriculas.FirstOrDefault(p => p.Id == matriculaId) ?? throw new BusinessException("Matrícula não encontrada.");

            if (matricula.Ativa)
                return true;

            matricula.Ativar();

            if (!EntidadeValida(aluno))
                return false;

            return await CommitAsync();
        }
        public async Task<bool> BloquearMatricula(Guid matriculaId)
        {
            var aluno = await _alunoRepository.ObterAtravesDaMatricula(matriculaId, true) ?? throw new BusinessException("Aluno não encontrado.");

            var matricula = aluno.Matriculas.FirstOrDefault(p => p.Id == matriculaId) ?? throw new BusinessException("Matrícula não encontrada.");

            if (!matricula.Ativa)
                return true;

            matricula.Bloquear();

            if (!EntidadeValida(aluno))
                return false;

            return await CommitAsync();
        }

        public async Task<bool> RegistrarAulaConcluida(Guid matriculaId, Guid aulaId)
        {
            var aluno = await _alunoRepository.ObterAtravesDaMatricula(matriculaId, true) ?? throw new BusinessException("Aluno não encontrado.");

            var matricula = aluno.Matriculas.FirstOrDefault(p => p.Id == matriculaId) ?? throw new BusinessException("Matrícula não encontrada.");

            if (!matricula.Ativa)
                throw new BusinessException("A matrícula não está ativa, não é possível registrar a conclusão da aula.");

            aluno.RegistrarAulaAssistida(matricula.CursoId, aulaId);

            if (!EntidadeValida(aluno))
                return false;

            return await CommitAsync(ignoreNoChangeUpdated: true);
        }

        public async Task<string> EmitirCertificado(Guid matriculaId)
        {
            var aluno = await _alunoRepository.ObterAtravesDaMatricula(matriculaId, true) ?? throw new BusinessException("Aluno não encontrado.");

            var matricula = aluno.Matriculas.FirstOrDefault(p => p.Id == matriculaId) ?? throw new BusinessException("Matrícula não encontrada.");

            if (!matricula.Ativa)
                throw new BusinessException("A matrícula não está ativa, não é possível registrar a conclusão da aula.");

            matricula.EmitirCertificado();

            if (!EntidadeValida(aluno))
                throw new BusinessException(string.Join('.', aluno.ValidationResult.Errors.Select(p => p.ErrorMessage)));

            if (await CommitAsync())
                return matricula.Certificado.CertificadoUrl;

            throw new BusinessException("Não foi possível emitir o certificado, tente novamente mais tarde ou contate os administradores da plataforma.");
        }
    }
}
