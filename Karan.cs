using CG1.Application.DTO;
using CG1.Application.Interfaces;
using CG1.Domain.Entities.HRMS;
using System.Collections.Concurrent;

namespace CG1.Application.Services;

public class SkillService : ISkillService
{
    private readonly IUnitOfWork _unitOfWork;
    private static readonly ConcurrentDictionary<string, int> _statusIdCache = new();

    public SkillService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Skill>> GetAllSkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SkillRepository.GetAllAsync(cancellationToken);
    }

    public async Task<IEnumerable<object>> GetAllPrimarySkillsAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SkillRepository.GetAllPrimarySkillsAsync(cancellationToken);
    }

    public async Task<Skill> GetSkillByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SkillRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task AddSkillAsync(Skill skill, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.SkillRepository.AddAsync(skill, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSkillAsync(Skill skill, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.SkillRepository.UpdateAsync(skill, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteSkillAsync(int id, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.SkillRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    //NEW Services
    public async Task<IReadOnlyList<SkillManagementDto>> GetSkillsByStatusAsync(
        string? status,
        CancellationToken cancellationToken = default)
    {
        int? statusId = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            var statusEntity = await _unitOfWork.StatusRepository
                .GetByNameAsync(status, cancellationToken);

            if (statusEntity == null)
            {
                throw new KeyNotFoundException(
                    $"Status '{status}' not found.");
            }

            statusId = statusEntity.Id;
        }

        return await _unitOfWork.SkillRepository
            .GetSkillsByStatusAsync(statusId, cancellationToken);
    }

    public async Task<SkillManagementDto?> GetSkillForManagementAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.SkillRepository
            .GetSkillForManagementAsync(id, cancellationToken);
    }

    public async Task CreateSkillForManagementAsync(
        string description,
        string? createdBy,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            var activeStatusId = await GetStatusIdOrThrowAsync("Active", cancellationToken);

            var skill = new Skill
            {
                Description = description,
                StatusId = activeStatusId,
                CreatedBy = createdBy,
                ModifiedBy = createdBy,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };

            await _unitOfWork.SkillRepository
                .CreateSkillForManagementAsync(skill, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }

    public async Task UpdateSkillForManagementAsync(
        int id,
        string? description,
        string? status,
        string? modifiedBy,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            var skill = await GetSkillEntityOrThrowAsync(id, cancellationToken);

            if (!string.IsNullOrWhiteSpace(description))
            {
                skill.Description = description;
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var statusId = await GetStatusIdOrThrowAsync(status, cancellationToken);
                skill.StatusId = statusId;
            }

            skill.ModifiedBy = modifiedBy;
            skill.DateModified = DateTime.UtcNow;

            await _unitOfWork.SkillRepository
                .UpdateSkillForManagementAsync(skill, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }

    // Soft delete is being handled directly in the service layer as this is pure business logic
    public async Task SoftDeleteSkillAsync(
        int id,
        string? modified