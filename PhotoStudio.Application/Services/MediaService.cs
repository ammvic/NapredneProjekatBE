using AutoMapper;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoStudio.Application.Services
{
    public class MediaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MediaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddMediaAsync(MediaDTO mediaDto)
        {
            var media = _mapper.Map<Media>(mediaDto);
            await _unitOfWork.Media.AddMediaAsync(media);
        }

        public async Task<MediaDTO> GetMediaByIdAsync(int id)
        {
            var media = await _unitOfWork.Media.GetMediaByIdAsync(id);
            return _mapper.Map<MediaDTO>(media);
        }

        public async Task<IEnumerable<MediaDTO>> GetMediaByIdsAsync(IEnumerable<int> ids)
        {
            var mediaList = await _unitOfWork.Media.GetMediaByIdsAsync(ids);
            return _mapper.Map<IEnumerable<MediaDTO>>(mediaList);
        }

        public async Task<IEnumerable<MediaDTO>> GetMediaByAlbumIdAsync(int albumId)
        {
            var mediaList = await _unitOfWork.Media.GetMediaByAlbumIdAsync(albumId);
            return _mapper.Map<IEnumerable<MediaDTO>>(mediaList);
        }

        public async Task<IEnumerable<MediaDTO>> GetAllMediaAsync()
        {
            var mediaList = await _unitOfWork.Media.GetAllMediaAsync();
            return _mapper.Map<IEnumerable<MediaDTO>>(mediaList);
        }

        public async Task UpdateMediaAsync(MediaDTO mediaDto)
        {
            var media = _mapper.Map<Media>(mediaDto);
            await _unitOfWork.Media.UpdateMediaAsync(media);
        }

        public async Task DeleteMediaAsync(int id)
        {
            await _unitOfWork.Media.DeleteMediaAsync(id);
        }

        public async Task<IEnumerable<MediaDTO>> GetMediaWithEmployeeByAlbumIdAsync(int albumId)
        {
            var mediaList = await _unitOfWork.Media.GetMediaWithEmployeeByAlbumIdAsync(albumId);
            return _mapper.Map<IEnumerable<MediaDTO>>(mediaList);
        }
    }
}
