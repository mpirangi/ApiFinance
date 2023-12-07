using ApiFinance;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly string _connectionString = "workstation id=the-specialist;packet size=4096;user id=sa;data source=209.121.105.211;persist security info=True;initial catalog=Finance;password=*sql*123";

    [HttpGet("{symbol}")]
    public async Task<ActionResult<IEnumerable<Meta>>> GetStockData(string symbol)
    {
        try
        {  

            var stockDataList = new List<Meta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string sql = "SELECT TOP 31 Symbol, RegularMarketPrice, DateTime FROM Meta WHERE Symbol = @Symbol ORDER BY DateTime DESC";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Symbol", symbol);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        double? previousDayPrice = null;

                        while (await reader.ReadAsync())
                        {
                            var currentPrice = (double)Convert.ToDecimal(reader["RegularMarketPrice"]);
                            var currentDateTime = Convert.ToDateTime(reader["DateTime"]);

                            var variacaoDiaAnterior = previousDayPrice.HasValue
                                ? ((currentPrice - previousDayPrice.Value) / previousDayPrice.Value) * 100
                                : 0;

                            var variacaoDesdePrimeiraData = stockDataList.Count > 0
                                ? ((currentPrice - stockDataList.First().regularMarketPrice) / stockDataList.First().regularMarketPrice) * 100
                                : 0;

                            var metaData = new Meta
                            {
                                symbol = reader["Symbol"].ToString(),
                                regularMarketPrice = currentPrice,
                                variacaoDiaAnterior = variacaoDiaAnterior,
                                variacaoDesdePrimeiraData = variacaoDesdePrimeiraData,
                                dateTime = currentDateTime
                            };

                            stockDataList.Add(metaData);

                            previousDayPrice = currentPrice;
                        }
                    }
                }
            }

            return Ok(stockDataList);
        }
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    } 
}
