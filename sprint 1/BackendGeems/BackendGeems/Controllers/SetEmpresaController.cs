﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

[Route("api/[controller]")]
[ApiController]
public class SetEmpresaController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public SetEmpresaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public class EmpresaModel
    {
        public string CedulaJuridica { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string Senas { get; set; }
    }

    [HttpPost("crearEmpresa")]
    public IActionResult CrearEmpresa([FromBody] EmpresaModel empresa)
    {
        if (empresa == null)
        {
            return BadRequest(new { message = "Datos de la empresa inválidos." });
        }

        try
        {
            using SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();

            string insertQuery = @"
                INSERT INTO Empresa (
                    CedulaJuridica,
                    Nombre,
                    Descripcion,
                    Telefono,
                    Correo,
                    Provincia,
                    Canton,
                    Distrito,
                    Senas
                )
                VALUES (
                    @CedulaJuridica,
                    @Nombre,
                    @Descripcion,
                    @Telefono,
                    @Correo,
                    @Provincia,
                    @Canton,
                    @Distrito,
                    @Senas
                );";

            using SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@CedulaJuridica", empresa.CedulaJuridica);
            cmd.Parameters.AddWithValue("@Nombre", empresa.Nombre);
            cmd.Parameters.AddWithValue("@Descripcion", empresa.Descripcion);
            cmd.Parameters.AddWithValue("@Telefono", empresa.Telefono);
            cmd.Parameters.AddWithValue("@Correo", empresa.Correo);
            cmd.Parameters.AddWithValue("@Provincia", empresa.Provincia);
            cmd.Parameters.AddWithValue("@Canton", empresa.Canton);
            cmd.Parameters.AddWithValue("@Distrito", empresa.Distrito);
            cmd.Parameters.AddWithValue("@Senas", empresa.Senas);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok(new { message = "Empresa creada exitosamente", empresa.CedulaJuridica });
            }
            else
            {
                return StatusCode(500, new { message = "No se pudo insertar la empresa." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al insertar empresa", error = ex.Message });
        }
    }
}
