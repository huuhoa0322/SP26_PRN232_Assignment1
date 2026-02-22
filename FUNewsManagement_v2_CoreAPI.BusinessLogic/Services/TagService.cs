using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.Services.Interfaces;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.DataAccess.Repositories.Interfaces;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;

        public TagService(ITagRepository tagRepository, IMapper mapper, IAuditLogService auditLogService)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _auditLogService = auditLogService;
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TagDto>>(tags);
        }

        public async Task<TagDto?> GetByIdAsync(int id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            return tag == null ? null : _mapper.Map<TagDto>(tag);
        }

        public async Task<TagDto> CreateAsync(CreateTagRequest request)
        {
            // Check duplicate name
            var existing = await _tagRepository.GetByNameAsync(request.TagName);
            if (existing != null)
            {
                throw new InvalidOperationException($"Tag '{request.TagName}' đã tồn tại.");
            }

            var tag = _mapper.Map<Tag>(request);
            
            // Generate ID manually since DB is not identity
            var allTags = await _tagRepository.GetAllAsync();
            int newId = allTags.Any() ? allTags.Max(t => t.TagId) + 1 : 1;
            tag.TagId = newId;

            await _tagRepository.AddAsync(tag);
            var resultDto = _mapper.Map<TagDto>(tag);

            await _auditLogService.LogActionAsync("Create", "Tag", tag.TagId.ToString(), null, resultDto);

            return resultDto;
        }

        public async Task<TagDto?> UpdateAsync(int id, UpdateTagRequest request)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return null;

            var oldData = _mapper.Map<TagDto>(tag);

            // Check duplicate name if changed
            if (!string.IsNullOrEmpty(request.TagName) && request.TagName != tag.TagName)
            {
                var existing = await _tagRepository.GetByNameAsync(request.TagName);
                if (existing != null)
                {
                    throw new InvalidOperationException($"Tag '{request.TagName}' đã tồn tại.");
                }
            }

            _mapper.Map(request, tag);
            await _tagRepository.UpdateAsync(tag);
            var newData = _mapper.Map<TagDto>(tag);

            await _auditLogService.LogActionAsync("Update", "Tag", id.ToString(), oldData, newData);

            return newData;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var isUsed = await _tagRepository.IsTagUsedAsync(id);
            if (isUsed)
            {
                throw new InvalidOperationException("Không thể xóa Tag này vì đang được sử dụng trong bài viết.");
            }

            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null) return false;
            
            var oldData = _mapper.Map<TagDto>(tag);
            await _tagRepository.DeleteAsync(tag);
            
            await _auditLogService.LogActionAsync("Delete", "Tag", id.ToString(), oldData, null);

            return true;
        }
    }
}
