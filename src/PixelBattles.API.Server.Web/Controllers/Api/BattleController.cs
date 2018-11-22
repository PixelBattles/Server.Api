﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PixelBattles.API.DataTransfer;
using PixelBattles.API.DataTransfer.Battle;
using PixelBattles.API.Server.BusinessLogic.Managers;
using PixelBattles.API.Server.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PixelBattles.API.Server.Web.Controllers.Api
{
    [Route("api")]
    public class BattleController : BaseApiController
    {
        protected IBattleManager BattleManager { get; set; }

        public BattleController(
            IBattleManager battleManager,
            IMapper mapper,
            ILogger<BattleController> logger) : base(
                mapper: mapper,
                logger: logger)
        {
            BattleManager = battleManager ?? throw new ArgumentNullException(nameof(battleManager));
        }
        
        [HttpGet("battle/{battleId:guid}")]
        public async Task<IActionResult> GetBattleAsync(Guid battleId)
        {
            try
            {
                var battle = await BattleManager.GetBattleAsync(battleId);
                if (battle == null)
                {
                    return NotFound();
                }
                var result = Mapper.Map<Battle, BattleDTO>(battle);
                return Ok(result);
            }
            catch (Exception exception)
            {
                return OnException(exception, "Error while getting battle.");
            }
        }

        [HttpGet("battle")]
        public async Task<IActionResult> GetBattlesAsync(BattleFilterDTO battleFilterDTO)
        {
            try
            {
                var battleFilter = new BattleFilter()
                {
                    Name = battleFilterDTO.Name
                };

                var battles = await BattleManager.GetBattlesAsync(battleFilter);
                if (battles == null)
                {
                    return NotFound();
                }

                var battlesResult = Mapper.Map<IEnumerable<Battle>, IEnumerable<BattleDTO>>(battles);
                return Ok(battlesResult);
            }
            catch (Exception exception)
            {
                return OnException(exception, "Error while getting battles.");
            }
        }

        [HttpPost("battle")]
        public async Task<IActionResult> CreateBattleAsync([FromBody] CreateBattleDTO commandDTO)
        {
            try
            {
                CreateBattleCommand command = new CreateBattleCommand()
                {
                    Name = commandDTO.Name,
                    Description = commandDTO.Description
                };
                var result = await BattleManager.CreateBattleAsync(command);
                var resultDTO = Mapper.Map<CreateBattleResult, CreateBattleResultDTO>(result);
                return OnResult(resultDTO);
            }
            catch (Exception exception)
            {
                return OnException(exception, "Error while creating battle.");
            }
        }

        [HttpDelete("battle/{battleId:guid}")]
        public async Task<IActionResult> DeleteBattleAsync(Guid battleId)
        {
            try
            {
                DeleteBattleCommand command = new DeleteBattleCommand()
                {
                    BattleId = battleId
                };
                var result = await BattleManager.DeleteBattleAsync(command);
                var resultDTO = Mapper.Map<Result, ResultDTO>(result);
                return OnResult(resultDTO);
            }
            catch (Exception exception)
            {
                return OnException(exception, "Error while deleting battle.");
            }
        }

        [HttpPut("battle")]
        public async Task<IActionResult> UpdateBattleAsync([FromBody] UpdateBattleDTO commandDTO)
        {
            try
            {
                UpdateBattleCommand command = new UpdateBattleCommand()
                {
                    BattleId = commandDTO.BattleId,
                    Name = commandDTO.Name,
                    Description = commandDTO.Description
                };
                var result = await BattleManager.UpdateBattleAsync(command);
                var resultDTO = Mapper.Map<Result, ResultDTO>(result);
                return OnResult(resultDTO);
            }
            catch (Exception exception)
            {
                return OnException(exception, "Error while updating battle.");
            }
        }

        [HttpPost("battle/token")]
        public async Task<IActionResult> CreateBattleTokenAsync([FromBody] CreateBattleTokenDTO commandDTO)
        {
            try
            {
                var command = new CreateBattleTokenCommand()
                {
                    BattleId = commandDTO.BattleId,
                    UserId = Guid.Empty
                };
                var result = await BattleManager.CreateBattleTokenAsync(command);
                var resultDTO = Mapper.Map<CreateBattleTokenResult, CreateBattleTokenResultDTO>(result);
                return OnResult(resultDTO);
            }
            catch (Exception exception)
            {
                return OnException(exception, "Error while creating battle token.");
            }
        }
    }
}