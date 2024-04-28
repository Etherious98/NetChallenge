using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Xml.Linq;
using NetChallenge.Abstractions;
using NetChallenge.Domain;
using NetChallenge.Dto.Input;
using NetChallenge.Dto.Output;

namespace NetChallenge
{
    public class OfficeRentalService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly IBookingRepository _bookingRepository;

        public OfficeRentalService(ILocationRepository locationRepository, IOfficeRepository officeRepository, IBookingRepository bookingRepository)
        {
            _locationRepository = locationRepository;
            _officeRepository = officeRepository;
            _bookingRepository = bookingRepository;
        }

        public void AddLocation(AddLocationRequest request)
        {
            try
            {
                var location = new Location
                {
                    Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : throw new Exception("El nombre del local no puede estar vacío."),
                    Neighborhood = !string.IsNullOrWhiteSpace(request.Neighborhood) ? request.Neighborhood : throw new Exception("El nombre del barrio no puede estar vacío."),
                };
                _locationRepository.Add(location);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void AddOffice(AddOfficeRequest request)
        {
            var location = _locationRepository.GetByName(request.LocationName) ?? throw new ArgumentException("El local especificado no existe.");
            var office = new Office
            {
                LocationId = location.Id,
                Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : throw new Exception("El nombre de la oficina no puede estar vacío."),
                Capacity = request.MaxCapacity > 0 ? request.MaxCapacity : throw new Exception("La capacidad de la oficina debe ser mayor que cero."),
            };
            _officeRepository.Add(office);
        }

        public void BookOffice(BookOfficeRequest request)
        {
            {
                // Obtener la locación por nombre
                var location = _locationRepository.AsEnumerable().FirstOrDefault(l => l.Name == request.LocationName);
                if (location == null)
                {
                    throw new ArgumentException("La locación especificada no existe.", nameof(request.LocationName));
                }

                // Obtener la oficina por nombre y locación
                var office = _officeRepository.GetAllByLocation(location.Id).FirstOrDefault(o => o.Name == request.OfficeName);
                if (office == null)
                {
                    throw new ArgumentException("La oficina especificada no existe en la locación proporcionada.", nameof(request.OfficeName));
                }

                // Validar que la fecha de inicio sea válida
                if (request.DateTime < DateTime.Now)
                {
                    throw new ArgumentException("La fecha de inicio no puede ser en el pasado.", nameof(request.DateTime));
                }

                // Validar que la duración sea mayor que cero
                if (request.Duration <= TimeSpan.Zero)
                {
                    throw new ArgumentException("La duración de la reserva debe ser mayor que cero.", nameof(request.Duration));
                }

                // Verificar disponibilidad de la oficina para el horario especificado
                var bookings = _bookingRepository.GetByOffice(office.Id);
                var endTime = request.DateTime.Add(request.Duration);
                var isAvailable = !bookings.Any(b =>
                    request.DateTime < b.StartTime.AddHours(b.Duration) && endTime > b.StartTime);

                if (!isAvailable)
                {
                    throw new InvalidOperationException("La oficina ya está reservada para el horario especificado.");
                }

                // Crear una nueva instancia de Booking
                var booking = new Booking
                {
                    OfficeId = office.Id,
                    StartTime = request.DateTime,
                    Duration = (int)request.Duration.TotalHours,
                    User = request.UserName
                };

                // Agregar la nueva reserva al repositorio y devolverla
                _bookingRepository.Add(booking);
            }
        }

        public IEnumerable<BookingDto> GetBookings(string locationName, string officeName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LocationDto> GetLocations()
        {
            return (IEnumerable<LocationDto>)_locationRepository.AsEnumerable();
        }

        public IEnumerable<OfficeDto> GetOffices(string locationName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OfficeDto> GetOfficeSuggestions(SuggestionsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}