using System;
using System.Collections.Generic;
using System.Linq;
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
                if (_locationRepository.LocationAlreadyExists(request.Name))
                {
                    throw new InvalidOperationException("El objeto que intenta insertar ya existe!");
                }
                var location = new Location
                {
                    Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : throw new ArgumentException("El nombre del local no puede estar vacío."),
                    Neighborhood = !string.IsNullOrWhiteSpace(request.Neighborhood) ? request.Neighborhood : throw new ArgumentException("El nombre del barrio no puede estar vacío."),
                };
                _locationRepository.Add(location);
            }
            catch
            {
                throw;
            }
        }

        public void AddOffice(AddOfficeRequest request)
        {
            try
            {
                var location = _locationRepository.AsEnumerable().Where(l => l.Name == request.LocationName).FirstOrDefault() ?? throw new ArgumentException("El local especificado no existe.");
                var office = new Office
                {
                    LocationName = request.LocationName,
                    Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : throw new ArgumentException("El nombre de la oficina no puede estar vacío."),
                    Capacity = request.MaxCapacity > 0 ? request.MaxCapacity : throw new ArgumentException("La capacidad de la oficina debe ser mayor que cero."),
                    OfficeResource = request.AvailableResources.ToArray()
                };
                if (_officeRepository.OfficeAlreadyExist(office)) throw new InvalidOperationException("El objeto que intenta insertar ya existe!");
                _officeRepository.Add(office);
            }
            catch(ArgumentException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void BookOffice(BookOfficeRequest request)
        {
            {
                try
                {
                    if (!_officeRepository.OfficeAlreadyExist(new Office { LocationName = request.LocationName, Name = request.OfficeName })) throw new ArgumentNullException("La oficina especificada no existe");

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
                    var bookings = GetBookings(request.LocationName, request.OfficeName);
                    var endTime = request.DateTime.Add(request.Duration);
                    var isAvailable = !bookings.Any(b =>
                        request.DateTime < b.DateTime.Add(b.Duration) && endTime > b.DateTime);

                    if (!isAvailable)
                    {
                        throw new InvalidOperationException("La oficina ya está reservada para el horario especificado.");
                    }

                    // Crear una nueva instancia de Booking
                    var booking = new Booking
                    {
                        OfficeName = request.OfficeName,
                        LocationName = request.LocationName,
                        StartTime = request.DateTime,
                        Duration = (int)request.Duration.TotalHours,
                        User = !string.IsNullOrWhiteSpace(request.UserName) ? request.UserName : throw new ArgumentException("El nombre de usuario no puede estar vacío."),
                    };

                    // Agregar la nueva reserva al repositorio y devolverla
                    _bookingRepository.Add(booking);
                }
                catch(ArgumentException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public IEnumerable<BookingDto> GetBookings(string locationName, string officeName)
        {
            try
            {
                var result = from booking in _bookingRepository.AsEnumerable()
                             join office in _officeRepository.AsEnumerable() on booking.OfficeName equals office.Name
                             join location in _locationRepository.AsEnumerable() on office.LocationName equals location.Name
                             where location.Name.Equals(locationName, StringComparison.OrdinalIgnoreCase)
                                && office.Name.Equals(officeName, StringComparison.OrdinalIgnoreCase)
                             select new BookingDto
                             {
                                 LocationName = location.Name,
                                 OfficeName = office.Name,
                                 DateTime = booking.StartTime,
                                 Duration = TimeSpan.FromHours(booking.Duration),
                                 UserName = booking.User
                             };

                return result;
            }
            catch
            {
                return Enumerable.Empty<BookingDto>();
            }
        }

        public IEnumerable<LocationDto> GetLocations()
        {
            try
            {
                return from location in _locationRepository.AsEnumerable()
                       select new LocationDto
                       {
                           Name = location.Name,
                           Neighborhood = location.Neighborhood,
                       };
            }
            catch
            {
                return Enumerable.Empty<LocationDto>();
            }
        }

        public IEnumerable<OfficeDto> GetOffices(string locationName)
        {
            try
            {
                var officesDto = from office in _officeRepository.AsEnumerable()
                                 join location in _locationRepository.AsEnumerable()
                                 on office.LocationName equals location.Name
                                 where location.Name.Equals(locationName, StringComparison.OrdinalIgnoreCase)
                                 select new OfficeDto
                                 {
                                     LocationName = location.Name,
                                     Name = office.Name,
                                     MaxCapacity = office.Capacity,
                                     AvailableResources = office.OfficeResource
                                 };

                return officesDto;
            }
            catch
            {
                return Enumerable.Empty<OfficeDto>();
            }
        }

        public IEnumerable<OfficeDto> GetOfficeSuggestions(SuggestionsRequest request)
        {
            try
            {
                var suggestions = _officeRepository.AsEnumerable()
                .Where(office => office.Capacity >= request.CapacityNeeded &&
                                 request.ResourcesNeeded.All(resource => office.OfficeResource.Contains(resource)))
                .Select(office => new
                {
                    Office = office,
                    Location = _locationRepository.AsEnumerable().FirstOrDefault(loc => loc.Name == office.LocationName)
                })
                .Where(officeLocation => officeLocation.Location != null)
                .Select(officeLocation => new OfficeDto
                {
                    LocationName = officeLocation.Location.Name,
                    Name = officeLocation.Office.Name,
                    MaxCapacity = officeLocation.Office.Capacity,
                    AvailableResources = officeLocation.Office.OfficeResource
                })
                .OrderByDescending(officeDto => officeDto.LocationName)
                .ThenBy(officeDto => officeDto.MaxCapacity)
                .ThenBy(officeDto => officeDto.AvailableResources.Except(request.ResourcesNeeded).Count());

                return suggestions;
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al obtener las sugerencias " + ex);
            }
        }        
    }
}