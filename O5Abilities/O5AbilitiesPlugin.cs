using System;
using System.Threading.Tasks;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using UncomplicatedCustomRoles.Extensions;

namespace O5Abilities;

public class O5AbilitiesPlugin : Plugin<object>
{
	private static int _roundId;

	public override string Name => "O5 Abilities";

	public override string Description => "Способности для О5, Шпиона Хаоса и Предателя МОГ.";

	public override string Author => "Admin";

	public override Version Version => new Version(1, 0, 0);

	public override Version RequiredApiVersion => new Version(1, 0, 0);

	public override void Enable()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		ServerEvents.RoundStarted += OnRoundStarted;
		PlayerEvents.Spawned += OnPlayerSpawned;
		PlayerEvents.Dying += OnPlayerDying;
		PlayerEvents.Hurting += OnPlayerHurting;
		Logger.Info((object)"[O5] Плагин успешно загружен и готов к работе!");
	}

	public override void Disable()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected Obj, but got Unknown
		ServerEvents.RoundStarted -= OnRoundStarted;
		PlayerEvents.Spawned -= OnPlayerSpawned;
		PlayerEvents.Dying -= OnPlayerDying;
		PlayerEvents.Hurting -= OnPlayerHurting;
	}

	private void OnRoundStarted()
	{
		_roundId++;
		O5CallMtfCommand.Used = false;
		O5BlackoutCommand.Used = false;
		O5FinalNukeCommand.Used = false;
		ChaosSpyCallCommand.Used = false;
	}

	public static bool HasRole(Player player, string roleId)
	{
		if (player == null)
		{
			return false;
		}
		if (player.TryGetSummonedInstance(out var summonedInstance) && summonedInstance != null)
		{
			string text = ((summonedInstance.Role != null) ? summonedInstance.Role.Id.ToString() : summonedInstance.Id);
			return text == roleId || summonedInstance.Id == roleId;
		}
		return false;
	}

	private void OnPlayerDying(PlayerDyingEventArgs ev)
	{
		Logger.Info((object)("[O5] ИГРОК УМИРАЕТ: " + ev.Player.Nickname));
		if (HasRole(ev.Player, "500"))
		{
			Logger.Info((object)"[O5] Это Совет О5! Запускаю Мёртвую руку...");
			Server.SendBroadcast("Совет О5 погиб! Протокол 'Мёртвая рука' активирован!", (ushort)10, (BroadcastFlags)0, false);
			if (!Warhead.IsDetonated)
			{
				Warhead.Start(false, false, (Player)null);
				Server.SendBroadcast("Протокол 'Мёртвая рука': Боеголовка запущена!", (ushort)10, (BroadcastFlags)0, false);
				Logger.Info((object)"[O5] Ядерка успешно запущена!");
			}
			else
			{
				Logger.Info((object)"[O5] Ядерка не запущена (уже взорвана).");
			}
		}
		else
		{
			Logger.Info((object)"[O5] Умирает не О5, игнорирую.");
		}
	}

	private async void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
	{
		await Task.Delay(500);
		if (ev.Player == null || !ev.Player.IsAlive)
		{
			return;
		}
		if (HasRole(ev.Player, "500"))
		{
			ev.Player.SendHint("Вы - Совет О5!\nКоманды (~): .o5mtf, .o5blackout, .o5nuke", 10f);
			ev.Player.SendConsoleMessage("Способности:\n.o5mtf - Вызов МОГ (1 раз)\n.o5blackout - Отключить свет на 30 сек (1 раз)\n.o5nuke - Судный день (если остался 1 из Фонда или прошло 20 мин)", "yellow");
			Logger.Info((object)"[O5] Роль 500 (Совет О5) распознана!");
		}
		else if (HasRole(ev.Player, "100"))
		{
			ev.Player.SendHint("Вы - Шпион Хаоса!\nКоманда (~): .shchaos", 10f);
			ev.Player.SendConsoleMessage("Способность:\n.shchaos - Вызов Хаоса (1 раз)", "green");
			Logger.Info((object)"[O5] Роль 100 (Шпион Хаоса) распознана!");
		}
		else if (HasRole(ev.Player, "600"))
		{
			int currentRound = _roundId;
			ev.Player.IsGodModeEnabled = true;
			ev.Player.SendHint("Вы - Предатель МОГ!\n15 секунд неуязвимости, стрельба заблокирована!", 10f);
			Logger.Info((object)"[O5] Роль 600 (Предатель) распознана! Включен GodMode.");
			await Task.Delay(15000);
			if (currentRound == _roundId && ev.Player != null && ev.Player.IsAlive)
			{
				ev.Player.IsGodModeEnabled = false;
				ev.Player.SendHint("Неуязвимость закончилась. Оружие разблокировано.", 5f);
				Logger.Info((object)"[O5] GodMode выключен для Предателя.");
			}
		}
	}

	private void OnPlayerHurting(PlayerHurtingEventArgs ev)
	{
		if (ev.Attacker != null && ev.Attacker.IsGodModeEnabled && HasRole(ev.Attacker, "600"))
		{
			ev.IsAllowed = false;
			ev.Attacker.SendHint("Вы не можете атаковать в режиме неуязвимости!", 1f);
		}
	}
}
