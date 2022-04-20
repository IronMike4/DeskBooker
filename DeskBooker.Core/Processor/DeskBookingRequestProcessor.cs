﻿using DeskBooker.Core.DataInterface;
using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Processor;

public class DeskBookingRequestProcessor
{
  private IDeskBookingRepository @object;

  public DeskBookingRequestProcessor(IDeskBookingRepository @object)
  {
    this.@object = @object;
  }

  public DeskBookingResult BookDesk(DeskBookingRequest request)
  {
    if (request == null)
    {
      throw new ArgumentNullException(nameof(request));
    }

    return new DeskBookingResult
    {
      FirstName = request.FirstName,
      LastName = request.LastName,
      Email = request.Email,
      Date = request.Date,
    };
  }
}