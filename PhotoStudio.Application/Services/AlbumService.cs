using AutoMapper;
using PhotoStudio.Application.DTOs;
using PhotoStudio.Domain.Exceptions;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoStudio.Application.Services
{
    public class AlbumService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AlbumService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // 1. Kreiranje novog albuma
        public async Task<AlbumDTO> CreateAlbumAsync(AlbumDTO albumDto)
        {
            var album = _mapper.Map<Album>(albumDto);
            album.Code = GenerateUniqueCode();
            await _unitOfWork.Albums.AddAlbumAsync(album);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<AlbumDTO>(album);
        }

        // 2. Preuzimanje albuma pomoću koda
        public async Task<AlbumDTO> GetAlbumByCodeAsync(string code)
        {
            var album = await _unitOfWork.Albums.GetAlbumByCodeAsync(code);
            if (album == null)
            {
                throw new NotFoundException("Album not found.");
            }
            return _mapper.Map<AlbumDTO>(album);
        }

        // 3. Preuzimanje javnih albuma
        public async Task<IEnumerable<AlbumDTO>> GetPublicAlbumsAsync()
        {
            var albums = await _unitOfWork.Albums.GetPublicAlbumsAsync();
            return _mapper.Map<IEnumerable<AlbumDTO>>(albums);
        }

        // 4. Dodavanje medija u album
        public async Task AddMediaToAlbumAsync(string albumCode, MediaDTO mediaDto, int employeeId)
        {
            var album = await _unitOfWork.Albums.GetAlbumByCodeAsync(albumCode);
            if (album == null)
            {
                throw new NotFoundException("Album not found.");
            }

            // Provera vlasništva nad albumom (zaposleni koji dodaje medij mora biti vlasnik albuma)
            if (album.EmployeeId != employeeId)
            {
                throw new System.UnauthorizedAccessException("You are not authorized to add media to this album.");
            }

            var media = _mapper.Map<Media>(mediaDto);
            album.Media.Add(media);

            await _unitOfWork.CompleteAsync();
        }

        // 5. Generisanje jedinstvenog koda za album
        private string GenerateUniqueCode()
        {
            // Logika za generisanje jedinstvenog koda 
            return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }
    }
}
