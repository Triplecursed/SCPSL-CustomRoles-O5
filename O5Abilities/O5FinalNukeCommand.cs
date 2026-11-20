using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace O5Abilities;

[CommandHandler(typeof(ClientCommandHandler))]
public class O5FinalNukeCommand : ICommand
{
	public static bool Used;

	public string Command => "o5nuke";

	public string[] Aliases => new string[1] { "o5final" };

	public string Description => "Судный день (если О5 остался 1 из Фонда или прошло 20 мин).";

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
		if (O5AbilitiesPlugin.HasRole(val, "500"))
		{
			bool flag = Round.Duration.TotalMinutes >= 20.0;
			int num = Enumerable.Count(Player.List, (Player p) =>
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Invalid comparison between Unknown and I4
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Invalid comparison between Unknown and I4
				return p.IsAlive && ((int)p.Faction == 1 || (int)p.Role == 6);
			});
			if (!flag && num > 1)
			{
				response = $"Нельзя активировать сейчас! Нужно, чтобы прошло 20 минут раунда (сейчас {Math.Floor(Round.Duration.TotalMinutes)} мин) ИЛИ вы остались последним живым сотрудником Фонда.";
				return false;
			}
			if (Warhead.IsDetonated)
			{
				response = "Боеголовка уже взорвана!";
				return false;
			}
			Warhead.Start(false, false, (Player)null);
			Warhead.IsLocked = true;
			Used = true;
			Server.SendBroadcast("ПРОТОКОЛ СУДНОГО ДНЯ: Совет О5 запустил необратимую боеголовку!", (ushort)10, (BroadcastFlags)0, false);
			response = "Необратимая боеголовка запущена!";
			return true;
		}
		response = "Вы не Совет О5!";
		return false;
	}
}
