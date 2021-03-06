﻿using Engine.Helpers;
using Engine.Model.Entities;
using Engine.Model.Server;
using System;

namespace Engine.API.StandardAPI.ServerCommands
{
  class ServerSetRoomAdminCommand :
      BaseServerCommand,
      ICommand<ServerCommandArgs>
  {
    public void Run(ServerCommandArgs args)
    {
      var receivedContent = Serializer.Deserialize<MessageContent>(args.Message);

      if (string.IsNullOrEmpty(receivedContent.RoomName))
        throw new ArgumentException("RoomName");

      if (receivedContent.NewAdmin == null)
        throw new ArgumentNullException("NewAdmin");

      if (string.Equals(receivedContent.RoomName, ServerModel.MainRoomName))
      {
        ServerModel.API.SendSystemMessage(args.ConnectionId, "Невозможно назначить администратора для главной комнаты.");
        return;
      }

      if (!RoomExists(receivedContent.RoomName, args.ConnectionId))
        return;

      using (var server = ServerModel.Get())
      {
        var room = server.Rooms[receivedContent.RoomName];

        if (!room.Admin.Equals(args.ConnectionId))
        {
          ServerModel.API.SendSystemMessage(args.ConnectionId, "Вы не являетесь администратором комнаты. Операция отменена.");
          return;
        }

        room.Admin = receivedContent.NewAdmin.Nick;

        var message = string.Format("Вы назначены администратором комнаты {0}.", room.Name);
        ServerModel.API.SendSystemMessage(receivedContent.NewAdmin.Nick, message);
      }
    }

    [Serializable]
    public class MessageContent
    {
      string roomName;
      User newAdmin;

      public string RoomName { get { return roomName; } set { roomName = value; } }
      public User NewAdmin { get { return newAdmin; } set { newAdmin = value; } }
    }

    public const ushort Id = (ushort)ServerCommand.SetRoomAdmin;
  }
}
