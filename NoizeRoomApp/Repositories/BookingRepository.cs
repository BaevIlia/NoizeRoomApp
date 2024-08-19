using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using NoizeRoomApp.Abstractions;
using NoizeRoomApp.Database;
using NoizeRoomApp.Database.Models;
using NoizeRoomApp.Dtos;

namespace NoizeRoomApp.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly PostgreSQLContext _context;

        public BookingRepository(PostgreSQLContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteBooking(Guid id)
        {
            var bookingForDelete = _context.Bookings.FirstOrDefault(booking => booking.Id == id);
            if (bookingForDelete != null && bookingForDelete.GetType() == typeof(BookingEntity))
            {
                _context.Bookings.Remove(bookingForDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            else
                return false;

            
               
        }

        public async Task<string> GetBookerName(Guid id)
        {
            var result = await _context.Users.Where(u => u.Id.Equals(id)).Select(u => u.Name).FirstOrDefaultAsync();

            if (result is not null && result.GetType() == typeof(string))
                return result;
            else
                return string.Empty;
        }

        public async Task<BookingEntity> GetBooking(Guid id) 
        {
            var result = await _context.Bookings.FindAsync(id);
            if (result is not null && result.GetType() == typeof(BookingEntity))
                return result;
            else
                return null;
            
        }

        public List<IsBookedDto> GetBookingsByDate(List<DateTime> dates)
        {
            
            List<IsBookedDto> result = new();
            foreach (var date in dates) 
            {
                if (_context.Bookings.Any(b => b.Date.Equals(date))) 
                {
                    result.Add(new IsBookedDto(date, true));
                }
                else
                {
                    result.Add(new IsBookedDto(date, false));
                }
            }
            return result;
        }

        public async Task<List<BookingDto>> GetBooksByDay(Guid userId, DateTime date)
        {
            UserEntity currentUser = await _context.Users.FindAsync(userId);
            List<BookingDto> result = new();
            if (currentUser is null)
                return null;
            if (currentUser.RoleId == 1) 
            {
                result = (from books in _context.Bookings
                           .Where(b => b.Date.Day.Equals(date.Day))
                          select new BookingDto(books.Date, books.TimeFrom, books.TimeTo, books.BookerName)).ToList();

            }
            else if(currentUser.RoleId == 2)
            {
                result = (from books in _context.Bookings
                         .Where(b => b.Date.Day.Equals(date.Day) && b.BookerId.Equals(currentUser.Id))
                            select new BookingDto(books.Date, books.TimeFrom, books.TimeTo, books.BookerName)).ToList();
            }
            return result;
        }

        public List<StatisticDto> GetStatistic(List<DateTime> dates)
        {
            List<StatisticDto> result = new();

            foreach (var date in dates) 
            {
                result.Add(new StatisticDto(date, _context.Bookings.Where(b => b.Date.Equals(date)).Count()));
            }
            return result;
        }

        public async Task<bool> CreateBook(Guid bookerId, BookingEntity newBooking)
        {
           
            string? bookerName = await _context.Users
                .Where(u => u.Id.Equals(bookerId))
                .Select(u => u.Name)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(bookerName))
                return false;

            try
            {
                //Создание брони
                BookingEntity newBook = new()
                {
                    Id = Guid.NewGuid(),
                    BookerId = bookerId,
                    BookerName = bookerName,
                    Date = newBooking.Date,
                    TimeFrom = newBooking.TimeFrom,
                    TimeTo = newBooking.TimeTo,
                };

                if (newBook is null)
                {
                    return false;
                }
               
                _context.Bookings.Add(newBook);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateBooking(Guid bookingId,BookingDto bookingUpdateData)
        {
            var bookingForUpdate = await _context.Bookings.FindAsync(bookingId);

            if(bookingForUpdate is null)
                return false;

            bookingForUpdate.Date = bookingUpdateData.Date;
            bookingForUpdate.TimeFrom = bookingUpdateData.TimeFrom;
            bookingForUpdate.TimeTo = bookingUpdateData.TimeTo;
            bookingForUpdate.BookerName = bookingUpdateData.BookerName;

            try 
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }
           

            return true;
        }

        
    }
}
