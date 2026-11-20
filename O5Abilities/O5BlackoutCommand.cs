using System;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace O5Abilities;

[CommandHandler(typeof(ClientCommandHandler))]
public class O5BlackoutCommand : ICommand
{
	public static bool Used;

	public string Command => "o5blackout";

	public string[] Aliases => new string[1] { "blackout" };

	public string Description => "Отключение света на 30 сек (1 раз за раунд).";

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
			foreach (RoomLightController instance in RoomLightController.Instances)
			{
				instance.ServerFlickerLights(30f);
			}
			Used = true;
			Server.SendBroadcast("Совет О5 отключил свет во всём комплексе на 30 секунд!", (ushort)10, (BroadcastFlags)0, false);
			response = "Свет отключен на 30 секунд!";
			return true;
		}
		response = "Вы не Совет О5!";
		return false;
	}
}
