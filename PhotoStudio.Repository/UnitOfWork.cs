using Microsoft.EntityFrameworkCore;
using PhotoStudio.Domain.Interfaces;
using PhotoStudio.Infrastructure;
using PhotoStudio.Repository.Repositories;
using System;
using System.Threading.Tasks;

namespace PhotoStudio.Repository

{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PhotoStudioContext _context;

        public UnitOfWork(PhotoStudioContext context,
                          IUserRepository users,
                          IEmployeeRepository employees,
                          IMediaRepository media,
                          IPaymentRepository payments,
                          IAlbumRepository albums,
                          IBookingRepository bookings)
        {
            _context = context;
            Users = users;
            Employees = employees;
            Media = media;
            Payments = payments;
            Albums = albums;
            Bookings = bookings;
        }

        public IUserRepository Users { get; }
        public IMediaRepository Media {get; }
        public IEmployeeRepository Employees { get; }
        public IPaymentRepository Payments { get; }
     
        public IAlbumRepository Albums { get; }
      
        public IBookingRepository Bookings { get; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
