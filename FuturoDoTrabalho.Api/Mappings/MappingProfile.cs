using AutoMapper;
using FuturoDoTrabalho.Api.DTOs;
using FuturoDoTrabalho.Api.Models;

namespace FuturoDoTrabalho.Api.Mappings
{
    // ====================================================================================
    // MAPPING PROFILE: CONFIGURAÇÃO DO AUTOMAPPER
    // ====================================================================================
    // Define os mapeamentos entre Models (entidades do banco) e DTOs (objetos de transferência).
    // O AutoMapper facilita a conversão automática entre esses objetos, evitando código repetitivo.
    // ====================================================================================
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ====================================================================================
            // MAPEAMENTOS DE FUNCIONARIO
            // ====================================================================================

            // Model -> DTO (para leitura/consulta)
            CreateMap<Funcionario, FuncionarioReadDto>();

            // DTO -> Model (para criação)
            CreateMap<FuncionarioCreateDto, Funcionario>();

            // DTO -> Model (para atualização completa - PUT)
            CreateMap<FuncionarioUpdateDto, Funcionario>();

            // DTO -> Model (para atualização parcial - PATCH)
            CreateMap<FuncionarioPatchDto, Funcionario>();

            // ====================================================================================
            // MAPEAMENTOS DE DEPARTAMENTO
            // ====================================================================================

            // Model -> DTO (para leitura/consulta)
            CreateMap<Departamento, DepartamentoReadDto>();

            // DTO -> Model (para criação)
            CreateMap<DepartamentoCreateDto, Departamento>();

            // DTO -> Model (para atualização completa - PUT)
            CreateMap<DepartamentoUpdateDto, Departamento>();

            // DTO -> Model (para atualização parcial - PATCH)
            CreateMap<DepartamentoPatchDto, Departamento>();
        }
    }
}
