﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Channel.Network.Sending;
using Aura.Channel.World;
using Aura.Mabi.Const;
using Aura.Mabi.Network;
using Aura.Shared.Network;
using Aura.Shared.Util;

namespace Aura.Channel.Network.Handlers
{
	public partial class ChannelServerHandlers : PacketHandlerManager<ChannelClient>
	{
		/// <summary>
		/// Sent when prop is "attacked".
		/// </summary>
		/// <example>
		/// 001 [00A1000100090001] Long   : 45317475545972737
		/// </example>
		[PacketHandler(Op.HitProp)]
		public void HitProp(ChannelClient client, Packet packet)
		{
			var entityId = packet.GetLong();

			// Check creature and region
			var creature = client.GetCreatureSafe(packet.Id);
			if (creature.Region == Region.Limbo || creature.IsDead)
				return;

			// Check prop
			var prop = creature.Region.GetProp(entityId);
			if (prop == null)
			{
				Log.Warning("HitProp: Player '{0}' tried to hit unknown prop '{1}'.", creature.Name, entityId.ToString("X16"));
				Send.ServerMessage(creature, "Unknown target.");
			}
			else
			{
				if (creature.GetPosition().InRange(prop.GetPosition(), 1500))
				{
					Send.HittingProp(creature, prop.EntityId);

					if (prop.Behavior != null)
					{
						prop.Behavior(creature, prop);
					}
					else
					{
						Log.Unimplemented("HitProp: No prop behavior for '{0}'.", prop.EntityIdHex);
					}
				}
				else
				{
					Send.Notice(creature, NoticeType.MiddleLower, Localization.Get("You're too far away."));
					Log.Warning("HitProp: Player '{0}' tried to hit prop out of range.", creature.Name);
				}
			}

			Send.HitPropR(creature);
		}

		/// <summary>
		/// Sent when prop is "touched".
		/// </summary>
		/// <remarks>
		/// Mabinogi, hitting and touching props since 2004.
		/// </remarks>
		/// <example>
		/// 001 [00A000010009042A] Long   : 45036000569263146
		/// </example>
		[PacketHandler(Op.TouchProp)]
		public void TouchProp(ChannelClient client, Packet packet)
		{
			var entityId = packet.GetLong();

			// Check creature and region
			var creature = client.GetCreatureSafe(packet.Id);
			if (creature.Region == Region.Limbo || creature.IsDead)
				return;

			// Check prop
			var prop = creature.Region.GetProp(entityId);
			if (prop == null)
			{
				Log.Warning("TouchProp: Player '{0}' tried to touch unknown prop '{1}'.", creature.Name, entityId.ToString("X16"));
				Send.ServerMessage(creature, "Unknown target.");
			}
			else
			{
				// TODO: Check shape positions?
				// Props can be quite big, and the center of it isn't necessarily
				// where the player will touch it, so a simple range check
				// doesn't work properly, e.g. with dungeon's boss doors.
				// Checking the distance to any of the shapes could
				// solve this problem.

				if (!creature.GetPosition().InRange(prop.GetPosition(), 1500))
					Log.Warning("TouchProp: Player '{0}' tried to touch prop '{1:X16}' out of range (Distance: {2}).", creature.Name, entityId, creature.GetPosition().GetDistance(prop.GetPosition()));

				{
					if (prop.Behavior != null)
					{
						prop.Behavior(creature, prop);
					}
					else
					{
						Log.Unimplemented("TouchProp: No prop behavior for '{0}'.", prop.EntityIdHex);
					}
				}
				//else
				//{
				//	Send.Notice(creature, NoticeType.MiddleLower, Localization.Get("You're too far away."));
				//	Log.Warning("TouchProp: Player '{0}' tried to touch prop out of range.", creature.Name);
				//}
			}

			Send.TouchPropR(creature);
		}
	}
}
