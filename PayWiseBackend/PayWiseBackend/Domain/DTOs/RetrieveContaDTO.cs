﻿namespace PayWiseBackend.Domain.DTOs;

public class RetrieveContaDTO
{
    public int Id { get; set; }
    public string? Numero { get; set; }
    public decimal Saldo { get; set; }
    public string? Agencia { get; set; }
    public string? HistoricoUrl { get; set; }
}
