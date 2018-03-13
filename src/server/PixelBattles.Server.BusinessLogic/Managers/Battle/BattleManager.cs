﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PixelBattles.Server.BusinessLogic.Models;
using PixelBattles.Server.Core;
using PixelBattles.Server.DataStorage.Models;
using PixelBattles.Server.DataStorage.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PixelBattles.Server.BusinessLogic.Managers
{
    [Register(typeof(IBattleManager))]
    public class BattleManager : BaseManager, IBattleManager
    {
        protected IBattleStore BattleStore { get; set; }

        public BattleManager(
            IBattleStore battleStore,
            IHttpContextAccessor contextAccessor,
            ErrorDescriber errorDescriber,
            IMapper mapper,
            ILogger<BattleManager> logger)
            : base(
                  contextAccessor: contextAccessor,
                  errorDescriber: errorDescriber,
                  mapper: mapper,
                  logger: logger)
        {
            BattleStore = battleStore ?? throw new ArgumentNullException(nameof(battleStore));
        }

        public async Task<Battle> GetBattleAsync(Guid battleId)
        {
            ThrowIfDisposed();
            var battle = await BattleStore.GetBattleAsync(battleId, CancellationToken);
            return Mapper.Map<BattleEntity, Battle>(battle);
        }
        
        protected override void DisposeStores()
        {
            BattleStore.Dispose();
        }

        public async Task<CreateBattleResult> CreateBattleAsync(CreateBattleCommand command)
        {
            ThrowIfDisposed();

            if (String.IsNullOrWhiteSpace(command.Name))
            {
                return new CreateBattleResult(new Error("Empty name", "Name can't be empty"));
            }

            var existingBattle = await BattleStore.GetBattleAsync(command.Name, CancellationToken);
            if (existingBattle != null)
            {
                return new CreateBattleResult(new Error("Name duplication", "Battle with the same name is already exist"));
            }

            var battle = new BattleEntity()
            {
                Description = command.Description,
                Name = command.Name,
                Status = BattleStatusEntity.Running,
                UserBattles = new List<UserBattleEntity> { new UserBattleEntity { UserId = command.UserId } }
            };

            var result = await BattleStore.CreateAsync(battle, CancellationToken);
            if (result.Succeeded)
            {
                return new CreateBattleResult(battle.BattleId);
            }
            else
            {
                return new CreateBattleResult(result.Errors);
            }
        }

        public async Task<IEnumerable<Battle>> GetBattlesAsync(BattleFilter battleFilter)
        {
            ThrowIfDisposed();

            var battleEntityFilter = new BattleEntityFilter
            {
                Name = battleFilter.Name,
                UserId = battleFilter.UserId
            };

            var battles = await BattleStore.GetBattlesAsync(battleEntityFilter, CancellationToken);
            return Mapper.Map<IEnumerable<BattleEntity>, IEnumerable<Battle>>(battles);
        }
    }
}
