using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;

namespace DeskBooker.Core.Processor
{
  public class DeskBookingRequestProcessorTests
  {
    private DeskBookingRequestProcessor _processor;
    private DeskBookingRequest _request;
    private Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
    private Mock<IDeskRepository> _deskRepositoryMock;
    private List<Desk> _availableDesks;

    [SetUp]
    public void SetUp()
    {
      _request = new DeskBookingRequest
      {
        FirstName = "Mike",
        LastName = "Mahachi",
        Email = "iron@mike.com",
        Date = new DateTime(2022, 04, 10)
      };

      _availableDesks = new List<Desk> { new Desk() };

      _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();
      _deskRepositoryMock = new Mock<IDeskRepository>();
      _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_request.Date))
        .Returns(_availableDesks);

      _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
    }

    [Test]
    public void ShouldReturnDeskBookingResultWithRequestValues()
    {
      // Act 
      DeskBookingResult result = _processor.BookDesk(_request);

      // Assert
      Assert.That(result, Is.Not.Null);
      Assert.That(result.FirstName, Is.EqualTo(_request.FirstName));
      Assert.That(result.LastName, Is.EqualTo(_request.LastName));
      Assert.That(result.Email, Is.EqualTo(_request.Email));
      Assert.That(result.Date, Is.EqualTo(_request.Date));
    }

    [Test]
    public void ShouldThrowExceptionIfResultIsNull()
    {
      var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

      Assert.That(exception.ParamName, Is.EqualTo("request"));
    }

    [Test]
    public void ShouldSaveDeskBooking()
    {
      // Arrange 
      DeskBooking savedDeskBooking = null;
      _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
        .Callback<DeskBooking>(deskBooking =>
        {
          savedDeskBooking = deskBooking;
        });

      // Act
      _processor.BookDesk(_request);
      _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once());

      // Assert
      Assert.That(savedDeskBooking, Is.Not.Null);
      Assert.That(savedDeskBooking.FirstName, Is.EqualTo(_request.FirstName));
      Assert.That(savedDeskBooking.LastName, Is.EqualTo(_request.LastName));
      Assert.That(savedDeskBooking.Email, Is.EqualTo(_request.Email));
      Assert.That(savedDeskBooking.Date, Is.EqualTo(_request.Date));
    }

    [Test]
    public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
    {
      _availableDesks.Clear();

      _processor.BookDesk(_request);

      _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never());
    }
  }
}
