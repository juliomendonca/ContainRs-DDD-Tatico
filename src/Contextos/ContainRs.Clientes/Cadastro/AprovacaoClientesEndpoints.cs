﻿using ContainRs.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ContainRs.Clientes.Cadastro;

public static class AprovacaoClientesEndpoints
{
    public static IEndpointRouteBuilder MapAprovacaoClientesEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder
            .MapGroup(EndpointConstants.ROUTE_CLIENTES)
            .RequireAuthorization(policy => policy.RequireRole("Suporte"))
            .WithTags(EndpointConstants.TAG_CLIENTES)
            .WithOpenApi();

        group
            .MapApproveRegistroCliente()
            .MapRejectRegistroCliente();

        return builder;
    }

    public static RouteGroupBuilder MapApproveRegistroCliente(this RouteGroupBuilder builder)
    {
        builder.MapPatch("registration/{id:guid}/approve", async (
            [FromRoute] Guid id
            , [FromServices] IRepository<Cliente> repository
            , [FromServices] IAcessoManager userManager
            , CancellationToken cancellationToken) =>
        {
            var cliente = await repository
                .GetFirstAsync(
                    c => c.Id == id,
                    c => c.Id);
            if (cliente is null) return Results.NotFound();

            await userManager.AdicionarClienteAsync(cliente.Email.Value, cancellationToken);

            return Results.Ok(new RegistrationStatusResponse(cliente.Id.ToString(), cliente.Email.Value, "Aprovado"));
        })
        .Produces<RegistrationStatusResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
        return builder;
    }

    public static RouteGroupBuilder MapRejectRegistroCliente(this RouteGroupBuilder builder)
    {
        builder.MapPatch("registration/{id:guid}/reject", async (
            [FromRoute] Guid id
            , [FromServices] IRepository<Cliente> repository
            , [FromServices] IAcessoManager userManager
            , CancellationToken cancellationToken) =>
        {
            var cliente = await repository
                .GetFirstAsync(
                    c => c.Id == id,
                    c => c.Id);
            if (cliente is null) return Results.NotFound();

            await userManager.BloquearClienteAsync(cliente.Email.Value, cancellationToken);

            return Results.Ok(new RegistrationStatusResponse(cliente.Id.ToString(), cliente.Email.Value, "Registro não aprovado"));
        })
        .Produces<RegistrationStatusResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
        return builder;
    }
}
