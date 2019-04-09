using System;
using System.Collections.Generic;
using System.Linq; 

//房间
public class Room
{
	//状态
	public enum Status
	{
		Prepare = 1,
		Fight = 2 ,
	}
	public Status status = Status.Prepare;
	//玩家
	public int maxPlayers = 6;
	public Dictionary<string,Player> list = new Dictionary<string,Player>();


	//添加玩家
	public bool AddPlayer(Player player)
	{
		lock (list) 
		{
			if (list.Count >= maxPlayers)
				return false;
			PlayerTempData tempData = player.tempData;
			tempData.room = this; 
			tempData.team = SwichTeam ();
			tempData.status = PlayerTempData.Status.Room;
			
			if(list.Count == 0)
				tempData.isOwner = true;
			string id = player.id;
			list.Add(id, player);
		}
		return true;
	}
	
	//分配队伍
	public int SwichTeam()
	{
		int count1 = 0;
		int count2 = 0;
		foreach(Player player in list.Values)
		{
			if(player.tempData.team == 1) count1++;
			if(player.tempData.team == 2) count2++;
		}
		if (count1 <= count2)
			return 1;
		else
			return 2;
	}

	//删除玩家
	public void DelPlayer(string id)
	{
		lock (list) 
		{
			if (!list.ContainsKey(id))
				return;
			bool isOwner = list[id].tempData.isOwner;
			list[id].tempData.status = PlayerTempData.Status.None;
			list.Remove(id);
			if(isOwner)
				UpdateOwner();
		}
	}

	//更换房主
	public void UpdateOwner()
	{
		lock (list) 
		{
			if(list.Count <= 0)
				return;
			
			foreach(Player player in list.Values)
			{
				player.tempData.isOwner = false;
			}
			
			Player p = list.Values.First();
			p.tempData.isOwner = true;
		}
	}

	//广播
	public void Broadcast(ProtocolBase protocol)
	{
		foreach(Player player in list.Values)
		{
			player.Send(protocol);
		}
	}

	//房间信息
	public ProtocolBytes GetRoomInfo()
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.AddString ("GetRoomInfo");
		//房间信息
		protocol.AddInt (list.Count);
		//每个玩家信息
		foreach(Player p in list.Values)
		{
			protocol.AddString(p.id);
			protocol.AddInt(p.tempData.team);
			protocol.AddInt(p.data.win);
			protocol.AddInt(p.data.fail);
			int isOwner = p.tempData.isOwner? 1: 0;
			protocol.AddInt(isOwner);
		}
		return protocol;
	}
}