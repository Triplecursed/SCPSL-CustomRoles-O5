using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;
using Respawning;
using Respawning.Waves;

namespace O5Abilities;

[CommandHandler(typeof(ClientCommandHandler))]
public class ChaosSpyCallCommand : ICommand
{
	public static bool Used;

	public string Command => "shchaos";

	public string[] Aliases => new string[1] { "spychaos" };

	public string Description => "Вызов Хаоса (1 раз за раунд).";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (Used)
		{
			response = "Уже использовано в этом раунде!";
			return false;
		}
		Player val = default;
		if (!Player.TryGet(sender, ref val) || !val.IsAlive)
		{
			response = "Только для живых.";
			return false;
		}
		if (O5AbilitiesPlugin.HasRole(val, "100"))
		{
			if (!Player.List.Any((Player p) =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				return (int)p.Role == 2;
			}))
			{
				response = "Нет наблюдателей для высадки Хаоса! Попытка не потрачена.";
				return false;
			}
			ChaosSpawnWave val2 = WaveManager.Waves.OfType<ChaosSpawnWave>().FirstOrDefault();
			if (val2 != null)
			{
				if (val2.RespawnTokens <= 0)
				{
					val2.RespawnTokens = 1;
				}
				WaveManager.InitiateRespawn((SpawnableWaveBase)(object)val2);
				Used = true;
				Server.SendBroadcast("ВНИМАНИЕ: Обнаружено прибытие Повстанцев Хаоса!", (ushort)10, (BroadcastFlags)0, false);
				response = "Повстанцы Хаоса вызваны!";
				return true;
			}
			response = "Волна Хаоса не найдена.";
			return false;
		}
		response = "Вы не Шпион Хаоса!";
		return false;
	}
}
