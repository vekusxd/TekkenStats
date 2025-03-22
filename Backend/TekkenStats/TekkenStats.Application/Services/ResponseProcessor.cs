using TekkenStats.Core.Models;
using TekkenStats.DataAccess;

namespace TekkenStats.Application.Services;

public class ResponseProcessor
{
    private readonly AppDbContext _dbContext;

    public ResponseProcessor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //TODO
    public async Task ProcessResponse(WavuWankResponse response)
    {
        var player1Id = response.P1UserId;
        var player2Id = response.P2UserId;
    }
}