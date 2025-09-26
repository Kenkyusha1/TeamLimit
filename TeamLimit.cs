using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System; // Added for DateTime
using System.Linq;
using System.Collections.Generic;

public class TeamLimitConfig : BasePluginConfig
{
    public int MaxPlayersPerTeam { get; set; } = 5;
}

[MinimumApiVersion(340)]
public class TeamLimit : BasePlugin, IPluginConfig<TeamLimitConfig>
{
    public override string ModuleName => "TeamLimit";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Kenkyusha";

    public TeamLimitConfig Config { get; set; } = null!;

    // Dictionary to track last join attempt time per player and team
    private readonly Dictionary<(ulong SteamID, CsTeam Team), DateTime> _lastJoinAttempt = new();
    private const float CooldownSeconds = 10.0f;

    public void OnConfigParsed(TeamLimitConfig config)
    {
        Config = config;
        Server.PrintToConsole($"[TeamLimit] Config loaded: MaxPlayersPerTeam = {config.MaxPlayersPerTeam}");
    }

    public override void Load(bool hotReload)
    {
        AddCommandListener("jointeam", OnJoinTeam);
        Server.PrintToConsole("[TeamLimit] Plugin loaded successfully");
    }

    private int GetTeamCount(CsTeam team)
    {
        return Utilities.GetPlayers()
            .Where(p => p != null && p.IsValid && p.SteamID != 0 && p.TeamNum == (byte)team)
            .Count();
    }

    private HookResult OnJoinTeam(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || !player.IsValid || player.SteamID == 0 || info.ArgCount < 2)
        {
            return HookResult.Continue;
        }

        if (!int.TryParse(info.GetArg(1), out int teamInt))
        {
            return HookResult.Continue;
        }

        CsTeam newTeam = (CsTeam)teamInt;

        if (newTeam == CsTeam.Spectator || newTeam == CsTeam.None)
        {
            return HookResult.Continue;
        }

        if (newTeam != CsTeam.Terrorist && newTeam != CsTeam.CounterTerrorist)
        {
            return HookResult.Continue;
        }

        int currentCount = GetTeamCount(newTeam);

        if (currentCount >= Config.MaxPlayersPerTeam)
        {
            // Check cooldown for this player and team
            DateTime now = DateTime.UtcNow;
            var key = (player.SteamID, newTeam);
            bool isOnCooldown = _lastJoinAttempt.TryGetValue(key, out DateTime lastAttempt) &&
                                (now - lastAttempt).TotalSeconds < CooldownSeconds;

            // Always move to spectator if not already there
            if (player.Team != CsTeam.Spectator)
            {
                player.ChangeTeam(CsTeam.Spectator);
                Server.PrintToConsole($"[TeamLimit Debug] Moved {player.PlayerName} to spectator for attempting to join {newTeam}.");
            }

            // Only send message and sound if not on cooldown
            if (!isOnCooldown)
            {
                player.PrintToChat($"The {(newTeam == CsTeam.Terrorist ? "Terrorist" : "Counter-Terrorist")} team is full! You have been moved to spectator.");
                player.ExecuteClientCommand("play sounds/ui/weapon_cant_buy.vsnd");
                _lastJoinAttempt[key] = now;
                Server.PrintToConsole($"[TeamLimit Debug] Sent message and sound to {player.PlayerName} for {newTeam}.");
            }

            return HookResult.Stop;
        }

        // Clear cooldown for this team when successfully joining
        _lastJoinAttempt.Remove((player.SteamID, newTeam));
        return HookResult.Continue;
    }
}