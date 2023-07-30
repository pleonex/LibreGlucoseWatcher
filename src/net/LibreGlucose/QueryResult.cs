using PleOps.LibreGlucose.Connection;

namespace PleOps.LibreGlucose;

public record QueryResult<T>(int Status, T Data, TicketInfo Ticket);
