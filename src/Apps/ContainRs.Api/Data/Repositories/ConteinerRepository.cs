﻿using ContainRs.Engenharia.Conteineres;

namespace ContainRs.Api.Data.Repositories;

public class ConteinerRepository : BaseRepository<Conteiner>
{
    public ConteinerRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
