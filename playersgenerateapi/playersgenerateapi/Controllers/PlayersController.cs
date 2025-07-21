using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using playersgenerateapi.Models;

namespace playersgenerateapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PlayersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Players>> GetPlayers()
        {
            var players = new List<Players>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var command = new SqlCommand("SELECT PlayerID, PlayerName, PlayerLink, PlayerImg FROM players", conn);
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var player = new Players
                        {
                            PlayerID = reader["PlayerID"] as int? ?? 0,
                            PlayerName = reader["PlayerName"]?.ToString(),
                            PlayerLink = reader["PlayerLink"]?.ToString(),
                            PlayerImg = reader["PlayerImg"]?.ToString()
                        };

                        players.Add(player);
                    }
                }

                if (players.Count == 0)
                {
                    return NotFound("No players found.");
                }

                return Ok(players);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }
    }
}
