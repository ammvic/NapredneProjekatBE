using System;
using System.Threading.Tasks;

namespace PhotoStudio.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IEmployeeRepository Employees { get; }
        IMediaRepository Media { get; }
        IPaymentRepository Payments { get; }
        IAlbumRepository Albums { get; }
        IBookingRepository Bookings { get; }

        Task<int> CompleteAsync();
    }
}
